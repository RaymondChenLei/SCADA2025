using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using SCADA.Manager;
using SCADA.Models;
using SCADA.Models.RecordsDataGrid;
using SCADA.Service.SqlServer;
using SCADA.Service.SqlServer.Timing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.ViewModels.RecordsPages
{
    public class InspectionRecordViewModel : BindableBase
    {
        public InspectionRecordViewModel()
        {
            _inspectionservice = new(SqlService.Instance.Client);
            Message = new();
            Records = [];
            Query = new(GetRecords);
        }

        private void GetRecords()
        {
            try
            {
                var list = _inspectionservice.GetRecords(GlobalSettings.Instance.ProductNo, Date);
                Records.Clear();
                foreach (var item in list)
                {
                    var inspectionitems = JsonConvert.DeserializeObject<InspectionHSDJson>(item.JsonContent);
                    InspectionDataGrid value = new()
                    {
                        序号 = item.ID,
                        开始时间 = item.StartTime.ToString("yyyy/M/d HH:mm"),
                        工号 = item.CreateUserID,
                        看板号 = item.KanbanNo,
                        结束时间 = item.EndTime.ToString("yyyy/M/d HH:mm"),
                        持续时间 = (float)(item.EndTime - item.StartTime).TotalMinutes,
                        班次日期 = item.ShiftDate,
                        设备 = item.DeviceID,
                        外观 = item.IsGood,
                        弯曲 = item.WqGood,
                        检测次序 = item.CheckType,
                        模具1 = item.DieNo1,
                        模具2 = item.DieNo2,
                        模具3 = item.DieNo3,
                        中心导体1剥皮长度 = inspectionitems.中心导体1剥皮长度,
                        中心导体1压接宽度 = inspectionitems.中心导体1压接宽度,
                        中心导体1压接高度 = inspectionitems.中心导体1压接高度,
                        中心导体1拉力 = inspectionitems.中心导体1拉力,
                        中心导体2剥皮长度 = inspectionitems.中心导体2剥皮长度,
                        中心导体2压接宽度 = inspectionitems.中心导体2压接宽度,
                        中心导体2压接高度 = inspectionitems.中心导体2压接高度,
                        中心导体2拉力 = inspectionitems.中心导体2拉力,
                        中心导体3剥皮长度 = inspectionitems.中心导体3剥皮长度,
                        中心导体3压接宽度 = inspectionitems.中心导体3压接宽度,
                        中心导体3压接高度 = inspectionitems.中心导体3压接宽度,
                        中心导体3拉力 = inspectionitems.中心导体3拉力,
                        中心导体4剥皮长度 = inspectionitems.中心导体4剥皮长度,
                        中心导体4压接宽度 = inspectionitems.中心导体4压接宽度,
                        中心导体4压接高度 = inspectionitems.中心导体4压接高度,
                        中心导体4拉力 = inspectionitems.中心导体4拉力
                    };
                    Records.Add(value);
                }
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        #region 属性定义

        private InspectionService _inspectionservice;
        private DateTime _date = DateTime.Today.AddDays(-1);
        private SnackbarMessageQueue _message;
        private ObservableCollection<InspectionDataGrid> _records;

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; RaisePropertyChanged(); }
        }

        public SnackbarMessageQueue Message
        {
            get { return _message; }
            set { _message = value; RaisePropertyChanged(); }
        }

        public DelegateCommand Query { get; set; }

        public ObservableCollection<InspectionDataGrid> Records
        {
            get { return _records; }
            set { _records = value; RaisePropertyChanged(); }
        }

        #endregion 属性定义
    }
}