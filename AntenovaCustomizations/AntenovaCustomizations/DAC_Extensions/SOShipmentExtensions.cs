using PX.Data;
using PX.Data.BQL;
using PX.Objects.AR;
using PX.Objects.CS;
using System;
using static PX.Objects.SO.SOShipmentEntry_Extension;

namespace PX.Objects.SO
{
    public class SOShipmentExt : PXCacheExtension<PX.Objects.SO.SOShipment>
    {
        // Hide this field
        #region WorkgroupID  
        [PXDBInt]
        [PX.TM.PXCompanyTreeSelector]
        [PXFormula(typeof(Selector<SOShipment.customerID, Selector<Customer.workgroupID, TM.EPCompanyTree.description>>))]
        [PXUIField(DisplayName = "Workgroup", Visibility = PXUIVisibility.Visible, Visible = false)]
        public int? WorkgroupID { get; set; }
        #endregion

        #region UsrCarrierPluginID
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Carrier", Enabled = false)]
        [PXSelector(typeof(Search<CarrierPlugin.carrierPluginID>),
                    typeof(CarrierPlugin.carrierPluginID),
                    typeof(CarrierPlugin.pluginTypeName),
                    typeof(CarrierPlugin.description))]
        public virtual string UsrCarrierPluginID { get; set; }
        public abstract class usrCarrierPluginID : PX.Data.BQL.BqlString.Field<usrCarrierPluginID> { }
        #endregion

        #region UsrWaybill
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Waybill", Enabled = false)]
        public virtual string UsrWaybill { get; set; }
        public abstract class usrWaybill : PX.Data.BQL.BqlString.Field<usrWaybill> { }
        #endregion

        #region UsrNote
        [PXString(InputMask = "", IsUnicode = true)]
        [PXUIField(IsReadOnly = true)]
        public virtual string UsrNote { get; set; }
        public abstract class usrNote : PX.Data.BQL.BqlString.Field<usrNote> { }
        #endregion

        #region UsrShipTrackURL
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Shipment Tracking URL", Enabled = false)]
        public virtual string UsrShipTrackURL => GetShipWaybillURL(this.UsrCarrierPluginID, this.UsrWaybill);
        public abstract class usrShipTrackURL : PX.Data.BQL.BqlString.Field<usrShipTrackURL> { }
        #endregion
    }
}