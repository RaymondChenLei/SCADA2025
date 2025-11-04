using SCADA.Service.Models.Timing;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer.Timing
{
    public class StopCatelogService : BaseService
    {
        public StopCatelogService(ISqlSugarClient client) : base(client)
        {
            SetTable<StopCatelog>("StopCatelog");
        }

        public string GetStopName(int stopID)
        {
            var result = Client.Queryable<StopCatelog>()
                .Where(x => x.StopID == stopID)
                .ToList();
            if (result.Any())
            {
                return result.FirstOrDefault().StopName;
            }
            else
            {
                return null;
            }
        }

        public string GetStopCata(int stopID)
        {
            var result = Client.Queryable<StopCatelog>()
                .Where(x => x.StopID == stopID)
                .ToList();
            if (result.Any())
            {
                return result.FirstOrDefault().StopCatagory;
            }
            else
            {
                return null;
            }
        }

        public List<StopCatelog> GetAllData()
        {
            return Client.Queryable<StopCatelog>().ToList();
        }

        public void UpdataAllData(List<StopCatelog> sqldata)
        {
            Client.Deleteable<StopCatelog>().ExecuteCommand();
            Client.Insertable(sqldata).ExecuteCommand();
        }

        public int GetStopID(string name)
        {
            var result = Client.Queryable<StopCatelog>().Where(x => x.StopName == name).ToList();
            if (result.Any())
            {
                return result.FirstOrDefault().StopID;
            }
            else
            {
                return 0;
            }
        }
    }
}