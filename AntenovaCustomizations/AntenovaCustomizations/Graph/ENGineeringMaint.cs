using AntenovaCustomizations.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.TM;
using AntenovaCustomizations.Library;
using PX.Common;

namespace AntenovaCustomizations.Graph
{
    public class ENGineeringMaint : PXGraph<ENGineeringMaint>
    {

        #region ENUM
        public enum ENGStatus : int
        {
            New = 1,
            Process = 2,
            Awaiting = 3,
            Hold = 4,
            Closed = 5,
            Completion = 6,
            Recycled = 7
        }
        #endregion

        #region View / Setup

        [PXHidden]
        public PXSetup<ENGSetup> setup;

        [PXViewName("Engineering")]
        public SelectFrom<ENGineering>.View Document;

        [PXViewName("Line")]
        public SelectFrom<ENGLine>
               .Where<ENGLine.engrRef.IsEqual<ENGineering.engrRef.FromCurrent>>.View Line;

        [PXViewName("CurrentLine")]
        public SelectFrom<ENGLine>
               .Where<ENGLine.engrRef.IsEqual<ENGineering.engrRef.FromCurrent>>.View CurrentLine;
        [PXViewName("RevenueLine")]
        public SelectFrom<ENGRevenueLine>
               .Where<ENGRevenueLine.engrRef.IsEqual<ENGineering.engrRef.FromCurrent>>.View RevenueLine;

        #endregion

        #region Action
        public PXSave<ENGineering> Save;
        public PXCancel<ENGineering> Cancel;
        public PXInsert<ENGineering> Insert;
        public PXCopyPasteAction<ENGineering> CopyPaste;
        public PXDelete<ENGineering> Delete;
        public PXFirst<ENGineering> First;
        public PXPrevious<ENGineering> Previous;
        public PXNext<ENGineering> Next;
        public PXLast<ENGineering> Last;

        #endregion

        #region Override DAC
        /// <summary> engNbr </summary>
        [PXDefault]
        [PXMergeAttributes(Method = MergeMethod.Append)]
        public void _(Events.CacheAttached<ENGineering.engNbr> e) { }
        #endregion

        #region Events

        #region Row Persisting

        /// <summary> ENGineering RowPersisting </summary>
        public void _(Events.RowPersisting<ENGineering> e)
        {
            var row = e.Row as ENGineering;
            // Delete do noting
            if (this.Document.Cache.Deleted.Count() != 0 || this.Line.Cache.Deleted.Count() != 0)
                return;

            ValidField(e);
            row.Status = ((int)AutoChangeStatus((ENGStatus)int.Parse(row.Status))).ToString();
            if ((ENGStatus)int.Parse(row.Status) == ENGStatus.Process && !this.Line.Current.ProcessDate.HasValue)
                this.Line.Current.ProcessDate = DateTime.Now;
        }

        /// <summary> ENGLine RowPersisting </summary>
        public void _(Events.RowPersisting<ENGLine> e)
            => ValidField(e);

        #endregion

        #region Row Selected

