//using PX.CS;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Linq;

namespace AntenovaCustomizations.Descriptor
{
    public class GetDropDownAttribute : PXStringListAttribute
    {
        private string _attributeID = string.Empty;
        private bool _showDesc = false;

        public GetDropDownAttribute() : base() { }

        public GetDropDownAttribute(string _id, bool _ShowDesc = false, bool _mutiSelect=false)
        {
            this._attributeID = _id;
            this._showDesc = _ShowDesc;
            this.MultiSelect = _mutiSelect;
        }

        public override void CacheAttached(PXCache sender)
        {
            base.CacheAttached(sender);
            var data = SelectFrom<CSAttributeDetail>
                       .Where<CSAttributeDetail.attributeID.IsEqual<P.AsString>
                            .And<CSAttributeDetail.disabled.IsEqual<False>>>
                       .View.Select(new PXGraph(), this._attributeID).RowCast<CSAttributeDetail>();
            if (data != null)
            {
                try
                {
                    this._AllowedLabels = this._showDesc ? data.Select(x => x.Description).ToArray() : data.Select(x => x.ValueID).ToArray();
                    this._AllowedValues = data.Select(x => x.ValueID).ToArray();
                }
                catch(Exception)
                {
                    this._AllowedLabels = new string[1];
                    this._AllowedValues = new string[1];
                }
            }
        }
    }

    #region ItemPlasticWeightAttribute
    public class ItemPlasticWeightAttribute : PXEventSubscriberAttribute, IPXFieldDefaultingSubscriber
    {
        const string PlastWeighID = "PLASTWEIGH";

        public class PlastWeighAttr : PX.Data.BQL.BqlString.Constant<PlastWeighAttr>
        {
            public PlastWeighAttr() : base(PlastWeighID) { }
        }

        public virtual void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            var row = e.Row as PX.Objects.SO.SOPackageDetailEx;

            if (e.NewValue == null && row?.InventoryID != null)
            {
                var attr = CSAnswers.PK.Find(sender.Graph, InventoryItem.PK.Find(sender.Graph, row.InventoryID)?.NoteID, PlastWeighID);

                e.NewValue = row.Qty * Convert.ToDecimal(attr?.Value ?? "0");
            }
        }
        #endregion
    }
}
