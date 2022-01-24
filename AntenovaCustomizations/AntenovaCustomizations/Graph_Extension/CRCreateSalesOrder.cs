using PX.Data;
using PX.Objects.CR;
using PX.Objects.CR.Extensions.CRCreateSalesOrder;
using PX.Objects.Extensions.Discount;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PX.Objects.CR.Extensions
{
    public class CRCreateSalesOrderExt<TDiscountExt, TGraph, TMaster> : CRCreateSalesOrder<OpportunityMaint.Discount, OpportunityMaint, CROpportunity>
    {
        public override CRQuote GetQuoteForWorkflowProcessing()
        {
            return Base.PrimaryQuoteQuery.SelectSingle();
        }

        public override void DoCreateSalesOrder()
        {
            try
            {
                base.DoCreateSalesOrder();
            }
            catch (PXRedirectRequiredException ex)
            {
                var opportunityID = Base.Opportunity.Current.OpportunityID;
                var soGraph = ex.Graph;
                var soLine = soGraph.Caches[typeof(SOLine)].Cached.RowCast<SOLine>();
                foreach (var item in soLine)
                    item.GetExtension<SOLineExt>().UsrOpportunityID = opportunityID;
                throw new PXRedirectRequiredException(soGraph, "");
            }
        }
    }
}
