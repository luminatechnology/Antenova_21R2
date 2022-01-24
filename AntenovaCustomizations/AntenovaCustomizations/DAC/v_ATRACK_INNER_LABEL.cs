using System;
using PX.Data;

namespace AntenovaCustomizations
{
    [Serializable]
    [PXCacheName("v_ATRACK_INNER_LABEL")]
    public class v_ATRACK_INNER_LABEL : IBqlTable
    {
        #region NowNbr
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Now Nbr")]
        public virtual int? NowNbr { get; set; }
        public abstract class nowNbr : PX.Data.BQL.BqlInt.Field<nowNbr> { }
        #endregion

        #region ShipmentNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Shipment Nbr")]
        public virtual string ShipmentNbr { get; set; }
        public abstract class shipmentNbr : PX.Data.BQL.BqlString.Field<shipmentNbr> { }
        #endregion

        #region ShipDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Ship Date")]
        public virtual DateTime? ShipDate { get; set; }
        public abstract class shipDate : PX.Data.BQL.BqlDateTime.Field<shipDate> { }
        #endregion

        #region AlternateID
        [PXDBString(50, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Alternate ID")]
        public virtual string AlternateID { get; set; }
        public abstract class alternateID : PX.Data.BQL.BqlString.Field<alternateID> { }
        #endregion

        #region CustomerOrderNbr
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Customer Order Nbr")]
        public virtual string CustomerOrderNbr { get; set; }
        public abstract class customerOrderNbr : PX.Data.BQL.BqlString.Field<customerOrderNbr> { }
        #endregion

        #region UsrDateCode
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Usr Date Code")]
        public virtual string UsrDateCode { get; set; }
        public abstract class usrDateCode : PX.Data.BQL.BqlString.Field<usrDateCode> { }
        #endregion

        #region Attributes_INBOX_Value
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Attributes_ INBOX_ Value")]
        public virtual string Attributes_INBOX_Value { get; set; }
        public abstract class attributes_INBOX_Value : PX.Data.BQL.BqlString.Field<attributes_INBOX_Value> { }
        #endregion

        #region InventoryItemDescr
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory Item Descr")]
        public virtual string InventoryItemDescr { get; set; }
        public abstract class inventoryItemDescr : PX.Data.BQL.BqlString.Field<inventoryItemDescr> { }
        #endregion

        #region InventoryCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory CD")]
        public virtual string InventoryCD { get; set; }
        public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
        #endregion

        #region SumQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Sum Qty")]
        public virtual Decimal? SumQty { get; set; }
        public abstract class sumQty : PX.Data.BQL.BqlDecimal.Field<sumQty> { }
        #endregion
    }
}