using System;
using PX.Data;

namespace PX.Objects.SO
{
    public class SOOrderEntry_Extension : PXGraphExtension<SOOrderEntry>
    {
        #region Event Handlers  
        protected void SOOrder_RowSelected(PXCache sender, PXRowSelectedEventArgs e, PXRowSelected InvokeBaseHandler)
        {
            if(InvokeBaseHandler != null)
              InvokeBaseHandler(sender, e);

            // Make the promised on ship date field editable even after the order has been completed.
            // This code is not enough to make the feature work - automation steps need to be modified for SO Completed and SO Invoiced to ensure the
            // caches are not disabled.
            sender.AllowUpdate = true;
            Base.Transactions.Cache.AllowUpdate = true;
        }
  
        protected void SOLine_RowSelected(PXCache cache, PXRowSelectedEventArgs e, PXRowSelected InvokeBaseHandler)
        {
            if(InvokeBaseHandler != null)
              InvokeBaseHandler(cache, e);

            //Automation steps were modified to keep the transactions grid enabled for the completed status; we are manually disabling it here but leaving the promised on ship date field editable.
            //if(Base.Document.Current.Status == SOOrderStatus.Completed)
                //PXUIFieldAttribute.SetEnabled(cache, e.Row, false);
           
            PXUIFieldAttribute.SetEnabled<SOLineExt.usrNewPO>(cache, e.Row, true);
        }
        #endregion
    }
}