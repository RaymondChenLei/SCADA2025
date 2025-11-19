using SCADA.Service.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class DailyCheckContentService : BaseService
    {
        public DailyCheckContentService(ISqlSugarClient client) : base(client)
        {
            //SetTable<DailyCheckContent>("MaintenanceCheckContent");
        }

        public List<DailyCheckContent> GetApprovedContent(string type)
        {
            var result = Client.Queryable<DailyCheckTypeList>()
                .Where(x => x.Type == type)
                .Where(x => x.Status == "QC批准")
                .ToList();
            if (result.Count > 0)
            {
                var test = Client.Queryable<DailyCheckContent>()
                .Where(x => x.Type == type)
                .Where(x => x.IsEnabled == true)
                .Where(x => x.ReviewID != null)
                .ToList();
                return Client.Queryable<DailyCheckContent>()
                .Where(x => x.Type == type)
                .Where(x => x.IsEnabled == true)
                .Where(x => x.ReviewID != null)
                .ToList();
            }
            else
            {
                return [];
            }
        }
    }
}