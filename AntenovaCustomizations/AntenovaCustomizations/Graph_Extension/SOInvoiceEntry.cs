using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.AR;

namespace PX.Objects.SO
{
    public class SOInvoiceEntry_Extension : PXGraphExtension<SOInvoiceEntry>
    {
        public const string ShipmentNoticeRpt = "LM643001";

        public override void Initialize()
        {
            base.Initialize();
            Base.report.AddMenuAction(InvoiceTaiwanReport);
            Base.report.AddMenuAction(InvoiceUKReport);
            //Base.report.AddMenuAction(InvoiceSignatureReport);
            Base.report.AddMenuAction(printShipNotice);
            Base.report.AddMenuAction(printTaiwanShipNotice);
            Base.report.AddMenuAction(printUKShipNotice);
            //Base.report.AddMenuAction(printSignatureShipNotice);

        }

        #region Report Actions

        #region Invoice_Taiwan
        public PXAction<SOShipment> InvoiceTaiwanReport;
        [PXButton]
        [PXUIField(DisplayName = "Print Invoice-Taiwan", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable invoiceTaiwanReport(PXAdapter adapter)
        {
            var _reportID = "so643001";
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.Document.Current.DocType;
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            }
            return adapter.Get();
        }
        #endregion

        #region Invoice_UK
        public PXAction<SOShipment> InvoiceUKReport;
        [PXButton]
        [PXUIField(DisplayName = "Print Invoice-UK", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable invoiceUKReport(PXAdapter adapter)
        {
            var _reportID = "so643002";
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.Document.Current.DocType;
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            }
            return adapter.Get();
        }
        #endregion

        /*#region Invoice_Signature
        public PXAction<SOShipment> InvoiceSignatureReport;
        [PXButton]
        [PXUIField(DisplayName = "Print Invoice-Signature", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable invoiceSignatureReport(PXAdapter adapter)
        {
            var _reportID = "so643003";
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.Document.Current.DocType;
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            }
            return adapter.Get();
        }
        #endregion*/

        #region ShipmentNotice
        public PXAction<ARInvoice> printShipNotice;
        [PXButton]
        [PXUIField(DisplayName = "Print Shipment Notice", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable PrintShipNotice(PXAdapter adapter)
        {

            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();

                parameters[nameof(ARInvoice.DocType)] = Base.Document.Current.DocType;
                parameters[nameof(ARInvoice.RefNbr)]  = Base.Document.Current.RefNbr;

                throw new PXReportRequiredException(parameters, ShipmentNoticeRpt);
            }
            return adapter.Get();
        }
        #endregion

        #region TaiwanShipmentNotice
        public PXAction<ARInvoice> printTaiwanShipNotice;
        [PXButton]
        [PXUIField(DisplayName = "Print Shipment Notice-Taiwan", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable PrintTaiwanShipNotice(PXAdapter adapter)
        {
            var _reportID = "LM643002";
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();

                parameters[nameof(ARInvoice.DocType)] = Base.Document.Current.DocType;
                parameters[nameof(ARInvoice.RefNbr)] = Base.Document.Current.RefNbr;

                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            }
            return adapter.Get();
        }
        #endregion

        #region UKShipmentNotice
        public PXAction<ARInvoice> printUKShipNotice;
        [PXButton]
        [PXUIField(DisplayName = "Print Shipment Notice-UK", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable PrintUKShipNotice(PXAdapter adapter)
        {
            var _reportID = "LM643003";
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();

                parameters[nameof(ARInvoice.DocType)] = Base.Document.Current.DocType;
                parameters[nameof(ARInvoice.RefNbr)] = Base.Document.Current.RefNbr;

                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            }
            return adapter.Get();
        }
        #endregion

        /*#region SignatureShipmentNotice
        public PXAction<ARInvoice> printSignatureShipNotice;
        [PXButton]
        [PXUIField(DisplayName = "Print Shipment Notice-Signature", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable PrintSignatureShipNotice(PXAdapter adapter)
        {
            var _reportID = "LM643004";
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();

                parameters[nameof(ARInvoice.DocType)] = Base.Document.Current.DocType;
                parameters[nameof(ARInvoice.RefNbr)] = Base.Document.Current.RefNbr;

                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            }
            return adapter.Get();
        }
        #endregion*/

        #endregion
    }
}