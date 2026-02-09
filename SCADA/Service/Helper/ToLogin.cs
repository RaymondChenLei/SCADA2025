using SCADA.Manager;
using SCADA.Service.Models;
using SCADA.Service.SqlServer;
using SCADA.Service.SqlServer.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Helper
{
    public class ToLogin
    {
        public void Login(string cardID, out string userName, out string userID)
        {
            var user = _userservice.GetUserbyCardID(cardID.TrimStart('\u0002').TrimEnd('\r', '\n', '\u0003'));
            userName = user.UserName;
            userID = user.IdNo;
            var classsetting = _classsettingservice.GetSettingList();
            var shiftDAYstarttime = DateTime.Parse(classsetting.Where(x => x.ClassName == "早班").FirstOrDefault().StartTime);
            var shiftdate = DateTime.Now.Hour >= shiftDAYstarttime.Hour && DateTime.Now.Minute >= shiftDAYstarttime.Minute ? DateTime.Today : DateTime.Today.AddDays(-1);
            GlobalSettings.Instance.ShiftDate = shiftdate;
            LoginRecord loginrecord = new()
            {
                DeviceID = GlobalSettings.Instance.ProductNo,
                StartTime = DateTime.Now,
                LoginUserID = userID,
                KanbanNo = GlobalSettings.Instance.KB,
                ShiftDate = shiftdate,
            };
            _loginRecordService.InsertRecord(loginrecord);
            TimingHelper timing = new();
            timing.TimingSetting(0, out string stopname);
            GlobalSettings.Instance.IsNeedDailyCheck = true;
        }

        public bool IfNeedDailyCheck()
        {
            return !_dailycheckrecordservice.IsCheckDone();
        }

        #region 属性定义

        private DailyCheckRecordService _dailycheckrecordservice = new(SqlService.Instance.Client);
        private LoginRecordService _loginRecordService = new(SqlService.Instance.Client);
        private UserService _userservice = new(SqlService.Instance.Client);
        private ClassSettingService _classsettingservice = new(SqlService.Instance.Client);
        private EquipmentCatalogService _equipmentcatalogservice = new(SQLiteService.Instance.Db);

        #endregion 属性定义
    }
}