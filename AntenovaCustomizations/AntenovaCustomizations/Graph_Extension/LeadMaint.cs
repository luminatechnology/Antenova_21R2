using AntenovaCustomizations;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CR.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntenovaCustomizations.DAC;
using AntenovaCustomizations.Library;
using PX.Common;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR.MassProcess;
using PX.Objects.EP;
using PX.TM;

namespace PX.Objects.CR
{
    public class LeadMaint_Extension : PXGraphExtension<LeadWorkflow, LeadMaint>
    {
        /// <summary> RowSelected CRLead </summary>
        public void _(Events.RowSelected<CRLead> e, PXRowSelected baseMethod)
        {
            baseMethod?.Invoke(e.Cache,e.Args);
            var wgID = (e.Row as CRLead).WorkgroupID;
            var role = new PublicFunc().CheckAcessRoleByWP(PXAccess.GetContactID(), wgID);
            if (!role && wgID.HasValue)
                throw new PXException("You don't have right to read this data.");
        }

        #region Override DAC
        [PXDBInt]
        [PXUIField(DisplayName = "Workgroup")]
        [PXSelector(typeof(SelectFrom<EPCompanyTree>
           .InnerJoin<vSALESPERSONREGIONMAPPING>.On<EPCompanyTree.workGroupID.IsEqual<vSALESPERSONREGIONMAPPING.workGroupID>
               .And<vSALESPERSONREGIONMAPPING.contactID.IsEqual<AccessInfo.contactID.FromCurrent>>>
           .SearchFor<EPCompanyTree.workGroupID>),
           SubstituteKey = typeof(EPCompanyTree.description))]
        [PXMassUpdatableField]
        [PXMassMergableField]
        [PXMergeAttributes(Method = MergeMethod.Replace)]
        public void _(Events.CacheAttached<CRLead.workgroupID> e) { }
        #endregion

        /// <summary> Events.RowPersisting CRLead</summary>
        public void _(Events.RowPersisting<CRLead> e, PXRowPersisting baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            var row = e.Row as CRLead;
            if (row.Resolution == "OT" && string.IsNullOrEmpty(row.GetExtension<CRLeadExt>().UsrReasonNote))
                throw new PXException("Reason Note can not be empty");
        }

        /// <summary> Events.FieldDefaulting CRLead.workgroupID</summary>
        public void _(Events.FieldDefaulting<CRLead.workgroupID> e, PXFieldDefaulting baseMethod)
        {
           baseMethod?.Invoke(e.Cache,e.Args);
           e.NewValue = SelectFrom<EPCompanyTreeMember>
               .InnerJoin<EPCompanyTree>.On<EPCompanyTreeMember.workGroupID.IsEqual<EPCompanyTree.workGroupID>
                   .And<EPCompanyTree.parentWGID.IsEqual<P.AsInt>>>
               .Where<EPCompanyTreeMember.contactID.IsEqual<AccessInfo.contactID.FromCurrent>>
               .View.Select(Base,new PublicFunc().GetCRMWorkGroupID()).RowCast<EPCompanyTreeMember>().FirstOrDefault()?.WorkGroupID;
        }

        /// <summary> FieldDefaulting CROpportunity.ownerID </summary>
        public void _(Events.FieldDefaulting<CRLead.ownerID> e, PXFieldDefaulting baseMethod)
        {
            var row = e.Row as CRLead;
            var salesPerson = SelectFrom<EPEmployee>.Where<EPEmployee.userID.IsEqual<AccessInfo.userID.FromCurrent>>
                .View.Select(Base).RowCast<EPEmployee>().FirstOrDefault()?.SalesPersonID;
            baseMethod?.Invoke(e.Cache, e.Args);
            if (salesPerson.HasValue)
                e.NewValue = new PublicFunc().GetEmployeeBySalesPerson(salesPerson.Value);
        }

        /// <summary> Events.FieldUpdated CRLead.usrsalesPerson</summary>
        public void _(Events.FieldUpdated<CRLeadExt.usrsalesPerson> e)
        {
            var row = e.Row as CRLead;
            if (e.NewValue == null)
                return;
            var record = PXSelectorAttribute.Select<CRLeadExt.usrsalesPerson>(e.Cache, row) as vSALESPERSONREGIONMAPPING;
            e.Cache.SetValueExt<CRLead.workgroupID>(row, record.WorkGroupID ?? null);
            e.Cache.SetValueExt<CRLead.ownerID>(row,new PublicFunc().GetEmployeeBySalesPerson((int)e.NewValue));
        }
    }
}
