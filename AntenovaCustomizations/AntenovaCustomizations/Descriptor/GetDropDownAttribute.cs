using PX.CS;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
