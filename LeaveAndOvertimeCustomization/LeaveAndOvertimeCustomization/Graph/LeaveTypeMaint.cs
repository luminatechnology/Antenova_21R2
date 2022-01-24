using LeaveAndOvertimeCustomization.DAC;
using PX.Data;
using PX.Data.BQL.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveAndOvertimeCustomization
{
    public class LeaveTypeMaint : PXGraph<LeaveTypeMaint>
    {
        public PXSave<LumLeaveType> save;
        public PXCancel<LumLeaveType> cancel;
        public SelectFrom<LumLeaveType>.View leaveTypeList;

        public virtual void _(Events.RowSelected<LumLeaveType> e)
        {
            if (e.Row is LumLeaveType row && row != null && !string.IsNullOrEmpty(row.LeaveType))
                PXUIFieldAttribute.SetEnabled<LumLeaveType.leaveType>(e.Cache, null, false);
            else
                PXUIFieldAttribute.SetEnabled<LumLeaveType.leaveType>(e.Cache, null, true);
        }

    }
}
