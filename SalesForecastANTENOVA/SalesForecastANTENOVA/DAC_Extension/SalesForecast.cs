using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Data.ReferentialIntegrity.Attributes;

namespace SalesForecastANTENOVA
{
    [Serializable]
    public partial class SalesForecast : IBqlTable
    {
        #region SalesPersonID
        [SalesPerson(IsKey = true)]
        [PXUIField(DisplayName = "Salesperson")]
        public virtual int? SalesPersonID { get; set; }
        public abstract class salesPersonID : IBqlField { }
        #endregion

        #region CustomerID
        [CustomerActive(typeof(Search<BAccountR.bAccountID>),
                        Visibility = PXUIVisibility.SelectorVisible,
                        DescriptionField = typeof(Customer.acctName),
                        Filterable = true,
                        IsKey = true)]
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXUIField(DisplayName = "CustomerID")]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : IBqlField { }
        #endregion

        #region CountryID
        [PXDBString(2)]
        [PXUIField(Enabled = true)]
        [Country()]
        [PXDefault(typeof(Search2<Address.countryID,
                                  InnerJoinSingleTable<BAccount, On<BAccount.defAddressID, Equal<Address.addressID>,
                                                       And<BAccount.bAccountID, Equal<Address.bAccountID>>>>,
                                  Where<BAccount.bAccountID, Equal<Current<SalesForecast.customerID>>>>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<SalesForecast.customerID>))]
        public virtual string CountryID { get; set; }
        public abstract class countryID : IBqlField { }
        #endregion

        #region SalesRegion
        public const string SalesrRgnID = "REGION";
        public class SalesrRgnIDAtt : PX.Data.BQL.BqlString.Constant<SalesrRgnIDAtt>
        {
            public SalesrRgnIDAtt() : base(SalesrRgnID) { }
        }
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Sales Region", Enabled = true)]
        [PXDefault(typeof(Search2<CSAnswers.value,
                          LeftJoin<Customer, On<Customer.noteID, Equal<CSAnswers.refNoteID>>>,
                          Where<CSAnswers.attributeID, Equal<SalesrRgnIDAtt>,
                                And<Customer.bAccountID, Equal<Current<SalesForecast.customerID>>>>>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<SalesForecast.customerID>))]
        public virtual string SalesRegion { get; set; }
        public abstract class salesRegion : IBqlField { }
        #endregion
    
        #region EndCustomer
        [PXDBString(50, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "End Customer")]
        [PXSelector(typeof(Search<ShippingZone.zoneID>),
                    DescriptionField = typeof(ShippingZone.description))]
        public virtual string EndCustomer { get; set; }
        public abstract class endCustomer : IBqlField { }
        #endregion
          
        #region IsTotal
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Is Total", Visibility = PXUIVisibility.Visible)]
        public virtual bool? IsTotal { get; set; }
        public abstract class isTotal : IBqlField { }
        #endregion
          
        #region IsSplit
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
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
        [SOLineInventoryItem(Filterable = true, IsKey = true)]
        [PXUIField(DisplayName = "Part Number")]       
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : IBqlField { }
        #endregion

        #region ItemClassID
        [PXDBInt()]//BqlField = typeof(InventoryItem.itemClassID))]
        [PXUIField(DisplayName = "Item Class", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [PXDimensionSelector(INItemClass.Dimension, typeof(Search<INItemClass.itemClassID>), typeof(INItemClass.itemClassCD), 
                             DescriptionField = typeof(INItemClass.descr))]
        [PXDefault(typeof(Search<InventoryItem.itemClassID,
                                 Where<InventoryItem.inventoryID, Equal<Current<SalesForecast.inventoryID>>>>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<SalesForecast.inventoryID>))]
        public virtual int? ItemClassID { get; set; }
        public abstract class itemClassID : IBqlField { }
        #endregion

        #region FinPeriodID
        [FinPeriodSelector(IsKey = true)]
        //[SOFinPeriod(IsKey = true)]
        //[FinPeriodNonLockedSelector(IsKey = true)]
        [PXUIField(DisplayName = "Fin Period")] 
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : IBqlField { }
        #endregion

        #region Date
        [PXDBDate()]
        [PXUIField(DisplayName = "Trans Date")]
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
        [PXDBCurrency(typeof(Search<CommonSetup.decPlPrcCst>), 
                      typeof(AccessInfo.baseCuryID), 
                      typeof(SalesForecast.unitPrice))]
        [PXUIField(DisplayName = "Unit Price")]
        public virtual Decimal? UnitPrice { get; set; }
        public abstract class unitPrice : IBqlField{ }
        #endregion
    
        #region Amount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "LineAmount", Enabled = false)]
        [PXFormula(typeof(Mult<SalesForecast.qty, SalesForecast.unitPrice>))]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : IBqlField { }
        #endregion
    
        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : IBqlField { }
        #endregion
    
        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : IBqlField { }
        #endregion
    
        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
        #endregion
          
        #region DesignSalesID
        [SalesPerson(DisplayName = "Design Sales ID")]
        [PXForeignReference(typeof(Field<SalesForecast.designSalesID>.IsRelatedTo<SalesPerson.salesPersonID>))]
        public virtual int? DesignSalesID { get; set;}
        public abstract class designSalesID : IBqlField {}
        #endregion
          
        #region OpportunityID
        [PXDBString(10, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Opportunity Nbr.")]
        public virtual String OpportunityID { get; set; }
        public abstract class opportunityID : IBqlField  { } 
        #endregion
    }
}