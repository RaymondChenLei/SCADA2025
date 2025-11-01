using SCADA.Manager;
using SCADA.Service.Models;
using SCADA.Service.Models.Timing;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer.Timing
{
    public class UpDownTimeRecordService : BaseService
    {
        public UpDownTimeRecordService(ISqlSugarClient client) : base(client)
        {
            SetTable<UpDownTimeRecord>("UpDownTimeRecord");
        }

        private StopCatelogService _stopCatelogService = new(SQLiteService.Instance.Db);

        public void SaveLastTiming(MachineStatus nowstatus)
        {
            var productNo = GlobalSettings.Instance.ProductNo;
            var equipmentName = Client.Queryable<EquipmentCatalog>().Where(x => x.ProductNo == productNo).First()?.MachineName ?? null;
            var timename = _stopCatelogService.GetStopName(nowstatus.StopID);
            var timecata = _stopCatelogService.GetStopCata(nowstatus.StopID);
            UpDownTimeRecord record = new()
            {
                ProductNo = productNo,
                MachineName = equipmentName,
                TimingCatagory = timecata,
                TimingName = timename,
                StartTime = nowstatus.StartTime,
                EndTime = DateTime.Now,
                OperatorID = GlobalSettings.Instance.CurrentUserId,
                KanbanNo = GlobalSettings.Instance.KB,
                ShiftDate = GlobalSettings.Instance.ShiftDate
            };
            Client.Insertable(record).ExecuteCommand();
        }
    }
}