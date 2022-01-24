using AntenovaCustomizations;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CR.MassProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntenovaCustomizations.DAC;
using AntenovaCustomizations.Descriptor;
using PX.Objects.EP;
using PX.TM;

namespace PX.Objects.CR
{
    public class CRLeadExt : PXCacheExtension<PX.Objects.CR.CRLead>
    {

        [PXDBString(1000, IsUnicode = true, BqlField = typeof(Standalone.CRLead.description))]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string Description { get; set; }

        #region UsrEndCust

        [PXDBString(200, IsUnicode = true, InputMask = "", BqlField = typeof(Standalone.CRLeadStandaloneExt.usrEndCust))]
        [PXUIField(DisplayName = "End Customer")]
        [PXSelector(typeof(CRMendcust.custId),
                    typeof(CRMendcust.name),
                    DescriptionField = typeof(CRMendcust.name))]
        public virtual string UsrEndCust { get; set; }
        public abstract class usrEndCust : PX.Data.BQL.BqlString.Field<usrEndCust> { }
        #endregion

        #region UsrReasonNote

        [PXDBString(200, BqlField = typeof(Standalone.CRLeadStandaloneExt.usrReasonNote))]
        [PXUIField(DisplayName = "Reason Note")]
        public virtual string UsrReasonNote { get; set; }
        public abstract class usrReasonNote : PX.Data.BQL.BqlString.Field<usrReasonNote> { }

        #endregion

        #region UsrSource
        [PXDBString(10, InputMask = "", BqlField = typeof(Standalone.CRLeadStandaloneExt.usrSource))]
        [PXUIField(DisplayName = "Source")]
        [PXSelector(typeof(CRMSource.sourceID),
            typeof(CRMSource.descrption))]
        public virtual string UsrSource { get; set; }
        public abstract class usrSource : PX.Data.BQL.BqlString.Field<usrSource> { }
        #endregion

        #region UsrSalesPerson
        [PXDBInt(BqlField = typeof(Standalone.CRLeadStandaloneExt.usrsalesPerson))]
        [PXUIField(DisplayName = "Sales Person")]
        [PXDefault(typeof(SearchFor<EPEmployee.salesPersonID>.Where<EPEmployee.userID.IsEqual<AccessInfo.userID.FromCurrent>>))]
        [PXSelector(typeof(SelectFrom<vSALESPERSONREGIONMAPPING>
                .InnerJoin<EPCompanyTreeMember>.On<EPCompanyTreeMember.workGroupID.IsEqual<vSALESPERSONREGIONMAPPING.workGroupID>>
                .InnerJoin<AR.SalesPerson>.On<AR.SalesPerson.salesPersonID.IsEqual<vSALESPERSONREGIONMAPPING.salespersonID>>
                .Where<EPCompanyTreeMember.contactID.IsEqual<AccessInfo.contactID.FromCurrent>>
                .AggregateTo<GroupBy<vSALESPERSONREGIONMAPPING.salespersonID,Max<vSALESPERSONREGIONMAPPING.salespersonID>>>
                .SearchFor<vSALESPERSONREGIONMAPPING.salespersonID>),
            typeof(vSALESPERSONREGIONMAPPING.salespersonCD),
            typeof(PX.Objects.AR.SalesPerson.isActive),
            SubstituteKey = typeof(vSALESPERSONREGIONMAPPING.salespersonCD),
            DescriptionField = typeof(vSALESPERSONREGIONMAPPING.descr))]
        public virtual int? UsrSalesPerson { get; set; }
        public abstract class usrsalesPerson : PX.Data.BQL.BqlInt.Field<usrsalesPerson> { }
        #endregion

    }
}
