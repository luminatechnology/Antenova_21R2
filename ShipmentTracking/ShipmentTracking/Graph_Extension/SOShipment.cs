using PX.Commerce.Objects;
using PX.Data;
using System;

namespace PX.Objects.SO
{
    public class SOShipmentExt2 : PXCacheExtension<BCSOShipmentExt, PX.Objects.SO.SOShipment>
    {
        #region UsrPacking
        [PXDBBool]
        [PXUIField(DisplayName = "Packing")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrPacking { get; set; }
        public abstract class usrPacking : PX.Data.BQL.BqlBool.Field<usrPacking> { }
        #endregion

        #region UsrSentCarrier
        [PXDBBool]
        [PXUIField(DisplayName = "Invoice Sent", IsReadOnly = true)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrSentCarrier { get; set; }
        public abstract class usrSentCarrier : PX.Data.BQL.BqlBool.Field<usrSentCarrier> { }
        #endregion
    }
}