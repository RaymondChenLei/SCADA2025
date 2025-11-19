using SCADA.Service.Models.Count;
using SCADA.Service.SqlServer;
using SCADA.Service.SqlServer.Count;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Helper
{
    public class ChangeOver
    {
        public bool IfNeedScan(KanbanStatus newKB)
        {
            var result = _statusService.GetLastKB();
            bool allMaterialsMatch = newKB.MaterialD == result.MaterialD &&
                             newKB.MaterialE == result.MaterialE &&
                             newKB.MaterialF == result.MaterialF &&
                             newKB.MaterialG == result.MaterialG &&
                             newKB.MaterialH == result.MaterialH &&
                             newKB.MaterialI == result.MaterialI;
            return !allMaterialsMatch;
        }

        private KanbanStatusService _statusService = new(SQLiteService.Instance.Db);
    }
}