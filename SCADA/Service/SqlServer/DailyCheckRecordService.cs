using SCADA.Manager;
using SCADA.Service.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class DailyCheckRecordService : BaseService
    {
        public DailyCheckRecordService(ISqlSugarClient client) : base(client)
        {
            SetTable<DailyCheckRecord>("DailyCheckRecord");
        }

        public void InsertRecord(List<DailyCheckRecord> records, out long id)
        {
            var deviceid = records.FirstOrDefault().DeviceID;
            var userid = records.FirstOrDefault().CreateUserID;
            var shiftdate = records.FirstOrDefault().ShiftDate;
            Client.Deleteable<DailyCheckRecord>()
                .Where(it => it.DeviceID == deviceid)
                .Where(x => x.CreateUserID == userid)
                .Where(u => u.ShiftDate == shiftdate)
                .ExecuteCommand();
            id = Client.Insertable(records).ExecuteReturnSnowflakeId();
        }

        public bool IsCheckDone()
        {
            var result = Client.Queryable<DailyCheckRecord>()
                .Where(x => x.IsSubmit == true).Count();
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<DailyCheckRecord> GetRecords(string productNo, DateTime date)
        {
            return Client.Queryable<DailyCheckRecord>()
                .Where(x => x.DeviceID == productNo)
                .Where(x => x.ShiftDate == date)
                .ToList();
        }
    }
}