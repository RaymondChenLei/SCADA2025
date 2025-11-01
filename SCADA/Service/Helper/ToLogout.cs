using SCADA.Service.Models;
using SCADA.Service.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Helper
{
    public class ToLogout
    {
        public void Logout()
        {
            _loginRecordService.UpdateLoutOut();
            TimingHelper timehelper = new();
            timehelper.TimingSetting(3, out string stopname);
        }

        #region 属性定义

        private LoginRecordService _loginRecordService = new(SqlService.Instance.Client);

        #endregion 属性定义
    }
}