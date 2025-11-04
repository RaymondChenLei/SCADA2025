using SCADA.Service.Models.Count;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer.Count
{
    public class CountStatusService : BaseService
    {
        public CountStatusService(ISqlSugarClient client) : base(client)
        {
            SetTable<CountStatus>("CountStatus");
        }

        public void UpdateStatus(CountStatus count)
        {
            int id;
            var result = Client.Queryable<CountStatus>().ToList();
            if (result.Any())
            {
                count.Id = result.FirstOrDefault().Id;
                Client.Updateable(count).ExecuteCommand();
            }
            else
            {
                Client.Storageable(count).ExecuteCommand();
            }
        }

        public CountStatus GetData()
        {
            var result = Client.Queryable<CountStatus>().ToList();
            if (result.Any())
            {
                return result.FirstOrDefault();
            }
            else
            {
                CountStatus count = new() { GoodCount = 0, NGCount = 0, KB = "Null", SampleCount = 0, TotalCount = 0 };
                return count;
            }
        }
    }
}