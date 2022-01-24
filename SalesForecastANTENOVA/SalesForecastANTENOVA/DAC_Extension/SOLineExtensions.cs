using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.Common.Bql;
using PX.Objects.Common.Discount.Attributes;
using PX.Objects.Common.Discount;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN.Matrix.Interfaces;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System.Collections;
using System;

namespace PX.Objects.SO
{
    public class SOLineExt : PXCacheExtension<PX.Objects.SO.SOLine>
    {
        #region UsrEndCustomer
        [PXDBString(15, IsUnicode = true, InputMask = ">aaaaaaaaaaaaaaa")]
        [PXUIField(DisplayName = "Shipping Zone")]
        [PXSelector(typeof(ShippingZone.zoneID), CacheGlobal = true)]
        [PXDefault(typeof(Search<Location.cShipZoneID, Where<Location.bAccountID, Equal<Current<SOOrder.customerID>>, And<Location.locationID, Equal<Current<SOOrder.customerLocationID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrEndCustomer { get; set; }
        public abstract class usrEndCustomer : PX.Data.BQL.BqlString.Field<usrEndCustomer> { }
        #endregion

        #region UsrDesignSalesID
        [SalesPerson(DisplayName = "Design Sales ID")]
        [PXParent(typeof(Select<SOSalesPerTran,
                        Where<SOSalesPerTran.orderType, Equal<Current<SOLine.orderType>>,
                              And<SOSalesPerTran.orderNbr, Equal<Current<SOLine.orderNbr>>,
                                  And<SOSalesPerTran.salespersonID, Equal<Current2<SOLine.salesPersonID>>>>>>),
          LeaveChildren = true,
          ParentCreate = true)]
        [PXDefault()]
        [PXForeignReference(typeof(Field<SOLine.salesPersonID>.IsRelatedTo<SalesPerson.salesPersonID>))]
        public virtual int? UsrDesignSalesID { get; set; }
        public abstract class usrDesignSalesID : PX.Data.BQL.BqlInt.Field<usrDesignSalesID> { }
        #endregion

        #region UsrOpportunityID
        [PXDBString(10, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Opportunity Nbr.",Enabled = false)]
        public virtual string UsrOpportunityID { get; set; }
        public abstract class usrOpportunityID : PX.Data.BQL.BqlString.Field<usrOpportunityID> { }
        #endregion

        #region UsrNewPO
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "New PO")]
        public virtual bool? UsrNewPO { get; set; }
        public abstract class usrNewPO : PX.Data.BQL.BqlBool.Field<usrNewPO> { }
        #endregion
    }
}