using System;
using System.Collections;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;

namespace SalesForecastANTENOVA
{
    public class SalesFcstInquiryEntry : PXGraph<SalesFcstInquiryEntry>
    {
        //public PXFilter<SalesFcstInquiry> FilterView;
        public PXSelectReadonly<SalesFcstInquiry> SOFcstInq;
        public PXSelect<SalesForecast> SOForecast;
        public PXSelectJoin<SOLine,
                            InnerJoin<SOOrder, On<SOOrder.orderNbr, Equal<SOLine.orderNbr>,
                                                  And<SOLine.orderType, Equal<SOOrder.orderType>>>,
                                      InnerJoin<Customer, On<Customer.bAccountID, Equal<SOOrder.customerID>>,
                                                InnerJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<SOLine.inventoryID>>>>>,
                            Where<SOLine.openQty, Greater<Zero>>> Backlog;
      
        public class Zero : Constant<string>
        {
            public Zero() : base("0") { }
        }

        #region Delegate Data View
        public IEnumerable sOFcstInq()
        {
            Boolean found = false;

            //Checking cache for existing items
            foreach (SalesFcstInquiry row in SOFcstInq.Cache.Cached)
            {
                found = true;
                yield return row;
            }

            if (!found) //if not found - Select from DB
            {
                InsertRecords();

                //Checking cache for existing items
                foreach (SalesFcstInquiry row in SOFcstInq.Cache.Cached)
                {
                    yield return row;
                }
            }

            SOFcstInq.Cache.IsDirty = false;
        }
        #endregion

        #region Action
        //public PXAction<SalesFcstInquiry> generateRecords;
        //[PXUIField(DisplayName = "Generate Records")]
        //[PXButton(CommitChanges = true)]
        //public virtual IEnumerable GenerateRecords(PXAdapter adapter)
        //{
        //    PXLongOperation.StartOperation(this, delegate
        //    {
        //        InsertRecords();
        //    });

        //    return adapter.Get();
        //}
        #endregion

        #region Function
        private void InsertRecords()
        {
            int count = 1;

            SalesFcstInquiry fcstInquiry = SOFcstInq.Cache.CreateInstance() as SalesFcstInquiry;

            foreach (PXResult<SalesForecast> result in SOForecast.Select())
            {
                SalesForecast salesForecast = result.GetItem<SalesForecast>();

                fcstInquiry.Numbering     = count;
                fcstInquiry.SalesPersonID = salesForecast.SalesPersonID;
                fcstInquiry.CustomerID    = salesForecast.CustomerID;
                fcstInquiry.EndCustomer   = salesForecast.EndCustomer;
                fcstInquiry.InventoryID   = salesForecast.InventoryID;
                fcstInquiry.FinPeriodID   = salesForecast.FinPeriodID;
                fcstInquiry.CountryID     = salesForecast.CountryID;
                fcstInquiry.IsTotal       = salesForecast.IsTotal;
                fcstInquiry.IsSplit       = salesForecast.IsSplit;
                fcstInquiry.PercentSplit  = salesForecast.PercentSplit;
                fcstInquiry.Date          = salesForecast.Date;
                fcstInquiry.Qty           = salesForecast.Qty;
                fcstInquiry.UnitPrice     = salesForecast.UnitPrice;
                fcstInquiry.ItemClassID   = salesForecast.ItemClassID;

                count++;
                SOFcstInq.Cache.Insert(fcstInquiry);
            }

            foreach (PXResult<SOLine, SOOrder, Customer, InventoryItem> result2 in Backlog.Select())
            {
                SOOrder order = result2.GetItem<SOOrder>();
                SOLine  line  = result2.GetItem<SOLine>();

                fcstInquiry.Numbering        = count;
                fcstInquiry.OrderNbr         = order.OrderNbr;
                fcstInquiry.OrderDate        = line.OrderDate;
                fcstInquiry.RequestDate      = line.RequestDate;
                fcstInquiry.ShipDate         = line.ShipDate;
                fcstInquiry.InventoryID      = line.InventoryID;
                fcstInquiry.SiteID           = line.SiteID;
                fcstInquiry.OrderQty         = line.OrderQty;
                fcstInquiry.OpenQty          = line.OpenQty;
                fcstInquiry.CustomerID       = line.CustomerID;
                fcstInquiry.LineNbr          = line.LineNbr;
                fcstInquiry.UnitPrice        = line.UnitPrice;
                fcstInquiry.Amount           = line.OpenAmt;
                fcstInquiry.SalesPersonID    = line.SalesPersonID;
                fcstInquiry.FinPeriodID      = String.Format("{0}{1}", line.ShipDate.Value.Year, line.ShipDate.Value.Month.ToString("00") );
                fcstInquiry.EndCustomer      = PXCache<SOLine>.GetExtension<SOLineExt>(line).UsrEndCustomer;
                fcstInquiry.ItemClassID      = result2.GetItem<InventoryItem>().ItemClassID;
                fcstInquiry.CustomerOrderNbr = order.CustomerOrderNbr;
                fcstInquiry.Status           = order.Status;
                fcstInquiry.TermsID          = result2.GetItem<Customer>().TermsID;

                count++;
                SOFcstInq.Cache.Insert(fcstInquiry);
            }
        }
        #endregion

