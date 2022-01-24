using System;
using PX.Data;

namespace PX.Objects.IN
{
    public class INItemLotSerialExt : PXCacheExtension<PX.Objects.IN.INItemLotSerial>
    {
        #region UsrNoteID
        [PXNote()]
        public virtual Guid? UsrNoteID{ get; set; }
        public abstract class usrNoteID: PX.Data.BQL.BqlGuid.Field<usrNoteID> { }
        #endregion
    }
}