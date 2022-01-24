using AntenovaCustomizations.Descriptor;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntenovaCustomizations.DAC_Extensions
{
    public class CROpportunityStandaloneExt : PXCacheExtension<PX.Objects.CR.Standalone.CROpportunity>
    {
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "End Customer")]
        public virtual string UsrEndCust { get; set; }
        public abstract class usrendCust : PX.Data.BQL.BqlString.Field<usrendCust> { }

        [PXDBInt]
        [PXUIField(DisplayName = "Sales Person")]
        public virtual int? UsrSalesPerson { get; set; }
        public abstract class usrSalesPerson : PX.Data.BQL.BqlInt.Field<usrSalesPerson> { }

        [PXDBString(10, InputMask = "")]
        [PXUIField(DisplayName = "Source")]
        public virtual string UsrSource { get; set; }
        public abstract class usrSource : PX.Data.BQL.BqlString.Field<usrSource> { }
    }
}
