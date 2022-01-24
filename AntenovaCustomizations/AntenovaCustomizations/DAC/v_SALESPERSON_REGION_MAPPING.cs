using System;
using PX.Data;

namespace AntenovaCustomizations.DAC
{
    [Serializable]
    [PXCacheName("vSALESPERSONREGIONMAPPING")]
    public class vSALESPERSONREGIONMAPPING : IBqlTable
    {
        #region WorkGroupID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Work Group ID")]
        public virtual int? WorkGroupID { get; set; }
        public abstract class workGroupID : PX.Data.BQL.BqlInt.Field<workGroupID> { }
        #endregion

        #region Description
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Work Group Name")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region Parentwgid
        [PXDBInt()]
        [PXUIField(DisplayName = "Parentwgid")]
        public virtual int? Parentwgid { get; set; }
        public abstract class parentwgid : PX.Data.BQL.BqlInt.Field<parentwgid> { }
        #endregion

        #region Userid
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Contact ID")]
        public virtual int? ContactID { get; set; }
        public abstract class contactID : PX.Data.BQL.BqlInt.Field<contactID> { }
        #endregion

        #region Active
        [PXDBBool()]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
        #endregion

        #region BAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "BAccount ID")]
        public virtual int? BAccountID { get; set; }
        public abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }
        #endregion

        #region SalespersonID
        [PXDBInt()]
        [PXUIField(DisplayName = "Salesperson ID")]
        public virtual int? SalespersonID { get; set; }
        public abstract class salespersonID : PX.Data.BQL.BqlInt.Field<salespersonID> { }
        #endregion

        #region SalespersonCD
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Salesperson CD")]
        public virtual string SalespersonCD { get; set; }
        public abstract class salespersonCD : PX.Data.BQL.BqlString.Field<salespersonCD> { }
        #endregion

        #region Descr
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Name")]
        public virtual string Descr { get; set; }
        public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }
        #endregion

    }
}