        /// <summary> RowSelected Engineering  </summary>
        public void _(Events.RowSelected<ENGineering> e)
        {
            var row = e.Row as ENGineering;
            var prjType = SelectFrom<ENGProjectType>.View.Select(this).RowCast<ENGProjectType>();

            // Valid Work Group Access right
            var wgID = (e.Row as ENGineering).SalesRegion;
            int? ToNullableInt(string val)
                => int.TryParse(val, out var i) ? (int?)i : null;

            var role = new PublicFunc().CheckAcessRoleByWP(PXAccess.GetContactID(), ToNullableInt(wgID));
            if (!role && !string.IsNullOrEmpty(wgID))
                throw new PXException("You don't have right to read this data.");

            // Init prjtype ddl
            if (e.Row != null)
            {
                PXStringListAttribute.SetList<ENGineering.prjtype>(
                    e.Cache,
                    e.Row,
                    prjType.Select(x => x.Prjtype).ToArray(),
                    prjType.Select(x => x.Description).ToArray());
            }

            // Gerber Info Visible
            if (row != null && row.Prjtype?.ToLower() == "gerber")
            {
                PXUIFieldAttribute.SetVisible<ENGLine.geberFile>(this.CurrentLine.Cache, null, true);
                PXUIFieldAttribute.SetVisible<ENGLine.file3D>(this.CurrentLine.Cache, null, true);
                PXUIFieldAttribute.SetVisible<ENGLine.stackUpFile>(this.CurrentLine.Cache, null, true);
                PXUIFieldAttribute.SetVisible<ENGLine.deviceTopology>(this.CurrentLine.Cache, null, true);
                PXUIFieldAttribute.SetVisible<ENGLine.pCBATopology>(this.CurrentLine.Cache, null, true);
            }
        }

        #endregion

        #region Field Updated

        /// <summary> FieldUpdated ENGineering.opprid </summary>
        public void _(Events.FieldUpdated<ENGineering.opprid> e)
        {
            var _graphOpportunity = PXGraph.CreateInstance<OpportunityMaint>();
            var row = e.Row as ENGineering;
            if (e.NewValue != null)
            {
                // Auto get Engineering Field Data
                CROpportunity _oppor = SelectFrom<CROpportunity>
                                    .Where<CROpportunity.opportunityID.IsEqual<P.AsString>>
                                    .View.ReadOnly.SelectSingleBound(this, new object[] { row }, e.NewValue);
                row.OppBAccountID = _oppor.BAccountID;
                row.EndCust = _oppor.GetExtension<CROpportunityExt>().UsrEndCust;
                row.SalesPerson = _oppor.GetExtension<CROpportunityExt>().UsrSalesPerson;
                row.SalesRegion = _oppor?.WorkgroupID.ToString();

                // Auto Get Revenule Line Data
                if (this.RevenueLine.Select().Count == 0)
                {
                    var _oppProduct = SelectFrom<CROpportunityProducts>
                                      .InnerJoin<CROpportunity>.On<CROpportunityProducts.quoteID.IsEqual<CROpportunity.defQuoteID>>
                                      .Where<CROpportunity.opportunityID.IsEqual<P.AsString>>
                                      .View.Select(this, row.Opprid).RowCast<CROpportunityProducts>();
                    foreach (var _prod in _oppProduct)
                    {
                        var _data = this.RevenueLine.Insert((ENGRevenueLine)this.RevenueLine.Cache.CreateInstance());
                        _data.InventoryID = _prod.InventoryID;
                        _data.Descr = _prod.Descr;
                        _data.Quantity = _prod.Quantity;
                        _data.Uom = _prod.UOM;
                        _data.UnitPrice = _prod.UnitPrice;
                        _data.ExtPrice = _prod.ExtPrice;
                    }
                }
            }
        }

        /// <summary> FieldUpdated ENGineering.engNbr </summary>
        public void _(Events.FieldUpdated<ENGineering.engNbr> e)
            => (e.Row as ENGineering).EngNbr = e.NewValue.ToString().ToUpper();

        /// <summary> FieldUpdated ENGRevenueLine.inventoryID </summary>
        public void _(Events.FieldUpdated<ENGRevenueLine.inventoryID> e)
        {
            var row = e.Row as ENGRevenueLine;
            if (e.NewValue != null)
            {
                InventoryItem _item = SelectFrom<InventoryItem>
                            .Where<InventoryItem.inventoryID.IsEqual<P.AsInt>>
                            .View.ReadOnly.SelectSingleBound(this, new object[] { row }, e.NewValue);
                row.Uom = _item.BaseUnit;
                row.Descr = _item.Descr;
            }
        }

