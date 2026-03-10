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
            bool match1 = (newKB.MaterialD ?? "") == (result.MaterialD ?? "");
            bool match2 = (newKB.MaterialE ?? "") == (result.MaterialE ?? "");
            bool match3 = (newKB.MaterialF ?? "") == (result.MaterialF ?? "");
            bool match4 = (newKB.MaterialG ?? "") == (result.MaterialG ?? "");
            bool match5 = (newKB.MaterialH ?? "") == (result.MaterialH ?? "");
            bool match6 = (newKB.MaterialI ?? "") == (result.MaterialI ?? "");
            bool allMaterialsMatch = match1 && match2 && match3 && match4 && match5 && match6;
            return !allMaterialsMatch;
        }

        private KanbanStatusService _statusService = new(SQLiteService.Instance.Db);
    }
}