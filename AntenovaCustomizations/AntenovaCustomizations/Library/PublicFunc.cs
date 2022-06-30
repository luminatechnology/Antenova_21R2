using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntenovaCustomizations.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.EP;
using PX.SM;
using PX.TM;

namespace AntenovaCustomizations.Library
{
    public class PublicFunc
    {
        /// <summary> Work Group CRM Name Constant </summary>
        public class WorkGroupCrm : PX.Data.BQL.BqlString.Constant<WorkGroupCrm>
        {
            public WorkGroupCrm() : base("(CRM)") { }
        }

        public class DepartmentName : PX.Data.BQL.BqlString.Constant<DepartmentName>
        {
            public DepartmentName() : base("RD") { }
        }

        /// <summary> Get Parent CRM Work Group ID </summary>
        public virtual int? GetCRMWorkGroupID()
        {
            return SelectFrom<EPCompanyTree>
                .Where<EPCompanyTree.description.Contains<PublicFunc.WorkGroupCrm>>
                .View.Select(new PXGraph()).RowCast<EPCompanyTree>().FirstOrDefault()?.WorkGroupID;
        }

        /// <summary> Get WorkGroup and Employee Info By Sales Person </summary>
        public virtual PXResultset<vSALESPERSONREGIONMAPPING> GetWGInfoByEmployeeFromSP(int _salesPersonID)
        {
            return SelectFrom<vSALESPERSONREGIONMAPPING>
                   .Where<vSALESPERSONREGIONMAPPING.salespersonID.IsEqual<P.AsInt>>.View.Select(new PXGraph(), _salesPersonID);
        }

        /// <summary> Check AccessRole </summary>
        public virtual bool CheckAcessRoleByWP(int? _userContactID, int? _workgroup)
        {
            var IsAdmin = SelectFrom<UsersInRoles>
                .Where<UsersInRoles.username.IsEqual<AccessInfo.userName.FromCurrent>>
                .View.Select(new PXGraph()).RowCast<UsersInRoles>()
                .Where(x => x.Rolename.Contains("Administrator")).Any();
            var gpRoles = SelectFrom<EPCompanyTreeMember>
                .Where<EPCompanyTreeMember.contactID.IsEqual<P.AsInt>>
                .View.Select(new PXGraph(), _userContactID).RowCast<EPCompanyTreeMember>()
                .Where(x => x.WorkGroupID == _workgroup).Any();
            return IsAdmin || gpRoles;
        }

        /// <summary> Get Employee By Sales Person</summary>
        public int? GetEmployeeBySalesPerson(int SalesPerson)
        {
            return SelectFrom<EPEmployee>
                   .Where<EPEmployee.salesPersonID.IsEqual<P.AsInt>>
                   .View.SelectSingleBound(new PXGraph(), null, SalesPerson).TopFirst?.DefContactID;
        }
    }
}
