using System;
using PX.Data;
using PX.Objects.IN;

namespace AntenovaCustomizations
{
    public class LUMItemLotSerFileMaint : PXGraph<LUMItemLotSerFileMaint>
    {
        [PXFilterable()]
        public PXSelect<INItemLotSerial> ItemLotSerial;
        public PXCancel<INItemLotSerial> Cancel;
    }
}