        /// <summary> FieldUpdated ENGineering.salesPerson </summary>
        public void _(Events.FieldUpdated<ENGineering.salesPerson> e)
        {
            var row = e.Row as ENGineering;
            if (e.NewValue == null)
                return;
            var record = PXSelectorAttribute.Select<ENGineering.salesPerson>(e.Cache, row) as vSALESPERSONREGIONMAPPING;
            e.Cache.SetValueExt<ENGineering.salesRegion>(row, record.WorkGroupID?.ToString() ?? null);
        }

        #endregion

        #region Field Selecting

        /// <summary> FieldSelecting ENGLine engrAgeDays </summary>
        public void _(Events.FieldSelecting<ENGLine.engrAgeDays> e)
        {
            var row = e.Row as ENGLine;
            if (e.Row != null && row.ActComplete.HasValue && row.ProcessDate.HasValue)
                row.EngrAgeDays = (int)(row.ActComplete.Value.Date - row.ProcessDate.Value.Date).TotalDays;

        }

        /// <summary> FieldSelecting ENGineering prjtype </summary>
        public void _(Events.FieldSelecting<ENGineering.prjtype> e)
        {
            var row = e.Row as ENGineering;
            if (row != null && row.Prjtype?.ToLower() == "gerber")
            {
                PXUIFieldAttribute.SetVisible<ENGLine.geberFile>(this.CurrentLine.Cache, null, true);
                PXUIFieldAttribute.SetVisible<ENGLine.file3D>(this.CurrentLine.Cache, null, true);
                PXUIFieldAttribute.SetVisible<ENGLine.stackUpFile>(this.CurrentLine.Cache, null, true);
                PXUIFieldAttribute.SetVisible<ENGLine.deviceTopology>(this.CurrentLine.Cache, null, true);
                PXUIFieldAttribute.SetVisible<ENGLine.pCBATopology>(this.CurrentLine.Cache, null, true);
            }
        }

        #endregion

        #region Field Defaulting

        #endregion

        #endregion

        #region Function

