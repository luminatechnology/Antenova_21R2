using AntenovaCustomizations;
using AntenovaCustomizations.DAC;
using AntenovaCustomizations.Descriptor;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Objects.EP;
using PX.TM;

namespace PX.Objects.CR
{
    public class CROpportunityExt : PXCacheExtension<CROpportunity>
    {
        #region UsrEndCust
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "End Customer")]
        [PXSelector(typeof(CRMendcust.custId),
          typeof(CRMendcust.name),
          DescriptionField = typeof(CRMendcust.name))]
        public virtual string UsrEndCust { get; set; }
        public abstract class usrendCust : PX.Data.BQL.BqlString.Field<usrendCust> { }
        #endregion

        #region UsrSalesPerson
        [PXDBInt]
        [PXUIField(DisplayName = "Sales Person")]
        [PXDefault(typeof(SearchFor<EPEmployee.salesPersonID>.Where<EPEmployee.userID.IsEqual<AccessInfo.userID.FromCurrent>>))]
        [PXSelector(typeof(SelectFrom<vSALESPERSONREGIONMAPPING>
                .InnerJoin<EPCompanyTreeMember>.On<EPCompanyTreeMember.workGroupID.IsEqual<vSALESPERSONREGIONMAPPING.workGroupID>>
                .InnerJoin<AR.SalesPerson>.On<AR.SalesPerson.salesPersonID.IsEqual<vSALESPERSONREGIONMAPPING.salespersonID>>
                .Where<EPCompanyTreeMember.contactID.IsEqual<AccessInfo.contactID.FromCurrent>>
                .AggregateTo<GroupBy<vSALESPERSONREGIONMAPPING.salespersonID, Max<vSALESPERSONREGIONMAPPING.salespersonID>>>
                .SearchFor<vSALESPERSONREGIONMAPPING.salespersonID>),
            typeof(vSALESPERSONREGIONMAPPING.salespersonCD),
            typeof(PX.Objects.AR.SalesPerson.isActive),
            SubstituteKey = typeof(vSALESPERSONREGIONMAPPING.salespersonCD),
            DescriptionField = typeof(vSALESPERSONREGIONMAPPING.descr))]
        public virtual int? UsrSalesPerson { get; set; }
        public abstract class usrSalesPerson : PX.Data.BQL.BqlInt.Field<usrSalesPerson> { }
        #endregion

        #region UsrSource
        [PXDBString(10, InputMask = "")]
        [PXUIField(DisplayName = "Source")]
        [PXSelector(typeof(CRMSource.sourceID),
            typeof(CRMSource.descrption))]
        public virtual string UsrSource { get; set; }
        public abstract class usrSource : PX.Data.BQL.BqlString.Field<usrSource> { }
        #endregion
    }
}
