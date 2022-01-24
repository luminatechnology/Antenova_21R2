using PX.Data;

namespace SalesForecastANTENOVA
{
    public class SalesForecastEntry : PXGraph<SalesForecastEntry>
    { 
        public PXSavePerRow<SalesForecast> Save;
        public PXCancel<SalesForecast> Cancel;
  
        [PXFilterable]
        [PXImport(typeof(SalesForecast))]
        public PXSelect<SalesForecast> SalesForcast;
      
        #region CacheAttached
        //[PXMergeAttributes(Method = MergeMethod.Merge)]
        //[PXUIFieldAttribute(DisplayName = "Product Category")]
        //protected virtual void InventoryID_description_CacheAttached(PXCache cache) { }
        #endregion
    }
}