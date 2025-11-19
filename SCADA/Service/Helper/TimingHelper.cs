using SCADA.Service.SqlServer.Timing;
using SCADA.Service.SqlServer;

namespace SCADA.Service.Helper
{
    public class TimingHelper
    {
        public void TimingSetting(int stopID, out string stopcatagory)
        {
            var Nowstatus = _machinestatusservice.GetStatus();
            if (Nowstatus is not null)
            {
                if (Nowstatus.StopID != stopID)
                {
                    _upowntimerecordservice.SaveLastTiming(Nowstatus);
                    _machinestatusservice.SetStatus(stopID);
                    stopcatagory = _stopcatelogservice.GetStopName(Nowstatus.StopID);
                }
                else
                {
                    stopcatagory = _stopcatelogservice.GetStopName(Nowstatus.StopID);
                }
            }
            else
            {
                stopcatagory = "未分类停机";
            }
        }

        #region 属性定义

        private UpDownTimeRecordService _upowntimerecordservice = new(SqlService.Instance.Client);
        private MachineStatusService _machinestatusservice = new(SQLiteService.Instance.Db);
        private StopCatelogService _stopcatelogservice = new(SQLiteService.Instance.Db);

        #endregion 属性定义
    }
}