using SCADA.Service.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class LoginRecordService : BaseService
    {
        public LoginRecordService(ISqlSugarClient client) : base(client)
        {
            //SetTable<LoginRecord>("LoginRecord");
        }

        public void InsertRecord(LoginRecord loginrecord)
        {
            Client.Storageable(loginrecord).ExecuteCommand();
        }

        public void UpdateLoutOut()
        {
            var loginrecord = Client.Queryable<LoginRecord>()
                .OrderByDescending(x => x.StartTime)
                .First();
            if (loginrecord != null)
            {
                loginrecord.EndTime = DateTime.Now;
                Client.Updateable(loginrecord).ExecuteCommand();
            }
        }
    }
}