        #region Virtual Table DAC
        [Serializable]
        [PXVirtual]
        public class SalesFcstInquiry : IBqlTable
        {
            #region Numbering
            [PXDBInt(IsKey = true)]
            [PXUIField(DisplayName = "Row Nbr.")]
            public virtual int? Numbering { get; set; }
            public abstract class numbering : IBqlField { }
            #endregion

            #region SalesPersonID
            [SalesPerson()]
            public virtual int? SalesPersonID { get; set; }
            public abstract class salesPersonID : IBqlField { }
            #endregion

            #region CustomerID
            [CustomerActive(typeof(Search<BAccountR.bAccountID>),
                            Visibility = PXUIVisibility.SelectorVisible,
                            DescriptionField = typeof(Customer.acctName),
                            Filterable = true)]
            public virtual int? CustomerID { get; set; }
            public abstract class customerID : IBqlField { }
            #endregion

            #region CountryID
            [Country()]
            [PXFormula(typeof(Default<customerID>))]
            public virtual string CountryID { get; set; }
            public abstract class countryID : IBqlField { }
            #endregion

            #region SalesRegion
            [PXDBString(50, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Sales Region")]
            [PXDefault(typeof(Search2<CSAnswers.value,
                                      LeftJoin<Customer, On<Customer.noteID, Equal<CSAnswers.refNoteID>>>,
                                      Where<CSAnswers.attributeID, Equal<SalesForecast.SalesrRgnIDAtt>,
                                            And<Customer.bAccountID, Equal<Current<SalesFcstInquiry.customerID>>>>>),
                       PersistingCheck = PXPersistingCheck.Nothing)]
            [PXFormula(typeof(Default<customerID>))]
            public virtual string SalesRegion { get; set; }
            public abstract class salesRegion : IBqlField { }
            #endregion

            #region EndCustomer
            [PXDBString(50, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "End Customer")]
            public virtual string EndCustomer { get; set; }
            public abstract class endCustomer : IBqlField { }
            #endregion

            #region IsTotal
            [PXDBBool()]
            [PXUIField(DisplayName = "Is Total", Visibility = PXUIVisibility.Visible)]
            public virtual bool? IsTotal { get; set; }
            public abstract class isTotal : IBqlField { }
            #endregion

            #region IsSplit
            [PXDBBool()]
            [PXUIField(DisplayName = "Is Split", Visibility = PXUIVisibility.Visible)]
            public virtual bool? IsSplit { get; set; }
            public abstract class isSplit : IBqlField { }
            #endregion

            #region PercentSplit
            [PXDBDecimal()]
            [PXUIField(DisplayName = "% Split")]
            public virtual Decimal? PercentSplit { get; set; }
            public abstract class percentSplit : IBqlField { }
            #endregion

            #region InventoryID     
            [SOLineInventoryItem(Filterable = true)]
            [PXUIField(DisplayName = "Part Number")]
            public virtual int? InventoryID { get; set; }
            public abstract class inventoryID : IBqlField { }
            #endregion

            #region ItemClassID
            [PXDBInt()]
            [PXUIField(DisplayName = "Item Class", Visibility = PXUIVisibility.SelectorVisible)]
            [PXDimensionSelector(INItemClass.Dimension,
                                 typeof(Search<INItemClass.itemClassID>),
                                 typeof(INItemClass.itemClassCD),
                                 DescriptionField = typeof(INItemClass.descr))]
            public virtual int? ItemClassID { get; set; }
            public abstract class itemClassID : IBqlField { }
            #endregion

            #region FinPeriodID
            //[FinPeriodNonLockedSelector()]
            [PXDBString()]
            [PXUIField(DisplayName = "Fin Period")]
            public virtual string FinPeriodID { get; set; }
            public abstract class finPeriodID : IBqlField { }
            #endregion

            #region Date
            [PXDBDate()]
            public virtual DateTime? Date { get; set; }
            public abstract class date : IBqlField { }
            #endregion

            #region Qty
            [PXDBQuantity()]
            [PXUIField(DisplayName = "Qty")]
            public virtual Decimal? Qty { get; set; }
            public abstract class qty : IBqlField { }
            #endregion

            #region UnitPrice
            [PXDBCurrency(typeof(Search<CommonSetup.decPlPrcCst>), typeof(AccessInfo.baseCuryID), typeof(SalesFcstInquiry.unitPrice))]
            [PXUIField(DisplayName = "Unit Price")]
            public virtual Decimal? UnitPrice { get; set; }
            public abstract class unitPrice : IBqlField { }
            #endregion

            #region Amount
            [PXDBDecimal()]
            [PXUIField(DisplayName = "Amount")]
            [PXFormula(typeof(Mult<qty, unitPrice>))]
            public virtual Decimal? Amount { get; set; }
            public abstract class amount : IBqlField { }
            #endregion

            #region OrderNbr     
            [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
            [PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.SelectorVisible)]
            public virtual String OrderNbr { get; set; }
            public abstract class orderNbr : IBqlField { }
            #endregion

            #region OrderDate
            [PXDBDate(BqlField = typeof(SOLine.orderDate))]
            public virtual DateTime? OrderDate { get; set; }
            public abstract class orderDate : IBqlField { }
            #endregion

            #region RequestDate
            [PXDBDate(BqlField = typeof(SOLine.requestDate))]
            public virtual DateTime? RequestDate { get; set; }
            public abstract class requestDate : IBqlField { }
            #endregion

            #region ShipDate
            [PXDBDate(BqlField = typeof(SOLine.shipDate))]
            [PXUIField(DisplayName = "Promised Date")]
            public virtual DateTime? ShipDate { get; set; }
            public abstract class shipDate : IBqlField { }
            #endregion

            #region SiteID       
            //[SiteAvail(typeof(SOLine.inventoryID), typeof(SOLine.subItemID))]
            //[PXUIRequired(typeof(IIf<Where<SOLine.lineType, NotEqual<SOLineType.miscCharge>>, True, False>))]
            [PXDBInt()]
            public virtual Int32? SiteID { get; set; }
            public abstract class siteID : IBqlField { }
            #endregion

            #region OrderQty
            [PXDBQuantity()]
            [PXUIField(DisplayName = "Order Qty")]
            public virtual Decimal? OrderQty { get; set; }
            public abstract class orderQty : IBqlField { }
            #endregion

            #region OpenQty
            [PXDBQuantity()]
            [PXUIField(DisplayName = "Open Qty")]
            public virtual Decimal? OpenQty { get; set; }
            public abstract class openQty : IBqlField { }
            #endregion

            #region LineNbr      
            [PXDBInt(BqlField = typeof(SOLine.lineNbr))]
            [PXUIField(DisplayName = "Line Nbr.")]
            public virtual Int32? LineNbr { get; set; }
            public abstract class lineNbr : IBqlField { }
            #endregion

            #region CustomerOrderNbr
            [PXDBString(40, IsUnicode = true)]
            [PXUIField(DisplayName = "Customer Order", Visibility = PXUIVisibility.SelectorVisible)]
            public virtual String CustomerOrderNbr { get; set; }
            public abstract class customerOrderNbr : IBqlField { }
            #endregion

            #region Status
            protected string _Status;
            [PXDBString(1, IsFixed = true)]
            [PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible)]
            [SOOrderStatus.List()]
            public virtual String Status { get; set; }
            public abstract class status : IBqlField { }
            #endregion

            #region TermsID
            [PXDBString(10, IsUnicode = true)]
            [PXUIField(DisplayName = "Terms")]
            public virtual String TermsID { get; set; }
            public abstract class termsID : IBqlField { }
            #endregion
        }
        #endregion
    }
}