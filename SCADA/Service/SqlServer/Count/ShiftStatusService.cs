using SCADA.Service.Models.Count;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer.Count
{
    public class ShiftStatusService : BaseService
    {
        public ShiftStatusService(ISqlSugarClient client) : base(client)
        {
            SetTable<ShiftStatus>("ShiftStatus");
        }
    }
}