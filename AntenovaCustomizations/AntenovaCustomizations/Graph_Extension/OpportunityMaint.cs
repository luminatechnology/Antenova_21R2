using PX.Data;
using PX.Data.BQL.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntenovaCustomizations.DAC;
using PX.Data.BQL;
using PX.Objects.CS;
using PX.Objects.AR;
using AntenovaCustomizations.Graph;
using System.Collections;
using static PX.Objects.CR.OpportunityMaint;
using PX.Objects.SO;
using AntenovaCustomizations.Library;
using PX.Objects.Common.Extensions;
using PX.Objects.CR.MassProcess;
using PX.Objects.CR.Standalone;
using PX.TM;
using PX.Objects.CR.Extensions.CRCreateSalesOrder;

namespace PX.Objects.CR
{
    public class OpportunityMaint_Etension : PXGraphExtension<OpportunityMaint>
    {
        public PXSetup<ENGSetup> setup;

        public SelectFrom<ENGineering>
                .Where<ENGineering.opprid.IsEqual<CROpportunity.opportunityID.FromCurrent>>.View ENGList;

        public SelectFrom<ENGLine>
               .InnerJoin<ENGineering>.On<ENGLine.engrRef.IsEqual<ENGineering.engrRef>>
               .Where<ENGineering.opprid.IsEqual<CROpportunity.opportunityID.FromCurrent>>.View ENGListLine;

        public SelectFrom<SOOrder>
                .InnerJoin<SOLine>.On<SOLine.orderType.IsEqual<SOOrder.orderType>.And<SOLine.orderNbr.IsEqual<SOOrder.orderNbr>>>
                .Where<SOLineExt.usrOpportunityID.IsEqual<CROpportunity.opportunityID.FromCurrent>>
                .View Soorders;

        public PublicFunc library = new PublicFunc();

        #region Override DAC

        [PXDefault]
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Engineering Ref", Required = true)]
        [AutoNumber(typeof(ENGSetup.eNGSequenceID), typeof(AccessInfo.businessDate))]
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        public void _(Events.CacheAttached<ENGineering.engrRef> e) { }

        [PXDBInt(BqlField = typeof(CROpportunityRevision.workgroupID))]
        [PXUIField(DisplayName = "Workgroup")]
        [PXSelector(typeof(SelectFrom<EPCompanyTree>
                .InnerJoin<vSALESPERSONREGIONMAPPING>.On<EPCompanyTree.workGroupID.IsEqual<vSALESPERSONREGIONMAPPING.workGroupID>
                    .And<vSALESPERSONREGIONMAPPING.contactID.IsEqual<AccessInfo.contactID.FromCurrent>>>
                .SearchFor<EPCompanyTree.workGroupID>),
            SubstituteKey = typeof(EPCompanyTree.description))]
        [PXMassUpdatableField]
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        public void _(Events.CacheAttached<CROpportunity.workgroupID> e) { }

        #endregion

        #region Action
        public PXAction<ENGineering> viewENGDoc;

        [PXButton]
        [PXUIField(Visible = false)]
        public virtual IEnumerable ViewENGDoc(PXAdapter adapter)
        {
            var row = this.ENGList.Current;
            var graph = PXGraph.CreateInstance<ENGineeringMaint>();
            if (row != null)
            {
                graph.Document.Current = SelectFrom<ENGineering>
                                         .Where<ENGineering.engrRef.IsEqual<P.AsString>>
                                         .View.Select(Base, row.EngrRef);
                PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);
            }
            return adapter.Get();
        }

