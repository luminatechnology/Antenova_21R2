using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.SO
{
    public class SOShipmentEntry_Extension : PXGraphExtension<SOShipmentEntry>
    {
        #region Constant Class & String Variables
        public const string UPS = "UPS";
        public const string DHL = "DHL";
        public const string FDX = "FEDEX";
        public const string TNT = "TNT";

        private const string _QTYINBOX = "QTYINBOX";
        private const string _MADEIN = "MADEIN";

        public class QtyCartonAttr : PX.Data.BQL.BqlString.Constant<QtyCartonAttr>
        {
            public QtyCartonAttr() : base("QTYCARTON") { }
        }

        #endregion

        protected bool _IsAutoPacking = false;

        public delegate void PersistDelegate();

        public override void Initialize()
        {
            base.Initialize();
            Base.report.AddMenuAction(COCReport);
            Base.report.AddMenuAction(PackingList);
            Base.report.AddMenuAction(TaiwanPackingList);
            Base.report.AddMenuAction(UKPackingList);
            //Base.report.AddMenuAction(SignaturePackingList);
            Base.report.AddMenuAction(GeneralOuterLabel);
            Base.report.AddMenuAction(HanaOuterLabel);
            Base.report.AddMenuAction(AngliaOuterLabel);
            Base.report.AddMenuAction(GlobalEMSOuterLabel);
            Base.report.AddMenuAction(USIOuterLabel);
            Base.report.AddMenuAction(USIInnerLabel);
            Base.report.AddMenuAction(SanminaOuterLabel);
            Base.report.AddMenuAction(SanminaInnerLabel);
            Base.report.AddMenuAction(StandardOuterLabel1);
            Base.report.AddMenuAction(StandardOuterLabel2);
            Base.report.AddMenuAction(StandardOuterLabel3);
            Base.report.AddMenuAction(StandardOuterLabel4);

            Labels.MenuAutoOpen = true;
            Labels.AddMenuAction(BoschOuterLabel);
            Labels.AddMenuAction(BoschInnerLabel);
            Labels.AddMenuAction(WNCOuterLabel);
            Labels.AddMenuAction(WNCInnerLabel);
            Labels.AddMenuAction(TechcomOuterLabel);
            Labels.AddMenuAction(TechcomInnerLabel);
            Labels.AddMenuAction(SystechOuterLabel);
            Labels.AddMenuAction(SystechInnerLabel);
            Labels.AddMenuAction(USITWOuterLabel);
            Labels.AddMenuAction(USITWInnerLabel);
            Labels.AddMenuAction(QisdaOuterLabel);
            Labels.AddMenuAction(QisdaInnerLabel);
            Labels.AddMenuAction(HiflyingOuterLabel);
            Labels.AddMenuAction(AtrackOuterLabel);
            Labels.AddMenuAction(AtrackInnerLabel);
        }

        /// <summary> Override Persist Event </summary>
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            var needUpdatePackedQty = Base.Packages.Cache.Dirty.RowCast<SOPackageDetailEx>().Count() > 0;


            if (needUpdatePackedQty)
            {
                // Except Delete row
                IEnumerable<SOPackageDetailEx> _packages = Base.Packages.Cache.Cached.RowCast<SOPackageDetailEx>();
                _packages = _packages.Except((IEnumerable<SOPackageDetailEx>)Base.Packages.Cache.Deleted);
                var _shipLines = Base.Transactions.Cache.Cached.RowCast<SOShipLine>();
                _shipLines = _shipLines.Except((IEnumerable<SOShipLine>)Base.Transactions.Cache.Deleted);
                // Recalculate PackedQty
                foreach (var item in _shipLines)
                {
                    item.PackedQty = _packages.Where(x => x.GetExtension<SOPackageDetailExt>().UsrShipmentSplitLineNbr == item.LineNbr).Sum(x => x.Qty);
                    Base.Caches[typeof(SOShipLine)].Update(item);
                }
            }

            baseMethod();
        }

        #region Actions

        #region Labels
        public PXAction<SOShipment> Labels;
        [PXUIField(DisplayName = "LABELS", MapEnableRights = PXCacheRights.Select)]
        [PXButton]
        protected void labels() { }
        #endregion

        #region COCReport
        public PXAction<SOShipment> COCReport;
        [PXButton]
        [PXUIField(DisplayName = "Print COC Report", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable cOCReport(PXAdapter adapter)
        {
            var _reportID = "lm601000";
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["ShipmentNbr"] = Base.Document.Current.ShipmentNbr;
                parameters["ShipmentType"] = Base.Document.Current.ShipmentType;
                parameters["_CompanyID"] = PXAccess.GetBranchID().ToString();
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            }
            return adapter.Get();
        }
        #endregion

        #region Packing List - LM642005
        public PXAction<SOShipment> PackingList;
        [PXButton]
        [PXUIField(DisplayName = "Print Packing List", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable packingList(PXAdapter adapter)
        {
            var _reportID = "LM642005";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Taiwan Packing List - LM642006
        public PXAction<SOShipment> TaiwanPackingList;
        [PXButton]
        [PXUIField(DisplayName = "Print Packing List - Taiwan", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable taiwanpackingList(PXAdapter adapter)
        {
            var _reportID = "LM642006";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region UK Packing List - LM642007
        public PXAction<SOShipment> UKPackingList;
        [PXButton]
        [PXUIField(DisplayName = "Print Packing List - UK", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable uKpackingList(PXAdapter adapter)
        {
            var _reportID = "LM642007";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        /*#region Signature Packing List - LM642008
        public PXAction<SOShipment> SignaturePackingList;
        [PXButton]
        [PXUIField(DisplayName = "Print Packing List - Signature", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable signaturepackingList(PXAdapter adapter)
        {
            var _reportID = "LM642008";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion*/

        #region General Outer Label - LM642010
        public PXAction<SOShipment> GeneralOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print General Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable generalOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642010";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Hana Outer Label - LM642011
        public PXAction<SOShipment> HanaOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Hana Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable hanaOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642011";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Anglia Outer Label - LM642012
        public PXAction<SOShipment> AngliaOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Anglia Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable angliaOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642012";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Global EMS Outer Label - LM642013
        public PXAction<SOShipment> GlobalEMSOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Global EMS Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable globalEMSOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642013";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region USI Outer Label - LM642014
        public PXAction<SOShipment> USIOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Report / Print USI Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable uSIOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642014";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each DateCode
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrDateCode == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("DateCode Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Sanmina Outer Label - LM642015
        public PXAction<SOShipment> SanminaOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Sanmina Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable sanminaOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642015";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Standard Outer Label 1 - LM642016
        public PXAction<SOShipment> StandardOuterLabel1;
        [PXButton]
        [PXUIField(DisplayName = "Print Standard Outer Label 1", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable standardOuterLabel1(PXAdapter adapter)
        {
            var _reportID = "LM642016";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Standard Outer Label 2 - LM642017
        public PXAction<SOShipment> StandardOuterLabel2;
        [PXButton]
        [PXUIField(DisplayName = "Print Standard Outer Label 2", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable standardOuterLabel2(PXAdapter adapter)
        {
            var _reportID = "LM642017";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Bosch Outer Label - LM642018
        public PXAction<SOShipment> BoschOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Bosch Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable boschOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642018";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each DateCode
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrDateCode == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("DateCode Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region WNC Outer Label - LM642019
        public PXAction<SOShipment> WNCOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print WNC Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable wNCOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642019";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each OuterBoxOrder
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrOuterBoxOrder == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("Outer Box Order Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Sanmina Inner Label - LM642020
        public PXAction<SOShipment> SanminaInnerLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Sanmina Inner Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable sanminaInnerLabel(PXAdapter adapter)
        {
            var _reportID = "LM642020";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region USI Inner Label - LM642021
        public PXAction<SOShipment> USIInnerLabel;
        [PXButton]
        [PXUIField(DisplayName = "Report / Print USI Inner Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable uSIInnerLabel(PXAdapter adapter)
        {
            var _reportID = "LM642021";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Techcom Outer Label - LM642022
        public PXAction<SOShipment> TechcomOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Techcom Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable techcomOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642022";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each DateCode
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrDateCode == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("DateCode Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Techcom Inner Label - LM642023
        public PXAction<SOShipment> TechcomInnerLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Techcom Inner Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable techcomInnerLabel(PXAdapter adapter)
        {
            var _reportID = "LM642023";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each DateCode
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrDateCode == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("DateCode Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Standard Outer Label 3 - LM642024
        public PXAction<SOShipment> StandardOuterLabel3;
        [PXButton]
        [PXUIField(DisplayName = "Print Standard Outer Label 3", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable standardOuterLabel3(PXAdapter adapter)
        {
            var _reportID = "LM642024";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each DateCode
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrDateCode == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("DateCode Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Standard Outer Label 4 - LM642025
        public PXAction<SOShipment> StandardOuterLabel4;
        [PXButton]
        [PXUIField(DisplayName = "Print Standard Outer Label 4", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable standardOuterLabel4(PXAdapter adapter)
        {
            var _reportID = "LM642025";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each DateCode
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrDateCode == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("DateCode Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Bosch Inner Label - LM642026
        public PXAction<SOShipment> BoschInnerLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Bosch Inner Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable boschInnerLabel(PXAdapter adapter)
        {
            var _reportID = "LM642026";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each DateCode
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrDateCode == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("DateCode Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Systech Outer Label - LM642027
        public PXAction<SOShipment> SystechOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Systech Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable systechOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642027";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Systech Inner Label - LM642028
        public PXAction<SOShipment> SystechInnerLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Systech Inner Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable systechInnerLabel(PXAdapter adapter)
        {
            var _reportID = "LM642028";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region USITW Outer Label - LM642029
        public PXAction<SOShipment> USITWOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print USITW Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable uSITWOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642029";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each DateCode
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrDateCode == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("DateCode Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region USITW Inner Label - LM642030
        public PXAction<SOShipment> USITWInnerLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print USITW Inner Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable uSITWInnerLabel(PXAdapter adapter)
        {
            var _reportID = "LM642030";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each DateCode
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrDateCode == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("DateCode Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Qisda Outer Label - LM642031
        public PXAction<SOShipment> QisdaOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Qisda Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable qisdaOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642031";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each OuterBoxOrder
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrOuterBoxOrder == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("Outer Box Order Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Qisda Inner Label - LM642032
        public PXAction<SOShipment> QisdaInnerLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Qisda Inner Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable qisdaInnerLabel(PXAdapter adapter)
        {
            var _reportID = "LM642032";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each DateCode
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrDateCode == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("DateCode Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Hi-flying Outer Label - LM642033
        public PXAction<SOShipment> HiflyingOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Hi-flying Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable hiflyingOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642033";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region WNC Inner Label - LM642034
        public PXAction<SOShipment> WNCInnerLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print WNC Inner Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable wNCInnerLabel(PXAdapter adapter)
        {
            var _reportID = "LM642034";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            // Checking each DateCode
            if (parameters["ShipmentNbr"] != null)
            {
                bool emptyDateCode = true;
                foreach (SOPackageDetailEx curSOPackageDetailRow in Base.Packages.Cache.Cached)
                {
                    if (curSOPackageDetailRow.GetExtension<SOPackageDetailExt>()?.UsrDateCode == null)
                        emptyDateCode = false;
                }
                if (emptyDateCode)
                    throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
                else
                    throw new PXException("DateCode Can Not Be Null");
            }
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Atrack Outer Label - LM642035
        public PXAction<SOShipment> AtrackOuterLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Atrack Outer Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable atrackOuterLabel(PXAdapter adapter)
        {
            var _reportID = "LM642035";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        #region Atrack Outer Label - LM642036
        public PXAction<SOShipment> AtrackInnerLabel;
        [PXButton]
        [PXUIField(DisplayName = "Print Atrack Inner Label", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable atrackInnerLabel(PXAdapter adapter)
        {
            var _reportID = "LM642036";
            var parameters = new Dictionary<string, string>()
            {
                ["ShipmentNbr"] = (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr
            };
            if (parameters["ShipmentNbr"] != null)
                throw new PXReportRequiredException(parameters, _reportID, string.Format("Report {0}", _reportID));
            return adapter.Get<SOShipment>().ToList();
        }
        #endregion

        ///// <summary> Auto Packaging Button Click Event </summary>
        public PXAction<SOShipment> autoPackaging;
        [PXButton]
        [PXUIField(DisplayName = "Auto Packaging", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable AutoPackaging(PXAdapter adapter)
        {
            decimal parseResult = 0;
            var _maxCartno = GetMaxCartonNbr();
            SOShipLine _line = (SOShipLine)Base.Transactions.Cache.Current;
            var _QtyPerCarton = decimal.TryParse(_line.GetExtension<SOShipLineExt>().QtyPerCarton, out parseResult) ? parseResult : 0;
            // valid QtyPerCarton
            if (_QtyPerCarton == 0)
                throw new PXException("Qty per carton Can not be null or 0");

            var NumberOfPackages = Math.Floor((_line.GetExtension<SOShipLineExt>().RemainingQty.Value / _QtyPerCarton));

            PXLongOperation.StartOperation(Base, () =>
            {
                for (int i = 0; i < NumberOfPackages; i++)
                {
                    this._IsAutoPacking = true;
                    Base.Packages.Insert((SOPackageDetailEx)Base.Packages.Cache.CreateInstance());
                    SOPackageDetailEx _package = Base.Packages.Cache.Dirty.RowCast<SOPackageDetailEx>().ElementAt(i);
                    Base.Packages.Cache.SetValueExt<SOPackageDetail.shipmentNbr>(_package, _line.ShipmentNbr);
                    Base.Packages.Cache.SetValueExt<SOPackageDetailEx.customRefNbr1>(_package, (++_maxCartno).ToString().PadLeft(3, '0'));
                    Base.Packages.Cache.SetValueExt<SOPackageDetailExt.usrShipmentSplitLineNbr>(_package, _line.LineNbr);
                    Base.Packages.Cache.SetValueExt<SOPackageDetail.qty>(_package, _QtyPerCarton);
                }
                Base.Save.Press();
            });
            return adapter.Get();
        }

        /// <summary> Maunal Packaging Button Click Event </summary>
        public PXAction<SOShipment> manualPackaging;
        [PXButton]
        [PXUIField(DisplayName = "Maunal Packaging", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable ManualPackaging(PXAdapter adapter)
        {
            var _shipLines = Base.Transactions.Cache.Cached.RowCast<SOShipLine>();
            if (_shipLines.Where(x => x.GetExtension<SOShipLineExt>().RemainingQty < x.GetExtension<SOShipLineExt>().UsrPackingQty).Count() > 0)
                throw new PXException("Packing Qty cannot exceed Remaining Qty");
            PXLongOperation.StartOperation(Base, () =>
            {
                int pointer = 0;
                var _maxCartonNbr = GetMaxCartonNbr() + 1;
                foreach (var _line in _shipLines.Where(x => x.GetExtension<SOShipLineExt>().UsrPackingQty > 0))
                {
                    this._IsAutoPacking = true;
                    Base.Packages.Insert((SOPackageDetailEx)Base.Packages.Cache.CreateInstance());
                    SOPackageDetailEx _package = Base.Packages.Cache.Dirty.RowCast<SOPackageDetailEx>().ElementAt(pointer++);
                    Base.Packages.Cache.SetValueExt<SOPackageDetail.shipmentNbr>(_package, _line.ShipmentNbr);
                    Base.Packages.Cache.SetValueExt<SOPackageDetailEx.customRefNbr1>(_package, _maxCartonNbr.ToString().PadLeft(3, '0'));
                    Base.Packages.Cache.SetValueExt<SOPackageDetailExt.usrShipmentSplitLineNbr>(_package, _line.LineNbr);
                    Base.Packages.Cache.SetValueExt<SOPackageDetail.qty>(_package, _line.GetExtension<SOShipLineExt>().UsrPackingQty);
                }
                Base.Save.Press();
            });
            return adapter.Get();
        }


        #endregion

        #region Event Handlers
        protected virtual void _(Events.RowSelected<SOShipment> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            SOShipment row = (SOShipment)e.Row;

            if (row == null) return;

            e.Cache.AllowUpdate = true;
            PXUIFieldAttribute.SetEnabled<SOShipmentExt.usrCarrierPluginID>(e.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<SOShipmentExt.usrWaybill>(e.Cache, null, true);
        }

        protected void _(Events.FieldSelecting<SOShipmentExt.usrNote> e)
        {
            SOShipment row = e.Row as SOShipment;
            string str = string.Empty;
            if (row != null)
            {
                foreach (PXResult<Note> pxResult1 in SelectFrom<Note>.InnerJoin<SOOrder>.On<SOOrder.noteID.IsEqual<Note.noteID>>
                                                                         .InnerJoin<SOOrderShipment>.On<SOOrderShipment.orderType.IsEqual<SOOrder.orderType>
                                                                                                        .And<SOOrderShipment.orderNbr.IsEqual<SOOrder.orderNbr>>>
                                                                         .Where<SOOrderShipment.shipmentNbr.IsEqual<P.AsString>>.View.ReadOnly.Select((PXGraph)this.Base, (object)row.ShipmentNbr))
                {

                    Note note = (Note)pxResult1;
                    if (note.NoteText.Length > 0)
                        str += note.NoteText + "\n--------------------\n";
                }
            }
            e.ReturnValue = (object)str;
        }

        /// <summary> Set Date Code DDL </summary>
        protected void _(Events.RowSelected<SOPackageDetailEx>e, PXRowSelected baseMethod)
        {
            baseMethod?.Invoke(e.Cache, e.Args);
            if (e.Row != null)
            {
                var splitData = SelectFrom<SOShipLineSplit>
                                .Where<SOShipLineSplit.shipmentNbr.IsEqual<P.AsString>
                                    .And<SOShipLineSplit.lineNbr.IsEqual<P.AsInt>>>
                                .View.Select(Base, e.Row.ShipmentNbr, e.Row.GetExtension<SOPackageDetailExt>().UsrShipmentSplitLineNbr).RowCast<SOShipLineSplit>(); ;
                PXStringListAttribute.SetList<SOPackageDetailExt.usrDateCode>(
                    e.Cache,
                    e.Row,
                    splitData.Select(x => x?.LotSerialNbr).ToArray(),
                    splitData.Select(x => x?.LotSerialNbr).ToArray());
                PXStringListAttribute.SetList<SOPackageDetailExt.usrDateCode4LastBox>(
                    e.Cache,
                    e.Row,
                    splitData.Select(x => x?.LotSerialNbr).ToArray(),
                    splitData.Select(x => x?.LotSerialNbr).ToArray());
                PXStringListAttribute.SetList<SOPackageDetailExt.usrDateCode4ThisBox>(
                    e.Cache,
                    e.Row,
                    splitData.Select(x => x?.LotSerialNbr).ToArray(),
                    splitData.Select(x => x?.LotSerialNbr).ToArray());
            }
        }

        /// <summary> Get Country DDL </summary>
        protected void _(Events.FieldSelecting<SOPackageDetailExt.usrCountry> e)
        {
            if (e.Row != null)
            {
                var ddl = new PXGraph().Select<CSAttributeDetail>()
                                                .Where(x => x.AttributeID == "MADEIN");
                PXStringListAttribute.SetList<SOPackageDetailExt.usrCountry>(
                    e.Cache,
                    e.Row,
                    ddl.Select(x => x.ValueID).ToArray(),
                    ddl.Select(x => x.Description).ToArray());
            }
        }

        /// <summary> SOPackageDetailExt_usrShipmentSplitLineNbr Updated Event </summary>
        protected virtual void _(Events.FieldUpdated<SOPackageDetailExt.usrShipmentSplitLineNbr> e)
        {
            if (e.NewValue == null)
                return;
            var _shipLine = Base.Transactions.Cache.Cached.RowCast<SOShipLine>().Where(x => x.LineNbr == (int?)e.NewValue).SingleOrDefault();
            var _stockItemInfo = GetStockInfo(_shipLine.InventoryID.Value, _MADEIN);
            var _shipLineSplit = new PXGraph().Select<SOShipLineSplit>().Where(x => x.ShipmentNbr == _shipLine.ShipmentNbr && x.LineNbr == _shipLine.LineNbr);
            var boxsInfo = GetBoxsInfo(_shipLine.InventoryID);
            e.Cache.SetValueExt<SOPackageDetailEx.boxID>(e.Row, string.IsNullOrEmpty(boxsInfo.stockItemBox) ? boxsInfo.sBoxID : boxsInfo.stockItemBox);
            e.Cache.SetValueExt<SOPackageDetail.inventoryID>(e.Row, _shipLine.InventoryID);
            e.Cache.SetValueExt<SOPackageDetailExt.usrCountry>(e.Row, _stockItemInfo.GetItem<CSAnswers>()?.Value);
            if (!this._IsAutoPacking)
                e.Cache.SetValueExt<SOPackageDetail.qty>(e.Row, _shipLine.ShippedQty);
            // if ShipLineSplit count == 1 then set value
            if (_shipLineSplit.Count() == 1)
                e.Cache.SetValueExt<SOPackageDetailExt.usrDateCode>(e.Row, _shipLineSplit.FirstOrDefault()?.LotSerialNbr);
        }

        /// <summary> SOPackageDetailEx_qty Updated Event </summary>
        protected void _(Events.FieldUpdated<SOPackageDetail.qty> e)
        {
            var _splitLineNbr = e.Cache.GetExtension<SOPackageDetailExt>(e.Row).UsrShipmentSplitLineNbr;
            if (e.NewValue == null || _splitLineNbr == null)
                return;

            if ((decimal?)e.NewValue == 0)
            {
                e.Cache.SetValueExt<SOPackageDetail.weight>(e.Row, 0);
                return;
            }

            var row = (SOPackageDetailEx)e.Row;

            var _shipLine = Base.Transactions.Cache.Cached.RowCast<SOShipLine>().Where(x => x.LineNbr == _splitLineNbr).SingleOrDefault();
            PXResult<InventoryItem, CSAnswers> _sotockItemData = GetStockInfo(_shipLine.InventoryID.Value, _QTYINBOX);
            decimal _qtyInBox = 1;
            try
            {
                _qtyInBox = decimal.Parse(_sotockItemData.GetItem<CSAnswers>()?.Value);
            }
            catch (Exception)
            {
                _qtyInBox = 1;
            }

            e.Cache.SetValueExt<SOPackageDetail.weight>(e.Row, row.Qty * _sotockItemData.GetItem<InventoryItem>().BaseItemWeight / _qtyInBox);
        }

        /// <summary> SOPackageDetailEx_weight Updated Event</summary>
        protected void _(Events.FieldUpdated<SOPackageDetail.weight> e)
        {
            if (e.NewValue != null)
            {
                var row = (SOPackageDetailEx)e.Row;
                var _boxWeight = new PXGraph().Select<CSBox>().Where(x => x.BoxID == row.BoxID)?.FirstOrDefault()?.BoxWeight ?? 0;
                e.Cache.SetValueExt<SOPackageDetailExt.usrGrossWeight>(e.Row, (decimal)e.NewValue + _boxWeight);
            }
        }

        /// <summary> SOPackageDetailEx_boxID Updated Event</summary>
        protected void _(Events.FieldUpdated<SOPackageDetail.boxID> e)
        {
            if (e.NewValue == null)
                return;
            var _boxWeight = new PXGraph().Select<CSBox>().Where(x => x.BoxID == (string)e.NewValue)?.FirstOrDefault()?.BoxWeight ?? 0;
            e.Cache.SetValueExt<SOPackageDetailExt.usrGrossWeight>(e.Row, ((SOPackageDetailEx)e.Row).Weight + _boxWeight);
        }

        /// <summary> Verify Carton Nbr Is Numerical </summary>
        protected void _(Events.FieldVerifying<SOPackageDetail.customRefNbr1> e)
        {
            int result = 0;
            if (!int.TryParse(e.NewValue.ToString(), out result))
                throw new PXSetPropertyException<SOPackageDetail.customRefNbr1>("Carton Nbr must be numerical.");
        }

        /// <summary> Verify Packing Qty </summary>
        protected void _(Events.FieldVerifying<SOShipLineExt.usrPackingQty> e)
        {
            if ((decimal?)e.NewValue < 0)
                throw new PXSetPropertyException("Packing Qty must br greater than 0");
            if ((decimal?)e.NewValue > ((SOShipLine)e.Row).GetExtension<SOShipLineExt>().RemainingQty)
                throw new PXSetPropertyException("Packing Qty cannot exceed Remaining Qty");
        }

        /// <summary> SOPackageDetailEx_BoxID Defaulting </summary>
        protected void _(Events.FieldDefaulting<SOPackageDetailEx.boxID> e)
        {
            var row = (SOPackageDetailEx)e.Row;
            var boxsInfo = GetBoxsInfo(row.InventoryID);
            e.NewValue = string.IsNullOrEmpty(boxsInfo.stockItemBox) ? boxsInfo.sBoxID : boxsInfo.stockItemBox;
        }

        #endregion

        #region Method

        /// <summary> Get Max Carton Nbr </summary>
        public int GetMaxCartonNbr()
        {
            int result;
            var _PackageDetail = Base.Caches<SOPackageDetailEx>().Cached.RowCast<SOPackageDetailEx>();
            try
            {
                return _PackageDetail.Any() ? _PackageDetail.Where(x => !string.IsNullOrEmpty(x.CustomRefNbr1) && int.TryParse(x.CustomRefNbr1, out result)).Max(x => int.Parse(x.CustomRefNbr1)) : 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary> Get StockItem Info </summary>
        public PXResult<InventoryItem, CSAnswers> GetStockInfo(int InventoryID, string AttributeID)
        {
            return (PXResult<InventoryItem, CSAnswers>)SelectFrom<InventoryItem>
                                                      .LeftJoin<CSAnswers>.On<InventoryItem.noteID.IsEqual<CSAnswers.refNoteID>
                                                           .And<CSAnswers.attributeID.IsEqual<P.AsString>>>
                                                      .Where<InventoryItem.inventoryID.IsEqual<P.AsInt>>
                                                      .View.Select(Base, AttributeID, InventoryID);
        }

        /// <summary> Get sBoxID and stockItemBox </summary>
        public (string sBoxID, string stockItemBox) GetBoxsInfo(int? InventoryID)
        {
            var _sboxID = new PXGraph().Select<CSBox>().FirstOrDefault()?.BoxID;
            var _stockItemBoxID = new PXGraph().Select<INItemBoxEx>().Where(x => x.InventoryID == InventoryID).FirstOrDefault()?.BoxID;
            return (_sboxID, _stockItemBoxID);
        }
        #endregion

        #region Static Method
        public static string GetShipWaybillURL(string carrier, string wayBill)
        {
            string url = null;

            carrier = string.IsNullOrEmpty(carrier) ? string.Empty : carrier.ToUpper();

            switch (carrier)
            {
                case UPS:
                    url = $"https://www.ups.com/track?loc=en_tw&tracknum={wayBill.Trim()}&requester=WT/trackdetails";
                    break;
                case DHL:
                    url = $"https://mydhl.express.dhl/tw/en/tracking.html#/results?id={wayBill.Trim()}";
                    break;
                case FDX:
                    url = $"https://www.fedex.com/fedextrack/?trknbr={wayBill.Trim()}";
                    break;
                case TNT:
                    url = $" https://www.tnt.com/express/en_gc/site/shipping-tools/track.html?searchType=con&cons={wayBill.Trim()}";
                    break;
            }

            return url;
        }
        #endregion
    }
}