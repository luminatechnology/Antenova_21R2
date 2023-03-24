using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;

namespace PX.Objects.PO
{
    public class POLineExt : PXCacheExtension<POLine>
    {
        #region Unbound Fields

        #region UsrItemClassID
        [PXInt()]
        [PXUIField(DisplayName = "Item Class", Visibility = PXUIVisibility.SelectorVisible, Enabled = false, IsReadOnly = true)]
        [PXDimensionSelector(INItemClass.Dimension, typeof(Search<INItemClass.itemClassID>), typeof(INItemClass.itemClassCD), 
                             DescriptionField = typeof(INItemClass.descr), 
                             CacheGlobal = true)]
        [PXDefault(typeof(SelectFrom<InventoryItem>.Where<InventoryItem.inventoryID.IsEqual<POLine.inventoryID.FromCurrent>>.SearchFor<InventoryItem.itemClassID>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<POLine.inventoryID>))]
        [PXDBScalar(typeof(SelectFrom<InventoryItem>.Where<InventoryItem.inventoryID.IsEqual<POLine.inventoryID>>.SearchFor<InventoryItem.itemClassID>))]
        public virtual int? UsrItemClassID { get; set; }
        public abstract class usrItemClassID : PX.Data.BQL.BqlInt.Field<usrItemClassID> { }
        #endregion

        #endregion
    }
}
