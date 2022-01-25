using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.SO;
using System;
using System.Collections;
using System.Linq;

namespace ShipmentTracking
{
    public class MOShipmentTrkMaint : PXGraph<MOShipmentTrkMaint>
    {
        #region Select & Features        
        public PXSavePerRow<SOShipment> Save;
        public PXCancel<SOShipment> Cancel;
        [PXFilterable()]
        public SelectFrom<SOShipment>.View Shipment;
        #endregion

        #region Ctor
        public MOShipmentTrkMaint()
        {
            ActionMenu.AddMenuAction(Reset2UnsentCar);
        }
        #endregion

        #region Event Handlers
        protected virtual void _(Events.RowSelected<SOShipment> e)
        {
            PXUIFieldAttribute.SetEnabled<SOShipment.shipmentType>(e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<SOShipment.shipmentNbr>(e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<SOShipment.customerID>(e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<SOShipment.shipmentQty>(e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<SOShipment.shipDate>(e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<SOShipment.siteID>(e.Cache, e.Row, false);
        }
        #endregion

        #region Actions & Menu
        public PXAction<SOShipment> ActionMenu;
        [PXButton(MenuAutoOpen = true)]
        [PXUIField(DisplayName = "Actions")]
        public IEnumerable actionMenu(PXAdapter adapter) => adapter.Get();

        public PXAction<SOShipment> Reset2UnsentCar;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Reset To Unsent", MapEnableRights = PXCacheRights.Select)]
        public IEnumerable reset2UnsentCar(PXAdapter adapter)
        {
            foreach (SOShipment row in adapter.Get<SOShipment>().RowCast<SOShipment>().Where(ship => ship.GetExtension<SOShipmentExt2>().UsrSentCarrier == true && ship.Selected == true))
            {
                this.Shipment.Cache.SetValue<SOShipmentExt2.usrSentCarrier>(row, false);
                this.Shipment.Cache.MarkUpdated(row);
            }

            Actions.PressSave();

            return adapter.Get();
        }
        #endregion
    }
}