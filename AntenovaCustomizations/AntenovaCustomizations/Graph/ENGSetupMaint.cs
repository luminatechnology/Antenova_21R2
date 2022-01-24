using AntenovaCustomizations.DAC;
using PX.Data;
using PX.Data.BQL.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntenovaCustomizations.Graph
{
    public class ENGSetupMaint : PXGraph<ENGSetupMaint>
    {
        public PXSave<ENGSetup> Save;
        public PXCancel<ENGSetup> Cancel;
        public SelectFrom<ENGSetup>.View Header;
        public SelectFrom<ENGProjectType>.OrderBy<ENGProjectType.sort.Asc>.View TypeList;
    }
}
