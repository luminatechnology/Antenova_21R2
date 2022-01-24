using System;
using PX.Data;

namespace AntenovaCustomizations
{
  [Serializable]
  [PXCacheName("v_TECHCOM_INNER_LABEL")]
  public class v_TECHCOM_INNER_LABEL : IBqlTable
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

    #region PartNumber
    [PXDBString(50, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Part Number")]
    public virtual string PartNumber { get; set; }
    public abstract class partNumber : PX.Data.BQL.BqlString.Field<partNumber> { }
    #endregion

    #region UsrDateCode
    [PXDBString(50, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Usr Date Code")]
    public virtual string UsrDateCode { get; set; }
    public abstract class usrDateCode : PX.Data.BQL.BqlString.Field<usrDateCode> { }
    #endregion

    #region Qtyinbox_attributes
    [PXDBString(255, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Qtyinbox_attributes")]
    public virtual string Qtyinbox_attributes { get; set; }
    public abstract class qtyinbox_attributes : PX.Data.BQL.BqlString.Field<qtyinbox_attributes> { }
    #endregion

    #region DeliveryDate
    [PXDBDate()]
    [PXUIField(DisplayName = "Delivery Date")]
    public virtual DateTime? DeliveryDate { get; set; }
    public abstract class deliveryDate : PX.Data.BQL.BqlDateTime.Field<deliveryDate> { }
    #endregion

    #region UsrCountry
    [PXDBString(50, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Usr Country")]
    public virtual string UsrCountry { get; set; }
    public abstract class usrCountry : PX.Data.BQL.BqlString.Field<usrCountry> { }
    #endregion

    #region Spec
    [PXDBString(256, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Spec")]
    public virtual string Spec { get; set; }
    public abstract class spec : PX.Data.BQL.BqlString.Field<spec> { }
    #endregion

    #region MadeIn
    [PXDBString(60, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Made In")]
    public virtual string MadeIn { get; set; }
    public abstract class madeIn : PX.Data.BQL.BqlString.Field<madeIn> { }
    #endregion

    #region Remarks
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Remarks")]
    public virtual string Remarks { get; set; }
    public abstract class remarks : PX.Data.BQL.BqlString.Field<remarks> { }
    #endregion

    #region InventoryID
    [PXDBInt()]
    [PXUIField(DisplayName = "Inventory ID")]
    public virtual int? InventoryID { get; set; }
    public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
    #endregion

    #region SumQty
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Sum Qty")]
    public virtual Decimal? SumQty { get; set; }
    public abstract class sumQty : PX.Data.BQL.BqlDecimal.Field<sumQty> { }
    #endregion
  }
}