        public PXAction<CROpportunity> viewSOOrder;
        [PXButton]
        [PXUIField(Visible = false, Enabled = true)]
        public virtual IEnumerable ViewSOOrder(PXAdapter adapter)
        {
            var row = this.Soorders.Current;
            if (row != null)
            {
                var graph = PXGraph.CreateInstance<SOOrderEntry>();
                graph.Document.Current = graph.Document.Search<SOOrder.orderType, SOOrder.orderNbr>(row.OrderType, row.OrderNbr);
                PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);
            }
            return adapter.Get();
        }

        #endregion

        #region Events

        #region RowSelected

        /// <summary> RowSelected CROpportunity </summary>
        public void _(Events.RowSelected<CROpportunity> e, PXRowSelected baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            var wgID = (e.Row as CROpportunity).WorkgroupID;
            var role = new PublicFunc().CheckAcessRoleByWP(PXAccess.GetContactID(), wgID);
            if (!role && wgID.HasValue)
                throw new PXException("You don't have right to read this data.");
        }

        #endregion

        #region RowInserting
        /// <summary> RowInserting CROpportunity </summary>
        public void _(Events.RowInserting<CROpportunity> e, PXRowInserting baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            var row = e.Row;
            if (row != null && row.LeadID.HasValue)
            {
                var leads = SelectFrom<CRLead>
                    .Where<CRLead.noteID.IsEqual<P.AsGuid>>
                    .View.Select(Base, row.LeadID).RowCast<CRLead>().FirstOrDefault();
                // UsrEndCust
                row.GetExtension<CROpportunityExt>().UsrEndCust =
                    string.IsNullOrEmpty(row.GetExtension<CROpportunityExt>().UsrEndCust)
                        ? leads.GetExtension<CRLeadExt>().UsrEndCust
                        : row.GetExtension<CROpportunityExt>().UsrEndCust;
                // UsrSource
                row.GetExtension<CROpportunityExt>().UsrSource =
                    string.IsNullOrEmpty(row.GetExtension<CROpportunityExt>().UsrSource)
                        ? leads.GetExtension<CRLeadExt>().UsrSource
                        : row.GetExtension<CROpportunityExt>().UsrSource;
                // UsrSalesPerson
                row.GetExtension<CROpportunityExt>().UsrSalesPerson =
                    row.GetExtension<CROpportunityExt>().UsrSalesPerson == null
                        ? leads.GetExtension<CRLeadExt>().UsrSalesPerson
                        : row.GetExtension<CROpportunityExt>().UsrSalesPerson;
            }
        }
        #endregion

        #region RowPersisting

        /// <summary> RowPersisting CSAnswers </summary>
        public void _(Events.RowPersisting<CSAnswers> e, PXRowPersisting baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            var stageID = Base.Opportunity.Current.StageID;
            var status = Base.Opportunity.Current.Status;
            var resolution = Base.Opportunity.Current.Resolution;
            var row = e.Row as CSAnswers;
            if (status == "L" && resolution == "OT" && row.AttributeID == "OPPOLTNM" && string.IsNullOrEmpty(row.Value))
            {
                Base.Answers.Cache.RaiseExceptionHandling<CSAnswers.value>(e.Row, row.Value,
                    new PXSetPropertyException<CSAnswers.value>("Oppoltnm Can not be Empty"));
                Base.Cancel.Press();
                throw new PXException(@"Please Maintain 「Lost Other Reasons」 in Attribute and Save data before Closed as Lost !");
            }

            if (stageID == "MP" && row.AttributeID == "FULMPDATE" && string.IsNullOrEmpty(row.Value))
            {
                Base.Answers.Cache.RaiseExceptionHandling<CSAnswers.value>(e.Row, row.Value,
                    new PXSetPropertyException<CSAnswers.value>("Full MP Date Can not be Empty"));
                throw new PXException("Full MP Date Can not be Empty");
            }
        }

        /// <summary> RowPersisting ENGineering </summary>
        public void _(Events.RowPersisting<ENGineering> e)
        {
            var row = (ENGineering)e.Row;
            row.OppBAccountID = Base.Opportunity.Current.BAccountID;
            var _oppID = Base.Opportunity.Current.OpportunityID;
            if ((row.Opprid ?? string.Empty).ToLower().Contains("new"))
                e.Row.Opprid = _oppID;

            // Valid [EngrRef] + [Engineer Repeat]
            var isExixts = new PXGraph().Select<ENGineering>()
                .Where(x => x.EngNbr == row.EngNbr &&
                            x.Repeat == row.Repeat &&
                            x.EngrRef != row.EngrRef).Any();
            if (isExixts && !string.IsNullOrEmpty(row.EngNbr))
                e.Cache.RaiseExceptionHandling<ENGineering.engNbr>(e.Row, row.EngNbr,
                    new PXSetPropertyException<ENGineering.engNbr>("[EngrNbr] + [Engineer Repeat] is not allowed duplicated"));

            // Valid SalesPerson
            if (row.SalesPerson == null && row.Prjtype != "RD")
                e.Cache.RaiseExceptionHandling<ENGineering.salesPerson>(e.Row, row.Repeat,
                    new PXSetPropertyException<ENGineering.salesPerson>("Sales Person can not be empty"));

        }

        #endregion

        #region RowPersisted

        /// <summary> RowPersisted ENGineering </summary>
        public void _(Events.RowPersisted<ENGineering> e)
        {
            int count = 0;
            var row = e.Row as ENGineering;

            if (string.IsNullOrEmpty(row.Description) || string.IsNullOrEmpty(row.Prjtype) || string.IsNullOrEmpty(row.Priority) || string.IsNullOrEmpty(row.SalesRegion))
                return;

            var _RevenueData = new PXGraph().Select<ENGRevenueLine>().Where(x => x.EngrRef == row.EngrRef);
            if (_RevenueData.Count() == 0)
            {
                var _graph = PXGraph.CreateInstance<ENGineeringMaint>();
                var _oppProduct = Base.Products.Select().RowCast<CROpportunityProducts>();
                foreach (var _prod in _oppProduct)
                {
                    var _data = _graph.RevenueLine.Insert(_graph.RevenueLine.Cache.CreateInstance() as ENGRevenueLine);
                    _data.EngrRef = row.EngrRef;
                    _data.InventoryID = _prod.InventoryID;
                    _data.Descr = _prod.Descr;
                    _data.Quantity = _prod.Quantity;
                    _data.Uom = _prod.UOM;
                    _data.UnitPrice = _prod.UnitPrice;
                    _data.ExtPrice = _prod.ExtPrice;
                    _data.LineNbr = ++count;
                }
                _graph.Actions.PressSave();
                // update reveCntr
                PXUpdate<Set<ENGineering.reveCntr, Required<ENGineering.reveCntr>>,
                         ENGineering,
                         Where<ENGineering.engrRef, Equal<Required<ENGineering.engrRef>>
                         >>.Update(Base, count, row.EngrRef);
            }

        }

        #endregion

        #region FieldSelecting

        /// <summary> Set engrRef Disabled </summary>
        public void _(Events.FieldSelecting<ENGineering.engrRef> e)
            => PXUIFieldAttribute.SetEnabled<ENGineering.engrRef>(e.Cache, null, false);

        /// <summary> Initial Project Type DDL </summary>
        public void _(Events.FieldSelecting<ENGineering.prjtype> e)
        {
            var prjType = SelectFrom<ENGProjectType>.View.Select(Base).RowCast<ENGProjectType>();
            if (e.Row != null)
            {
                PXStringListAttribute.SetList<ENGineering.prjtype>(
                    e.Cache,
                    null,
                    prjType.Select(x => x.Prjtype).ToArray(),
                    prjType.Select(x => x.Description).ToArray());
            }
        }

        /// <summary> Set opprid Enabled </summary>
        public void _(Events.FieldSelecting<ENGineering.opprid> e)
        {
            if (e.Row != null)
                PXUIFieldAttribute.SetEnabled<ENGineering.opprid>(e.Cache, null, false);
        }

        #endregion

        #region FieldDefaulting

        /// <summary> Set opprid value </summary>
        public void _(Events.FieldDefaulting<ENGineering.opprid> e)
        {
            var AutoOppID = SelectFrom<ENGProjectType>
                            .Where<ENGProjectType.prjtype.IsEqual<P.AsString>>
                            .View.Select(Base, (e.Row as ENGineering).Prjtype).RowCast<ENGProjectType>().FirstOrDefault()?.LinkOppr ?? false;
            if (AutoOppID)
                e.NewValue = (Base.Opportunity.Cache.Current as CROpportunity).OpportunityID;
        }

        /// <summary> EndCust Default value </summary>
        public void _(Events.FieldDefaulting<ENGineering.endCust> e)
            => e.NewValue = Base.Opportunity.Cache.GetValueExt<CROpportunityExt.usrendCust>(Base.Opportunity.Current);

        /// <summary> Events.FieldUpdated ENGineering.salesPerson </summary>
        public void _(Events.FieldDefaulting<ENGineering.salesPerson> e)
            => e.NewValue = (Base.Opportunity.Cache.GetValueExt<CROpportunityExt.usrSalesPerson>(Base.Opportunity.Current) as PXStringState).Value;

        /// <summary> Events.FieldUpdated ENGineering.salesRegion </summary>
        public void _(Events.FieldDefaulting<ENGineering.salesRegion> e)
            => e.NewValue = Base.Opportunity.Current.WorkgroupID?.ToString();

        /// <summary> FieldDefaulting CROpportunity.workgroupID </summary>
        public void _(Events.FieldDefaulting<CROpportunity.workgroupID> e, PXFieldDefaulting baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            e.NewValue = SelectFrom<EPCompanyTreeMember>
                .InnerJoin<EPCompanyTree>.On<EPCompanyTreeMember.workGroupID.IsEqual<EPCompanyTree.workGroupID>
                    .And<EPCompanyTree.parentWGID.IsEqual<P.AsInt>>>
                .Where<EPCompanyTreeMember.contactID.IsEqual<AccessInfo.contactID.FromCurrent>>
                .View.Select(Base, library.GetCRMWorkGroupID()).RowCast<EPCompanyTreeMember>().FirstOrDefault()?.WorkGroupID;
        }

        /// <summary> FieldDefaulting CROpportunity.ownerID </summary>
        public void _(Events.FieldDefaulting<CROpportunity.ownerID> e, PXFieldDefaulting baseMethod)
        {
            var row = e.Row as CROpportunity;
            var salesPerson = SelectFrom<EP.EPEmployee>.Where<EP.EPEmployee.userID.IsEqual<AccessInfo.userID.FromCurrent>>
                .View.Select(Base).RowCast<EP.EPEmployee>().FirstOrDefault()?.SalesPersonID;
            baseMethod?.Invoke(e.Cache, e.Args);
            if (salesPerson.HasValue)
                e.NewValue = library.GetEmployeeBySalesPerson(salesPerson.Value);
        }

        #endregion

        #region FieldUpdated

        /// <summary> Events.FieldUpdated ENGineering.prjtype </summary>
        public void _(Events.FieldUpdated<ENGineering.prjtype> e)
        {
            var AutoOppID = SelectFrom<ENGProjectType>
                           .Where<ENGProjectType.prjtype.IsEqual<P.AsString>>
                           .View.Select(Base, e.NewValue).RowCast<ENGProjectType>().FirstOrDefault()?.LinkOppr ?? false;
            if (AutoOppID)
                e.Cache.SetValueExt<ENGineering.opprid>(e.Row, (Base.Opportunity.Cache.Current as CROpportunity).OpportunityID);
        }

        /// <summary> Events.FieldUpdated CROpportunityExt.usrSalesPerson </summary>
        public void _(Events.FieldUpdated<CROpportunityExt.usrSalesPerson> e)
        {
            var row = e.Row as CROpportunity;
            if (e.NewValue == null)
                return;
            var record = PXSelectorAttribute.Select<CROpportunityExt.usrSalesPerson>(e.Cache, row) as vSALESPERSONREGIONMAPPING;
            e.Cache.SetValueExt<CROpportunity.workgroupID>(row, record.WorkGroupID ?? null);
            e.Cache.SetValueExt<CROpportunity.ownerID>(row, library.GetEmployeeBySalesPerson((int)e.NewValue));
        }

        /// <summary> Events.FieldUpdated ENGineering.salesPerson </summary>
        public void _(Events.FieldUpdated<ENGineering.salesPerson> e)
        {
            var row = e.Row as ENGineering;
            if (e.NewValue == null)
                return;
            var record = PXSelectorAttribute.Select<ENGineering.salesPerson>(e.Cache, row) as vSALESPERSONREGIONMAPPING;
            e.Cache.SetValueExt<ENGineering.salesRegion>(row, record.WorkGroupID?.ToString() ?? null);
        }

        /// <summary> FieldUpdated ENGineering.engref </summary>
        public void _(Events.FieldUpdated<ENGineering.engNbr> e)
            => (e.Row as ENGineering).EngNbr = e.NewValue.ToString().ToUpper();

        #endregion

        #endregion
    }
}
