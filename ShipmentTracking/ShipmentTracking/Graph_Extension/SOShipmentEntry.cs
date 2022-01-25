using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Data.Reports;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Reports;
using PX.Reports.Data;

namespace PX.Objects.SO
{
    public class SOShipmentEntry_Extension : PXGraphExtension<SOShipmentEntry>
    {
        #region Constant Class & String Variables
        public string reportID;
        public string rptFormat;
        public string fileExt;

        [InjectDependency]
        protected IReportLoaderService ReportLoader { get; private set; }

        [InjectDependency]
        protected IReportDataBinder ReportDataBinder { get; private set; }

        public const string InvoiceRpt     = "SO643000";
        public const string PackListRpt    = "LM642005";
        public const string PckQtyExceeded = "Packing Qty Cannot Exceed Remaining Qty.";
        public const string NoInvoiceData  = "No Invoice Data.";
        public const string NoCustContAddr = "The Customer [{0}] Doesn't Have Contact Email Address.";
        #endregion
  
        #region Actions
         public PXAction<SOShipment> emailToCarrier;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Send Email To Carrier", Visible = false)]
        public virtual IEnumerable EmailToCarrier(PXAdapter adapter) => Notification(adapter, "SO INVOICE");

        public PXAction<SOShipment> notification;
        [PXUIField(DisplayName = "", Visible = false)]
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntryF)]
        public virtual IEnumerable Notification(PXAdapter adapter,
        [PXString]
        string notificationCD)
        {
            reportID = PackListRpt;
            rptFormat = PX.Reports.Data.RenderType.FilterPdf;
            fileExt = PX.Objects.CN.Common.Descriptor.Constants.PdfFileExtension;

            foreach (SOShipment shipment in adapter.Get<SOShipment>().RowCast<SOShipment>().Where(ship => ship.Selected == true))
            {
                try
                {
                    Base.Document.Current = shipment;

                    VerifyCustomerMailInfo(InvoiceRpt);

                    // Report Paramenters
                    Dictionary<string, string> packingParms = new Dictionary<string, string>();
                    Dictionary<string, string> invoiceParms = new Dictionary<string, string>();

                    foreach (SOOrderShipment orderShip in SelectFrom<SOOrderShipment>.Where<SOOrderShipment.shipmentType.IsEqual<@P.AsString>
                                                                                            .And<SOOrderShipment.shipmentNbr.IsEqual<@P.AsString>>>
                                                                                     .AggregateTo<GroupBy<SOOrderShipment.invoiceType,
                                                                                                          GroupBy<SOOrderShipment.invoiceNbr>>>.View.Select(Base, shipment.ShipmentType, shipment.ShipmentNbr))
                    {
                        if (string.IsNullOrEmpty(orderShip.InvoiceNbr))
                        {
                            throw new PXException(NoInvoiceData);
                        }

                        invoiceParms[nameof(ARInvoice.DocType)] = orderShip.InvoiceType;
                        invoiceParms[nameof(ARInvoice.RefNbr)]  = orderShip.InvoiceNbr;

                        List<Guid?> attachments = new List<Guid?>();

                        foreach (SOPackageDetailEx detailEx in SelectFrom<SOPackageDetailEx>.Where<SOPackageDetailEx.shipmentNbr.IsEqual<@P.AsString>>
                                                                                            .AggregateTo<GroupBy<SOPackageDetailEx.shipmentNbr>>.View.Select(Base, shipment.ShipmentNbr))
                        {
                            packingParms[nameof(SOShipment.ShipmentNbr)] = shipment.ShipmentNbr;

                            // Report Processing
                            PX.Reports.Controls.Report _report = ReportLoader.LoadReport(reportID, null);

                            ReportLoader.InitDefaultReportParameters(_report, packingParms);

                            ReportNode reportNode = ReportDataBinder.ProcessReportDataBinding(_report);

                            // Generation file
                            byte[] data = PX.Reports.Mail.Message.GenerateReport(reportNode, rptFormat).First();

                            PX.SM.FileInfo file = new PX.SM.FileInfo(reportNode.ExportFileName + fileExt, null, data);

                            UploadFileMaintenance graph = PXGraph.CreateInstance<PX.SM.UploadFileMaintenance>();

                            // Save the file with the setting to create a new version if one already exists based on the UID                          
                            graph.SaveFile(file, FileExistsAction.CreateVersion);

                            attachments.Add(file.UID);
                        }

                        PX.Objects.GL.Branch branch = PXSelectReadonly2<PX.Objects.GL.Branch, InnerJoin<INSite, On<INSite.branchID, Equal<PX.Objects.GL.Branch.branchID>>>,
                                                                                          Where<INSite.siteID, Equal<Optional<SOShipment.destinationSiteID>>,
                                                                                                And<Current<SOShipment.shipmentType>, Equal<SOShipmentType.transfer>,
                                                                                                    Or<INSite.siteID, Equal<Optional<SOShipment.siteID>>,
                                                                                                       And<Current<SOShipment.shipmentType>, NotEqual<SOShipmentType.transfer>>>>>>
                                                                                          .SelectSingleBound(Base, new object[] { shipment });

                        SOInvoiceEntry invoiceEntry = PXGraph.CreateInstance<SOInvoiceEntry>();

                        invoiceEntry.Document.Current = ARInvoice.PK.Find(invoiceEntry, orderShip.InvoiceType, orderShip.InvoiceNbr);

                        invoiceEntry.Activity.SendNotification(PX.Objects.AR.ARNotificationSource.Customer, notificationCD, (branch != null && branch.BranchID != null) ? branch.BranchID : Base.Accessinfo.BranchID, invoiceParms, attachments);
                        //Base.Activity.SendNotification(PX.Objects.AR.ARNotificationSource.Customer, notificationCD, (branch != null && branch.BranchID != null) ? branch.BranchID : Base.Accessinfo.BranchID, invoiceParms, attachments);
                    }

                    Base.Document.Cache.SetValue<SOShipmentExt2.usrSentCarrier>(shipment, true);
                    Base.Document.Cache.Update(shipment);
                }
                catch (PXException e)
                {
                    PXProcessing.SetError<SOShipment>(e);

                    throw;
                }

                yield return shipment;
            }

            Base.Save.Press();
        }
        #endregion
          
        #region Method
        public virtual void VerifyCustomerMailInfo(string reportID)
        {
            var customer = Base.customer.Current;

            if (string.IsNullOrEmpty(SelectFrom<Contact>.Where<Contact.contactID.IsEqual<@P.AsInt>
                                                               .And<Contact.bAccountID.IsEqual<@P.AsInt>>>.View.Select(Base, customer.DefContactID, customer.BAccountID).TopFirst.EMail))
            {
                throw new PXException(NoCustContAddr, Base.customer.Current.AcctCD);
            }
            if (SelectFrom<NotificationSource>.Where<NotificationSource.refNoteID.IsEqual<@P.AsGuid>
                                                     .And<NotificationSource.reportID.IsEqual<@P.AsString>>>.View.Select(Base, customer.NoteID, reportID).Count == 0)
            {
                CreateCRMailingSetting();
                //throw new PXException(NonNotifSrc, Base.customer.Current.AcctCD);
            }
        }
              
        public virtual void CreateCRMailingSetting()
        {
            CustomerMaint maint = PXGraph.CreateInstance<CustomerMaint>();

            foreach (NotificationSetup setup in SelectFrom<NotificationSetup>.Where<NotificationSetup.reportID.IsIn<@P.AsString, @P.AsString>>.View.Select(Base, InvoiceRpt))
            {
                NotificationSource source = maint.NotificationSources.Cache.CreateInstance() as NotificationSource;

                source.SetupID = setup.SetupID;

                source = maint.NotificationSources.Insert(source);

                source.ReportID = setup.ReportID;
                source.Format = setup.Format;
                source.Active = setup.Active;

                maint.NotificationSources.Update(source);
            }

            maint.CurrentCustomer.Current = Base.customer.Current;

            maint.Actions.PressSave();
        }
        #endregion
    }
}