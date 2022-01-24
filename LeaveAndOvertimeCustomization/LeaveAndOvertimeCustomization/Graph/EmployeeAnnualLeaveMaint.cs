using LeaveAndOvertimeCustomization.DAC;
using LeaveAndOvertimeCustomization.Graph;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveAndOvertimeCustomization
{
    public class EmployeeAnnualLeaveMaint : PXGraph<EmployeeAnnualLeaveMaint>
    {
        public PXSave<LumEmployeeAnnualLeave> save;
        public PXCancel<LumEmployeeAnnualLeave> cancel;

        public SelectFrom<LumEmployeeAnnualLeave>.View Document;

        public SelectFrom<LumEmployeeAnnualLeaveLine>
               .Where<LumEmployeeAnnualLeaveLine.employeeID.IsEqual<LumEmployeeAnnualLeave.employeeID.FromCurrent>
                   .And<LumEmployeeAnnualLeaveLine.leaveType.IsEqual<LumEmployeeAnnualLeaveLine.leaveType.FromCurrent>>>
               .View Transaction;

        public SelectFrom<LumEmployeeCompensated>
               .Where<LumEmployeeCompensated.employeeID.IsEqual<LumEmployeeAnnualLeave.employeeID.FromCurrent>>
               .View CompensatedTrans;

        #region Action
        public PXAction<LumEmployeeCompensated> viewOvertime;
        [PXButton]
        [PXUIField(Visible = false)]
        public virtual IEnumerable ViewOvertime(PXAdapter adapter)
        {
            var row = this.CompensatedTrans.Current;
            var graph = PXGraph.CreateInstance<OvertimeRequestEntry>();
            graph.document.Current = LumOvertimeRequest.PK.Find(this, row.RefNbr);
            PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.NewWindow);
            return adapter.Get();
        }

        #endregion

        #region Events

        public virtual void _(Events.RowSelected<LumEmployeeAnnualLeave> e)
        {
            var doc = e.Row;
            var totalCompensated = SelectFrom<LumEmployeeCompensated>
                                   .Where<LumEmployeeCompensated.employeeID.IsEqual<P.AsInt>>
                                   .View.Select(this, doc.EmployeeID).RowCast<LumEmployeeCompensated>().ToList()
                                   .Where(x => x.AvailableYear == DateTime.Now.Year).Sum(x => x.TransferHours);
            var usedCompensated = SelectFrom<LumLeaveRequest>
                                  .Where<LumLeaveRequest.status.IsEqual<LumLeaveRequestStatus.approved>
                                    .And<LumLeaveRequest.requestEmployeeID.IsEqual<P.AsInt>>>
                                  .View.Select(this, doc.EmployeeID).RowCast<LumLeaveRequest>().ToList()
                                  .Where(x => x.LeaveStart?.Year == DateTime.Now.Year && x.LeaveType == "Day in Lieu").Sum(x => x.Duration);
            this.Document.Cache.SetValueExt<LumEmployeeAnnualLeave.availCompensatedHrs>(doc, (totalCompensated - usedCompensated) / 8);
            this.Document.Cache.SetValueExt<LumEmployeeAnnualLeave.approvedCompensatedHrs>(doc, usedCompensated / 8);
        }

        public virtual void _(Events.RowSelected<LumEmployeeAnnualLeaveLine> e)
        {
            if (!e.Row.AnnualLeaveDays.HasValue)
                e.Row.AnnualLeaveDays = (e.Row.LeaveHours ?? 0) / 8;
            if (!e.Row.CarryForwardDays.HasValue)
                e.Row.CarryForwardDays = (e.Row.CarryForwardHours ?? 0) / 8;
            e.Cache.SetValueExt<LumEmployeeAnnualLeaveLine.entitledDays>(e.Row, (decimal)((e.Row.AnnualLeaveDays ?? 0) + (e.Row.CarryForwardDays ?? 0)));
        }

        public virtual void _(Events.FieldUpdated<LumEmployeeAnnualLeaveLine.leaveHours> e)
        {
            var row = (LumEmployeeAnnualLeaveLine)e.Row;
            e.Cache.SetValueExt<LumEmployeeAnnualLeaveLine.entitledDays>(e.Row, (((decimal)e.NewValue) + (row.CarryForwardDays ?? 0)));
        }

        public virtual void _(Events.FieldUpdated<LumEmployeeAnnualLeaveLine.carryForwardDays> e)
        {
            var row = (LumEmployeeAnnualLeaveLine)e.Row;
            e.Cache.SetValueExt<LumEmployeeAnnualLeaveLine.entitledDays>(e.Row, (((decimal)e.NewValue) + row.AnnualLeaveDays));
        }

        #endregion

    }
}
