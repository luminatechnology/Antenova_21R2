using System;
using PX.Data;

namespace AntenovaCustomizations.DAC
{
    [Serializable]
    [PXCacheName("ENGProjectType")]
    public class ENGProjectType : IBqlTable
    {
        #region Prjtype
        [PXDBDefault()]
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Value ID", Required = true)]
        public virtual string Prjtype { get; set; }
        public abstract class prjtype : PX.Data.BQL.BqlString.Field<prjtype> { }
        #endregion

        #region Description
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Desc")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region Sort
        [PXDBInt()]
        [PXUIField(DisplayName = "Sort")]
        public virtual int? Sort { get; set; }
        public abstract class sort : PX.Data.BQL.BqlInt.Field<sort> { }
        #endregion

        #region Active
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
        #endregion

        #region LinkOppr
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Link Oppr")]
        public virtual bool? LinkOppr { get; set; }
        public abstract class linkOppr : PX.Data.BQL.BqlBool.Field<linkOppr> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedByDateTime
        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedByDateTime { get; set; }
        public abstract class lastModifiedByDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedByDateTime> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}