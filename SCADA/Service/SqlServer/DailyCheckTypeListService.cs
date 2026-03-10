using SCADA.Service.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class DailyCheckTypeListService : BaseService
    {
        public DailyCheckTypeListService(ISqlSugarClient client) : base(client)
        {
            SetTable<DailyCheckTypeList>("DailyCheckTypeList");
        }

        internal List<DailyCheckTypeList> GetAllData()
        {
            return Client.Queryable<DailyCheckTypeList>().ToList();
        }

        internal void UpdateAllData(List<DailyCheckTypeList> sqldailychecktypelist)
        {
            Client.Storageable(sqldailychecktypelist).WhereColumns(x => x.Type).ExecuteCommand();
        }
    }
}