using MaterialDesignThemes.Wpf;
using SCADA.Manager;
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
    public class DowntimeRecordViewModel : BindableBase
    {
        public DowntimeRecordViewModel()
        {
            _updowntimerecordservice = new(SqlService.Instance.Client);
            Message = new();
            Records = [];
            Query = new(GetRecords);
        }

        private void GetRecords()
        {
            try
            {
                var list = _updowntimerecordservice.GetRecords(GlobalSettings.Instance.ProductNo, Date);
                Records.Clear();
                foreach (var item in list)
                {
                    DownTimeDataGrid value = new()
                    {
                        序号 = item.ID,
                        开始时间 = item.StartTime.ToString("yyyy/M/d HH:mm"),
                        设备类别 = item.MachineName,
                        工号 = item.OperatorID,
                        看板号 = item.KanbanNo,
                        结束时间 = item.EndTime.ToString("yyyy/M/d HH:mm"),
                        停机分类 = item.TimingCatagory,
                        停机名称 = item.TimingName,
                        持续时间 = (float)Math.Round((item.EndTime - item.StartTime).TotalMinutes, 2),
                        班次日期 = item.ShiftDate,
                        设备 = item.ProductNo
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

        private UpDownTimeRecordService _updowntimerecordservice;
        private DateTime _date = DateTime.Today.AddDays(-1);
        private SnackbarMessageQueue _message;
        private ObservableCollection<DownTimeDataGrid> _records;

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

        public ObservableCollection<DownTimeDataGrid> Records
        {
            get { return _records; }
            set { _records = value; RaisePropertyChanged(); }
        }

        #endregion 属性定义
    }
}