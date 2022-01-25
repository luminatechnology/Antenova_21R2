using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.SO;

namespace ShipmentTracking
{
    public class MOShipTrackingProc : PXGraph<MOShipTrackingProc>
    {
        #region Features
        public PXCancel<SOProcessFilter> Cancel;
        public PXFilter<SOProcessFilter> Filter;

        [PXFilterable()]
        public PXFilteredProcessingJoinGroupBy<SOShipment, SOProcessFilter, InnerJoin<SOOrderShipment, On<SOOrderShipment.shipmentType, Equal<SOShipment.shipmentType>,
                                                                                                           And<SOOrderShipment.shipmentNbr, Equal<SOShipment.shipmentNbr>>>>,
                                                                             Where<SOShipmentExt.usrWaybill, IsNotNull,
                                                                                   And<SOShipment.shipmentType, NotEqual<SOShipmentType.transfer>,
                                                                                       And<SOOrderShipment.invoiceNbr, IsNotNull>>>,
                                                                             Aggregate<GroupBy<SOOrderShipment.shipmentNbr>>> Records;
        #endregion

        #region Delegate Data View
        public virtual IEnumerable records()
        {
            SOProcessFilter filter = Filter.Current;

            if (filter.Action == PXAutomationMenuAttribute.Undefined) { yield break; }

            PXView view = new PXView(this, true, Records.View.BqlSelect);

            int totalRow = 0;
            int startRow = PXView.StartRow;

            if (filter.Action.StartsWith("Email Invoice & Shipment$"))
            {
                view.WhereAnd<Where<SOShipmentExt2.usrSentCarrier, Equal<False>, Or<SOShipmentExt2.usrSentCarrier, IsNull>>>();
            }

            foreach (var row in view.Select(PXView.Currents, PXView.Parameters, PXView.Searches, PXView.SortColumns, PXView.Descendings, Records.View.GetExternalFilters(), ref startRow, PXView.MaximumRows, ref totalRow))
            {
                yield return row;
            }

            PXView.StartRow = 0;
        }
        #endregion

        #region Event Handler
        protected void _(Events.RowSelected<SOProcessFilter> e)
        {
            if (e.Row == null) return;

            SOProcessFilter filter = e.Row as SOProcessFilter;

            if (filter != null && !String.IsNullOrEmpty(filter.Action))
            {
                Dictionary<string, object> parameters = Filter.Cache.ToDictionary(filter);

                Records.SetProcessTarget(null, null, null, filter.Action, parameters);
            }
        }
        #endregion 
    }
}