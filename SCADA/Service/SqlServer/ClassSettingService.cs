using SCADA.Service.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class ClassSettingService : BaseService
    {
        public ClassSettingService(ISqlSugarClient client) : base(client)
        {
            //SetTable<ClassSetting>("ClassSetting");
        }

        public List<ClassSetting> GetSettingList()
        {
            return Client.Queryable<ClassSetting>().ToList();
        }
    }
}