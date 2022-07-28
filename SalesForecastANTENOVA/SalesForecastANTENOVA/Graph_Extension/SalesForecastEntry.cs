using PX.Data;
using System.Collections;

namespace SalesForecastANTENOVA
{
    public class SalesForecastEntry : PXGraph<SalesForecastEntry>, PXImportAttribute.IPXPrepareItems
    {
        public PXSavePerRow<SalesForecast> Save;
        public PXCancel<SalesForecast> Cancel;

        [PXFilterable]
        [PXImport(typeof(SalesForecast))]
        public PXSelect<SalesForecast> SalesForcast;

        public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
            values["UnitPrice"] = decimal.Parse(string.IsNullOrEmpty(values["UnitPrice"]?.ToString()) ? "0" : values["UnitPrice"]?.ToString());
            return true;
        }

        public void PrepareItems(string viewName, IEnumerable items) { }

        public bool RowImported(string viewName, object row, object oldRow)
            => true;

        public bool RowImporting(string viewName, object row)
            => true;

        #region CacheAttached
        //[PXMergeAttributes(Method = MergeMethod.Merge)]
        //[PXUIFieldAttribute(DisplayName = "Product Category")]
        //protected virtual void InventoryID_description_CacheAttached(PXCache cache) { }
        #endregion
    }
}