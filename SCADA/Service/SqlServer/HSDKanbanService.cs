using SCADA.Service.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class HSDKanbanService : BaseService
    {
        public HSDKanbanService(ISqlSugarClient client) : base(client)
        {
            SetTable<HSDKanban>("HsdKanban");
        }

        public List<HSDKanban> GetKanban(string productNo)
        {
            return Client.Queryable<HSDKanban>().Where(x => x.DeviceID == productNo).ToList();
        }

        public void UpdataAllKanban(List<HSDKanban> sqlkabban)
        {
            Client.Deleteable<HSDKanban>().ExecuteCommand();
            Client.Insertable(sqlkabban).ExecuteCommand();
        }

        public HSDKanban GetInfobyKB(string kanban)
        {
            var result = Client.Queryable<HSDKanban>().Where(x => x.KanbanNo == kanban).ToList();
            if (result.Any())
            {
                return result.FirstOrDefault();
            }
            else
            {
                return new HSDKanban();
            }
        }
    }
}