        /// <summary> Valid ENGineering Row </summary>
        public void ValidField(Events.RowPersisting<ENGineering> e)
        {
            var row = e.Row as ENGineering;
            var line = this.CurrentLine.Cache.Current as ENGLine;

            #region Valid Opprid
            bool IsRqeuired = SelectFrom<ENGProjectType>
                                 .Where<ENGProjectType.prjtype.IsEqual<P.AsString>>
                                 .View.ReadOnly.SelectSingleBound(this, new object[] { row }, row.Prjtype)
                                 .RowCast<ENGProjectType>().FirstOrDefault()?.LinkOppr ?? false;
            if (IsRqeuired && string.IsNullOrEmpty(row.Opprid))
                e.Cache.RaiseExceptionHandling<ENGineering.opprid>(e.Row, row.Opprid,
                new PXSetPropertyException<ENGineering.opprid>("Opportunity Nbr can not be empty"));
            #endregion

            #region Valid Repeat
            if (string.IsNullOrEmpty(row.Repeat))
                e.Cache.RaiseExceptionHandling<ENGineering.repeat>(e.Row, row.Repeat,
                    new PXSetPropertyException<ENGineering.repeat>("Engineer Repeat can not be empty"));
            #endregion

            #region Valid Assign Engineer
            if (row.Engineer == null)
                e.Cache.RaiseExceptionHandling<ENGineering.engineer>(e.Row, row.Engineer,
                    new PXSetPropertyException<ENGineering.engineer>("Assign Engineer can not be empty"));

            #endregion

            #region Valid EngNbr
            if (string.IsNullOrEmpty(row.EngNbr))
                e.Cache.RaiseExceptionHandling<ENGineering.engNbr>(e.Row, row.EngNbr,
                    new PXSetPropertyException<ENGineering.engNbr>("Engineering Nbr can not be empty"));
            #endregion

            #region Valid [EngrRef] + [Engineer Repeat]
            var isExixts = new PXGraph().Select<ENGineering>()
                                        .Where(x => x.EngNbr == row.EngNbr &&
                                                    x.Repeat == row.Repeat &&
                                                    x.EngrRef != row.EngrRef).Count() > 0;
            if (isExixts && !string.IsNullOrEmpty(row.EngNbr))
                e.Cache.RaiseExceptionHandling<ENGineering.engNbr>(e.Row, row.EngNbr,
                  new PXSetPropertyException<ENGineering.engNbr>("[EngrNbr] + [Engineer Repeat] is not allowed duplicated"));
            #endregion

            #region  Valid Gerber Info

            if (row != null && row?.Prjtype?.ToLower() == "gerber" && this.Document.Cache.Deleted.Count() == 0)
            {
                if (line == null)
                    throw new PXException("Gerber Info Can not be Empty");
                if (string.IsNullOrEmpty(line.GeberFile))
                    this.CurrentLine.Cache.RaiseExceptionHandling<ENGLine.geberFile>(e.Row, line.GeberFile,
                        new PXSetPropertyException<ENGLine.geberFile>("Gerber File can not be empty"));
                if (string.IsNullOrEmpty(line.File3D))
                    this.CurrentLine.Cache.RaiseExceptionHandling<ENGLine.file3D>(e.Row, line.File3D,
                        new PXSetPropertyException<ENGLine.file3D>("3D File can not be empty"));
                if (string.IsNullOrEmpty(line.StackUpFile))
                    this.CurrentLine.Cache.RaiseExceptionHandling<ENGLine.stackUpFile>(e.Row, line.StackUpFile,
                        new PXSetPropertyException<ENGLine.stackUpFile>("Stack-Up File can not be empty"));
                if (string.IsNullOrEmpty(line.DeviceTopology))
                    this.CurrentLine.Cache.RaiseExceptionHandling<ENGLine.deviceTopology>(e.Row, line.DeviceTopology,
                        new PXSetPropertyException<ENGLine.deviceTopology>("Device Topology can not be empty"));
                if (string.IsNullOrEmpty(line.PCBATopology))
                    this.CurrentLine.Cache.RaiseExceptionHandling<ENGLine.pCBATopology>(e.Row, line.PCBATopology,
                        new PXSetPropertyException<ENGLine.pCBATopology>("PCBA Topology can not be empty"));

                this.CurrentLine.Cache.RaiseRowPersisting(this.CurrentLine.Cache.Current, PXDBOperation.Update);
            }

            #endregion

            #region Valid SalesPerson
            if (row.SalesPerson == null && row.Prjtype != "RD")
                e.Cache.RaiseExceptionHandling<ENGineering.salesPerson>(e.Row, row.Repeat,
                    new PXSetPropertyException<ENGineering.salesPerson>("Sales Person can not be empty"));
            #endregion
        }

