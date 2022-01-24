using PX.Data;
using PX.Data.BQL.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntenovaCustomizations.Graph
{
    public class CRMendCustMaint : PXGraph<CRMendCustMaint>
    {
        [PXImport]
        [PXViewName("CustList")]
        public SelectFrom<CRMendcust>.View CustList;

        [PXImport]
        [PXViewName("SourceList")]
        public SelectFrom<CRMSource>.View SourceList;

        public PXSave<CRMendcust> save;
        public PXCancel<CRMendcust> cancel;
    }
}
