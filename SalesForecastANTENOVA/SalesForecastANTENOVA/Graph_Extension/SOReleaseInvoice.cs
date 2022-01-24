using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.TX;
using PX.Objects.GL;
using PX.Objects.AR.MigrationMode;
using PX.Objects.CR;
using PX.Objects;
using PX.Objects.SO;

namespace PX.Objects.SO
{
    public class SOReleaseInvoice_Extension : PXGraphExtension<SOReleaseInvoice>
    {
        #region Event Handlers
        [PXDBCurrency(typeof(ARInvoice.curyInfoID), typeof(ARInvoice.taxTotal))]
        [PXUIField(DisplayName = "Tax Amount", Visibility = PXUIVisibility.Visible, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        protected virtual void ARInvoice_CuryTaxTotal_CacheAttached(PXCache sender) { }  
        #endregion
    }
}