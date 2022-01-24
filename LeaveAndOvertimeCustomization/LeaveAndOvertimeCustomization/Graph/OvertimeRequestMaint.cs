using LeaveAndOvertimeCustomization.DAC;
using PX.Data;
using PX.Data.BQL.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Objects.EP;
using PX.Data.EP;
using LeaveAndOvertimeCustomization.Descriptor;
using PX.Data.BQL;
using System.Collections;

namespace LeaveAndOvertimeCustomization.Graph
{
    public class OvertimeRequestMaint : PXGraph<OvertimeRequestMaint>
    {
        public PXFilter<OvertimeRequestFilter> Filter;

        [PXFilterable]
        public SelectFrom<LumOvertimeRequest>
               .Where<LumOvertimeRequest.requestEmployeeID.IsEqual<OvertimeRequestFilter.employeeID.FromCurrent>>
               .View document;

        public IEnumerable Document()
        {
            PXView select = new PXView(this, true, document.View.BqlSelect);
            Int32 totalrow = 0;
            Int32 startrow = PXView.StartRow;
            List<object> result = select.Select(PXView.Currents, PXView.Parameters,
                   PXView.Searches, PXView.SortColumns, PXView.Descendings,
                   PXView.Filters, ref startrow, PXView.MaximumRows, ref totalrow);
            PXView.StartRow = 0;
            foreach (LumOvertimeRequest item in result)
            {
                EPEmployee employee = SelectFrom<EPEmployee>.Where<EPEmployee.bAccountID.IsEqual<P.AsInt>>.View.Select(this, item.RequestEmployeeID);
                if (employee != null)
                { 
                    item.RequestDays = item.RequestHours / 8;
                    item.ApprovedRequestDays = item.ApprovedRequestHours / 8;
                }
            }
            return result;
        }

        public OvertimeRequestMaint()
        {
            this.document.View.IsReadOnly = true;
        }

        #region Action

        public PXSave<LumOvertimeRequest> Save;

        public PXAction<OvertimeRequestFilter> createNew;
        [PXInsertButton]
        [PXUIField(DisplayName = "")]
        protected virtual void CreateNew()
        {
            using (new PXPreserveScope())
            {
                OvertimeRequestEntry graph = (OvertimeRequestEntry)PXGraph.CreateInstance(typeof(OvertimeRequestEntry));
                graph.Clear(PXClearOption.ClearAll);
                LumOvertimeRequest claim = (LumOvertimeRequest)graph.document.Cache.CreateInstance();
                graph.document.Insert(claim);
                graph.document.Cache.IsDirty = false;
                PXRedirectHelper.TryRedirect(graph, PXRedirectHelper.WindowMode.InlineWindow);
            }
        }

        public PXAction<OvertimeRequestFilter> EditDetail;
        [PXEditDetailButton]
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select)]
        protected virtual void editDetail()
        {
            if (document.Current == null) return;
            LumOvertimeRequest row = PXSelect<LumOvertimeRequest, Where<LumOvertimeRequest.refNbr, Equal<Required<LumOvertimeRequest.refNbr>>>>.SelectSingleBound(this, null, document.Current.RefNbr);
            PXRedirectHelper.TryRedirect(this, row, PXRedirectHelper.WindowMode.InlineWindow);
        }

        public PXAction<OvertimeRequestFilter> delete;
        [PXDeleteButton]
        [PXUIField(DisplayName = "")]
        protected void Delete()
        {
            if (document.Current == null) return;

            if (document.Current.Status == LumOvertimeRequestStatus.Approved || document.Current.Status == LumOvertimeRequestStatus.Rejected)
                throw new PXException("can not delete this Document");

            OvertimeRequestEntry graph = (OvertimeRequestEntry)PXGraph.CreateInstance(typeof(OvertimeRequestEntry));
            graph.Clear(PXClearOption.ClearAll);
            graph.document.Current = graph.document.Search<LumOvertimeRequest.refNbr>(document.Current.RefNbr);
            graph.Delete.Press();
            this.document.View.RequestRefresh();
        }

        #endregion

    }

    [Serializable]
    [PXHidden]
    public partial class OvertimeRequestFilter : IBqlTable
    {
        private int? _employeeId;

        #region EmployeeID
        public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }

        [PXDBInt]
        [PXUIField(DisplayName = "Employee")]
        [PXDefault(typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSubordinateAndWingmenSelector()]
        [PXFieldDescription]
        public virtual Int32? EmployeeID
        {
            get { return _employeeId; }
            set { _employeeId = value; }
        }

        #endregion
    }
}
