using MaterialDesignThemes.Wpf;
using SCADA.Manager;
using SCADA.Models.RecordsDataGrid;
using SCADA.Service.SqlServer;
using SCADA.Service.SqlServer.Count;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.ViewModels.RecordsPages
{
    public class DailyCheckRecordViewModel : BindableBase
    {
        public DailyCheckRecordViewModel()
        {
            _dailycheckrecordservice = new(SqlService.Instance.Client);
            Message = new();
            Records = [];
            Query = new(GetRecords);
        }

        private void GetRecords()
        {
            try
            {
                var list = _dailycheckrecordservice.GetRecords(GlobalSettings.Instance.ProductNo, Date);
                Records.Clear();
                foreach (var item in list)
                {
                    DailyCheckDataGrid value = new()
                    {
                        序号 = item.ID,
                        创建时间 = item.CreateTime.ToString("yyyy/M/d HH:mm"),
                        备注 = item.Remark,
                        工号 = item.CreateUserID,
                        操作者 = item.UserName,
                        数值 = item.Value,
                        是否合格 = item.IsOK,
                        是否提交 = item.IsSubmit,
                        点检内容 = item.CheckName,
                        班次日期 = item.ShiftDate,
                        设备 = item.DeviceID
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

        private DailyCheckRecordService _dailycheckrecordservice;
        private DateTime _date = DateTime.Today.AddDays(-1);
        private SnackbarMessageQueue _message;
        private ObservableCollection<DailyCheckDataGrid> _records;

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

        public ObservableCollection<DailyCheckDataGrid> Records
        {
            get { return _records; }
            set { _records = value; RaisePropertyChanged(); }
        }

        #endregion 属性定义
    }
}