        /// <summary> Valid ENGLine Row </summary>
        public void ValidField(Events.RowPersisting<ENGLine> e)
        {
            var doc = this.Document.Cache.Current as ENGineering;
            var row = e.Row as ENGLine ?? new ENGLine();

            #region Valid Gerber Nbr
            if (row.IsGerber.Value && string.IsNullOrEmpty(row.GerberNbr))
                e.Cache.RaiseExceptionHandling<ENGLine.gerberNbr>(e.Row, row.GerberNbr,
                    new PXSetPropertyException<ENGLine.gerberNbr>("Gerber Review Number can not be empty"));
            #endregion

            #region Valid Awaiting
            if (!string.IsNullOrEmpty(row.AwaitReason))
            {
                if (!row.AwaitdateFrom.HasValue)
                    e.Cache.RaiseExceptionHandling<ENGLine.awaitdateFrom>(e.Row, row.AwaitdateFrom,
                        new PXSetPropertyException<ENGLine.awaitdateFrom>("Awaite Date From can not be empty"));
                if (!row.AwaitdateTo.HasValue)
                    e.Cache.RaiseExceptionHandling<ENGLine.awaitdateTo>(e.Row, row.AwaitdateTo,
                       new PXSetPropertyException<ENGLine.awaitdateTo>("Awaite Date To can not be empty"));

            }
            else if (row.AwaitdateTo.HasValue || row.AwaitdateFrom.HasValue)
            {
                if (string.IsNullOrEmpty(row.AwaitReason))
                    e.Cache.RaiseExceptionHandling<ENGLine.awaitReason>(e.Row, row.AwaitReason,
                       new PXSetPropertyException<ENGLine.awaitReason>("Awaite Reason can not be empty"));
                if (!row.AwaitdateFrom.HasValue)
                    e.Cache.RaiseExceptionHandling<ENGLine.awaitdateFrom>(e.Row, row.AwaitdateFrom,
                        new PXSetPropertyException<ENGLine.awaitdateFrom>("Awaite Date From can not be empty"));
                if (!row.AwaitdateTo.HasValue)
                    e.Cache.RaiseExceptionHandling<ENGLine.awaitdateTo>(e.Row, row.AwaitdateTo,
                       new PXSetPropertyException<ENGLine.awaitdateTo>("Awaite Date To can not be empty"));
                if (!row.EstStart.HasValue)
                    e.Cache.RaiseExceptionHandling<ENGLine.estStart>(e.Row, row.EstStart,
                       new PXSetPropertyException<ENGLine.estStart>("Est. Start Date can not be empty"));
                if (!row.EstComplete.HasValue)
                    e.Cache.RaiseExceptionHandling<ENGLine.estComplete>(e.Row, row.EstComplete,
                       new PXSetPropertyException<ENGLine.estComplete>("Est. Complete Date can not be empty"));
            }
            if (row.AwaitdateFrom.HasValue && row.AwaitdateTo.HasValue && row.AwaitdateTo.Value < row.AwaitdateFrom)
                e.Cache.RaiseExceptionHandling<ENGLine.awaitdateTo>(e.Row, row.AwaitdateTo,
                      new PXSetPropertyException<ENGLine.awaitdateTo>("Awaite Date To must Greater then Await From Date"));
            #endregion

            #region Valid OnHold

            if (row.OnholdDate.HasValue || !string.IsNullOrEmpty(row.OnholdReason))
            {
                if (string.IsNullOrEmpty(row.OnholdReason))
                    e.Cache.RaiseExceptionHandling<ENGLine.onholdReason>(e.Row, row.OnholdReason,
                        new PXSetPropertyException<ENGLine.onholdReason>("On Hold Reason can not be empty"));
                if (!row.OnholdDate.HasValue)
                    e.Cache.RaiseExceptionHandling<ENGLine.onholdDate>(e.Row, row.OnholdDate,
                   new PXSetPropertyException<ENGLine.onholdDate>("On Hold Date can not be empty"));
            }

            #endregion

            #region Valid Close
            if (row.CloseDate.HasValue || !string.IsNullOrEmpty(row.CloseReason))
            {
                if (string.IsNullOrEmpty(row.CloseReason))
                    e.Cache.RaiseExceptionHandling<ENGLine.closeReason>(e.Row, row.CloseReason,
                        new PXSetPropertyException<ENGLine.closeReason>("Close Reason can not be empty"));
                if (!row.CloseDate.HasValue)
                    e.Cache.RaiseExceptionHandling<ENGLine.closeDate>(e.Row, row.CloseDate,
                   new PXSetPropertyException<ENGLine.closeDate>("Close Date can not be empty"));
            }
            #endregion

            #region Valid Complete

            if (row.ActComplete.HasValue && string.IsNullOrEmpty(row.CompleteSummary))
                e.Cache.RaiseExceptionHandling<ENGLine.completeSummary>(e.Row, row.CompleteSummary,
                       new PXSetPropertyException<ENGLine.completeSummary>("Report Summary can not be empty"));

            if (!row.ActComplete.HasValue && !string.IsNullOrEmpty(row.CompleteSummary))
                e.Cache.RaiseExceptionHandling<ENGLine.actComplete>(e.Row, row.ActComplete,
                      new PXSetPropertyException<ENGLine.actComplete>("Actual Complete Date can not be empty"));

            #endregion

            #region Valid Gerber Info

            if (doc != null && doc?.Prjtype?.ToLower() == "gerber")
            {
                if (string.IsNullOrEmpty(row.GeberFile))
                    e.Cache.RaiseExceptionHandling<ENGLine.geberFile>(e.Row, row.GeberFile,
                        new PXSetPropertyException<ENGLine.geberFile>("Gerber File can not be empty"));
                if (string.IsNullOrEmpty(row.File3D))
                    e.Cache.RaiseExceptionHandling<ENGLine.file3D>(e.Row, row.File3D,
                        new PXSetPropertyException<ENGLine.file3D>("3D File can not be empty"));
                if (string.IsNullOrEmpty(row.StackUpFile))
                    e.Cache.RaiseExceptionHandling<ENGLine.stackUpFile>(e.Row, row.StackUpFile,
                        new PXSetPropertyException<ENGLine.stackUpFile>("Stack-Up File can not be empty"));
                if (string.IsNullOrEmpty(row.DeviceTopology))
                    e.Cache.RaiseExceptionHandling<ENGLine.deviceTopology>(e.Row, row.DeviceTopology,
                        new PXSetPropertyException<ENGLine.deviceTopology>("Device Topology can not be empty"));
                if (string.IsNullOrEmpty(row.PCBATopology))
                    e.Cache.RaiseExceptionHandling<ENGLine.pCBATopology>(e.Row, row.PCBATopology,
                        new PXSetPropertyException<ENGLine.pCBATopology>("PCBA Topology can not be empty"));
            }

            #endregion

        }

