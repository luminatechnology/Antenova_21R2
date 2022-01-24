using System;
using PX.Data;

namespace LeaveAndOvertimeCustomization.DAC
{
    [Serializable]
    [PXCacheName("v_GetOrganizationWorkGroup")]
    public class v_GetOrganizationWorkGroup : IBqlTable
    {
        #region Workgroupid
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Workgroupid")]
        public virtual int? Workgroupid { get; set; }
        public abstract class workgroupid : PX.Data.BQL.BqlInt.Field<workgroupid> { }
        #endregion

        #region Description
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion
    }
}