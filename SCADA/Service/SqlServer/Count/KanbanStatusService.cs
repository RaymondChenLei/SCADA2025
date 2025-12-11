using Oracle.ManagedDataAccess.Types;
using SCADA.Service.Models.Count;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer.Count
{
    public class KanbanStatusService : BaseService
    {
        public KanbanStatusService(ISqlSugarClient client) : base(client)
        {
            SetTable<KanbanStatus>("KanbanStatus");
        }

        public KanbanStatus GetLastKB()
        {
            var result = Client.Queryable<KanbanStatus>().ToList();
            if (result.Any())
            {
                return result.FirstOrDefault();
            }
            else
            {
                return new KanbanStatus();
            }
        }

        public void UpdateKBStatus(KanbanStatus newKB)
        {
            var result = Client.Queryable<KanbanStatus>().ToList();
            if (result.Any())
            {
                Client.Updateable(newKB).Where(x => x.Id == result.FirstOrDefault().Id).ExecuteCommand();
            }
            else
            {
                Client.Insertable(newKB).ExecuteCommand();
            }
        }

        public void UpdatePartsKBStatus(KanbanStatus newKB)
        {
            var result = Client.Queryable<KanbanStatus>().ToList();
            if (result.Any())
            {
                Client.Updateable(newKB)
                    .Where(x => x.Id == result.FirstOrDefault().Id)
                    .WhereColumns(x => new { x.KB, x.MaterialD, x.MaterialE, x.MaterialF, x.MaterialG, x.Shift, x.ShiftDate })
                    .ExecuteCommand();
            }
            else
            {
                Client.Insertable(newKB).ExecuteCommand();
            }
        }

        public void SetKBScanDone()
        {
            var result = Client.Queryable<KanbanStatus>().ToList();
            if (result.Any())
            {
                var kb = result.FirstOrDefault();
                kb.ScanDone = true;
                Client.Updateable(kb).ExecuteCommand();
            }
        }

        public bool IfKBScanDone()
        {
            var result = Client.Queryable<KanbanStatus>().ToList();
            if (result.Any())
            {
                return result.FirstOrDefault().ScanDone;
            }
            else
            {
                return false;
            }
        }
    }
}