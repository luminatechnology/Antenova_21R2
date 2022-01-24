using AntenovaCustomizations.Descriptor;
using PX.Data;
using PX.Objects.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.AR
{
    public class SalesPersonExt : PXCacheExtension<SalesPerson>
    {
        [PXStringList]
        [GetDropDownAttribute("REGION")]
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Sales Territory")]
        public string UsrSalesTerritory { get; set; }
        public abstract class usrSalesTerritory : PX.Data.BQL.BqlString.Field<usrSalesTerritory> { }
    }
}
