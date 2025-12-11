using SCADA.Service.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class InspectionService : BaseService
    {
        public InspectionService(ISqlSugarClient client) : base(client)
        {
            SetTable<Inspection>("Inspection");
        }

        public void InsertNewRecord(Inspection record)
        {
            Client.Insertable(record).ExecuteCommand();
        }

        public List<Inspection> GetRecords(string productNo, DateTime date)
        {
            return Client.Queryable<Inspection>()
                .Where(x => x.DeviceID == productNo)
                .Where(x => x.ShiftDate == date)
                .ToList();
        }
    }
}