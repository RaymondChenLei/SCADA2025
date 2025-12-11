using SCADA.Service.Models.Count;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer.Count
{
    public class KanbanCountService : BaseService
    {
        public KanbanCountService(ISqlSugarClient client) : base(client)
        {
            //SetTable<KanbanCount>("KanbanCount");
        }

        public void InsertKanban(KanbanCount record)
        {
            Client.Insertable(record).ExecuteCommand();
        }

        public List<KanbanCount> GetRecords(string productNo, DateTime date)
        {
            return Client.Queryable<KanbanCount>()
                .Where(x => x.DeviceID == productNo)
                .Where(x => x.ShiftDate == date)
                .ToList();
        }
    }
}