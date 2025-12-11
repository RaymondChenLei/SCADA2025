using MaterialDesignThemes.Wpf;
using SCADA.Manager;
using SCADA.Models.RecordsDataGrid;
using SCADA.Service.SqlServer;
using SCADA.Service.SqlServer.Count;
using System.Collections.ObjectModel;

namespace SCADA.ViewModels.RecordsPages
{
    public class ProductionRecordViewModel : BindableBase
    {
        public ProductionRecordViewModel()
        {
            _kanbancoucountService = new(SqlService.Instance.Client);
            Message = new();
            Records = [];
            Query = new(GetRecords);
        }

        private void GetRecords()
        {
            try
            {
                var list = _kanbancoucountService.GetRecords(GlobalSettings.Instance.ProductNo, Date);
                Records.Clear();
                foreach (var item in list)
                {
                    ProductionDataGrid value = new()
                    {
                        序号 = item.ID,
                        不合格数 = item.NgCount,
                        合格数 = item.PassCount,
                        工号 = item.CreateUserID,
                        总数 = item.NgCount + item.PassCount,
                        是否完成 = item.Isfinish,
                        班次日期 = item.ShiftDate,
                        看板号 = item.KanbanNo,
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

        private KanbanCountService _kanbancoucountService;
        private DateTime _date = DateTime.Today.AddDays(-1);
        private SnackbarMessageQueue _message;
        private ObservableCollection<ProductionDataGrid> _records;

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

        public ObservableCollection<ProductionDataGrid> Records
        {
            get { return _records; }
            set { _records = value; RaisePropertyChanged(); }
        }

        #endregion 属性定义
    }
}