        /// <summary> AutoChange Status </summary>
        public ENGStatus AutoChangeStatus(ENGStatus _orgStatus)
        {
            var doc = this.Document.Cache.Current as ENGineering;
            var line = this.Line.Cache.Current as ENGLine ?? this.Line.Cache.CreateInstance() as ENGLine;
            if (doc == null)
                return _orgStatus;

            if (line.ActComplete.HasValue && !string.IsNullOrEmpty(line.CompleteSummary))
                return ENGStatus.Completion;

            Dictionary<ENGStatus, DateTime?> dic = new Dictionary<ENGStatus, DateTime?>()
            {
                { ENGStatus.Process, line.ActStart ?? new DateTime()},
                { ENGStatus.Awaiting, line.AwaitdateFrom ?? new DateTime()},
                { ENGStatus.Hold, line.OnholdDate ?? new DateTime()},
                { ENGStatus.Closed, line.CloseDate ?? new DateTime()}
            };

            foreach (var item in dic.OrderByDescending(x => x.Value.Value))
            {
                switch (item.Key)
                {
                    case ENGStatus.Process:
                        if (line.ActStart.HasValue && line.EstComplete.HasValue)
                            return ENGStatus.Process;
                        break;
                    case ENGStatus.Awaiting:
                        if (line.EstStart.HasValue && line.EstComplete.HasValue && line.AwaitdateFrom.HasValue && line.AwaitdateTo.HasValue && !string.IsNullOrEmpty(line.AwaitReason))
                            return ENGStatus.Awaiting;
                        break;
                    case ENGStatus.Hold:
                        if (line.OnholdDate.HasValue && !string.IsNullOrEmpty(line.OnholdReason))
                            return ENGStatus.Hold;
                        break;
                    case ENGStatus.Closed:
                        if (line.CloseDate.HasValue && !String.IsNullOrEmpty(line.CloseReason))
                            return ENGStatus.Closed;
                        break;
                }
            }
            return _orgStatus;
        }

        #endregion

    }
}
