using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;

namespace PX.Objects.PO
{
    public class POLineExt : PXCacheExtension<POLine>
    {
        #region Unbound Fields

        #region UsrItemClassCD
        [PXString()]
        [PXUIField(DisplayName = "Item Class", Visibility = PXUIVisibility.SelectorVisible, Enabled = false, IsReadOnly = true)]
        [PXSelector(typeof(Search<INItemClass.itemClassCD>), typeof(INItemClass.descr),
                           DescriptionField = typeof(INItemClass.descr),
                           CacheGlobal = true)]
        [PXDefault(typeof(SelectFrom<INItemClass>.InnerJoin<InventoryItem>.On<INItemClass.itemClassID.IsEqual<InventoryItem.itemClassID>>
                                     .Where<InventoryItem.inventoryID.IsEqual<POLine.inventoryID.FromCurrent>>.SearchFor<INItemClass.itemClassCD>),
                   PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<POLine.inventoryID>))]
        [PXDBScalar(typeof(SelectFrom<INItemClass>.InnerJoin<InventoryItem>.On<INItemClass.itemClassID.IsEqual<InventoryItem.itemClassID>>
                                     .Where<InventoryItem.inventoryID.IsEqual<POLine.inventoryID>>.SearchFor<INItemClass.itemClassCD>))]
        public virtual string UsrItemClassCD { get; set; }
        public abstract class usrItemClassCD : PX.Data.BQL.BqlString.Field<usrItemClassCD> { }
        #endregion

        #endregion
    }
}
