using SCADA.Events;
using SCADA.Models;
using SCADA.Service.Helper;
using SCADA.Service.SqlServer;
using SCADA.Service.SqlServer.Timing;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Media;

namespace SCADA.ViewModels.HomePages
{
    public class DownTimePageViewModel : BindableBase
    {
        public DownTimePageViewModel(IEventAggregator eventAggregator)
        {
            _stopCatelogService = new(SQLiteService.Instance.Db);
            _machineStatusService = new(SQLiteService.Instance.Db);
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OnTimingCategoryChanged>().Subscribe(GetEvent);
            DowntimeCatalogs = [];
            GetCatalogs();
            SelectedCommand = new(SelectDownTime);
        }

        private void GetEvent(string timingCategory)
        {
            var newstopID = _machineStatusService.GetStatus().StopID;
            if (nowstopID != newstopID)
            {
                GetCatalogs();
                nowstopID = newstopID;
            }
        }

        private void GetCatalogs()
        {
            var list = _stopCatelogService.GetAllData()
                .Where(x => x.StopCatagory == "计划停机" || x.StopCatagory == "非计划停机" || x.StopCatagory == "运行")
                .Select(x => new { x.StopName, x.StopCatagory })
                .ToList();
            var nowtime = _machineStatusService.GetStatus();
            DowntimeCatalogs.Clear();
            foreach (var item in list)
            {
                DowntimeCatalog value = new()
                {
                    Name = item.StopName,
                    Catagory = item.StopCatagory,
                    Background = nowtime.StopID == _stopCatelogService.GetStopID(item.StopName) ? Brushes.Red : Brushes.Green,
                };
                DowntimeCatalogs.Add(value);
            }
        }

        private void SelectDownTime(DowntimeCatalog catalog)
        {
            var stopID = _stopCatelogService.GetStopID(catalog.Name);
            nowstopID = _machineStatusService.GetStatus().StopID;
            TimingHelper timing = new();
            if (nowstopID != stopID)
            {
                timing.TimingSetting(stopID, out string timingcatagory);
                GetCatalogs();
            }
            else
            {
                timing.TimingSetting(0, out string timingcatagory);
                GetCatalogs();
            }
            ;
        }

        #region 属性定义

        private int nowstopID;
        private readonly IEventAggregator _eventAggregator;
        private ObservableCollection<DowntimeCatalog> _downtimecatalogs;
        private StopCatelogService _stopCatelogService;
        private MachineStatusService _machineStatusService;

        public ObservableCollection<DowntimeCatalog> DowntimeCatalogs
        {
            get { return _downtimecatalogs; }
            set { _downtimecatalogs = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<DowntimeCatalog> SelectedCommand { get; set; }

        #endregion 属性定义
    }
}