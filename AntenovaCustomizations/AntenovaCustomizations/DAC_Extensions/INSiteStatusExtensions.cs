using System;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.SO;

namespace PX.Objects.IN
{
    public class INSiteStatusExt : PXCacheExtension<PX.Objects.IN.INSiteStatus>
    {
        #region UsrQtyCreditHold
        /// <summary>
        /// Specification(Shipment Related Customization).
        /// Get total SO line open quantity and only consider SO order as credit hold.
        /// </summary>
        [PXQuantity]
        [PXUIField(DisplayName = "Qty Credit Hold", Enabled = false)]
        [PXDBScalar(typeof(SelectFrom<SOLine>.InnerJoin<SOOrder>.On<SOOrder.orderType.IsEqual<SOLine.orderType>
                                                                    .And<SOOrder.orderNbr.IsEqual<SOLine.orderNbr>>>
                                             .Where<SOOrder.creditHold.IsEqual<True>
                                                    .And<SOLine.inventoryID.IsEqual<INSiteStatus.inventoryID>>
                                                         .And<SOLine.siteID.IsEqual<INSiteStatus.siteID>>>
                                             .AggregateTo<Sum<SOLine.openQty>>.SearchFor<SOLine.openQty>))]
        public virtual decimal? UsrQtyCreditHold { get; set; }
        public abstract class usrQtyCreditHold : PX.Data.BQL.BqlDecimal.Field<usrQtyCreditHold> { }
        #endregion
    }
}
