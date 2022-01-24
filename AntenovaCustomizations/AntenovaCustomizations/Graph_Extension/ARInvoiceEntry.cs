using System;
using PX.Data;
using PX.Objects.SO;

namespace PX.Objects.AR
{
    public class ARInvoiceEntry_Extension : PXGraphExtension<ARInvoiceEntry>
    {
        // Create this data view for notification template to get shipment header information on So invoice.
        [PXViewName(PX.Objects.SO.Messages.SOShipment)]
        public PXSelectJoin<SOShipment, InnerJoin<SOOrderShipment, On<SOOrderShipment.shipmentType, Equal<SOShipment.shipmentType>,
                                                                      And<SOOrderShipment.shipmentNbr, Equal<SOShipment.shipmentNbr>>>>,
                                        Where<SOOrderShipment.invoiceType, Equal<Current<ARInvoice.docType>>,
                                              And<SOOrderShipment.invoiceNbr, Equal<Current<ARInvoice.refNbr>>>>> Shipments;
    }
}
