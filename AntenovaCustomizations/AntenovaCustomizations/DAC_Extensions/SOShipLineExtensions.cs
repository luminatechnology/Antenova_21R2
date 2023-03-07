﻿using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Objects.SO.SOShipmentEntry_Extension;

namespace PX.Objects.SO
{
    public class SOShipLineExt : PXCacheExtension<SOShipLine>
    {
        #region Unbound Fields

        #region RemainingQty
        [PXDecimal]
        [PXDefault(TypeCode.Decimal, "0.0",PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Packing Qty")]
        public virtual decimal? UsrPackingQty { get; set; }
        public abstract class usrPackingQty : BqlDecimal.Field<usrPackingQty> { }

        [PXDecimal]
        [PXUIField(DisplayName = "Remaining Qty", Enabled = false)]
        [PXFormula(typeof(Sub<SOShipLine.shippedQty, SOShipLine.packedQty>))]
        public virtual decimal? RemainingQty { get; set; }
        public abstract class remainingQty : BqlDecimal.Field<remainingQty> { }
        #endregion

        #region QtyPerCarton
        [PXString]
        [PXUIField(DisplayName = "Qty Per Carton", Enabled = false)]
        [PXDefault(typeof(SelectFrom<CSAnswers>.
                          LeftJoin<InventoryItem>.On<InventoryItem.noteID.IsEqual<CSAnswers.refNoteID>.And<CSAnswers.attributeID.IsEqual<QtyCartonAttr>>>.
                          Where<InventoryItem.inventoryID.IsEqual<SOShipLine.inventoryID.FromCurrent>>.
                          SearchFor<CSAnswers.value>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBScalar(typeof(SelectFrom<CSAnswers>.
                          LeftJoin<InventoryItem>.On<InventoryItem.noteID.IsEqual<CSAnswers.refNoteID>.And<CSAnswers.attributeID.IsEqual<QtyCartonAttr>>>.
                          Where<InventoryItem.inventoryID.IsEqual<SOShipLine.inventoryID>>.
                          SearchFor<CSAnswers.value>))]
        public virtual string QtyPerCarton { get; set; }
        public abstract class qtyPerCarton : BqlString.Field<qtyPerCarton> { }
        #endregion

        #endregion

        #region UsrHSCode
        public class HSCodeAttr : PX.Data.BQL.BqlString.Constant<HSCodeAttr>
        {
            public HSCodeAttr() : base("HSCODE") { }
        }

        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "HS Code")]
        [PXDefault(typeof(SelectFrom<CSAnswers>.InnerJoin<InventoryItem>.On<InventoryItem.noteID.IsEqual<CSAnswers.refNoteID>
                                                                           .And<CSAnswers.attributeID.IsEqual<HSCodeAttr>>>
                                               .Where<InventoryItem.inventoryID.IsEqual<SOShipLine.inventoryID.FromCurrent>>.SearchFor<CSAnswers.value>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<SOShipLine.inventoryID>))]
        public virtual string UsrHSCode { get; set; }
        public abstract class usrHSCode : PX.Data.BQL.BqlString.Field<usrHSCode> { }
        #endregion
    }
}
