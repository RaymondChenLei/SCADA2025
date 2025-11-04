using SCADA.Models;
using SCADA.Service.Helper;
using SCADA.Service.SqlServer;
using SCADA.Service.SqlServer.Timing;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace SCADA.ViewModels.HomePages
{
    public class DownTimePageViewModel : BindableBase
    {
        public DownTimePageViewModel()
        {
            _stopCatelogService = new(SQLiteService.Instance.Db);
            _machineStatusService = new(SQLiteService.Instance.Db);
            DowntimeCatalogs = [];
            GetCatalogs();
            SelectedCommand = new(SelectDownTime);
        }

        private void GetCatalogs()
        {
            var list = _stopCatelogService.GetAllData()
                .Where(x => x.StopCatagory == "计划停机" || x.StopCatagory == "非计划停机")
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
            TimingHelper timing = new();
            timing.TimingSetting(stopID, out string timingcatagory);
            GetCatalogs();
        }

        #region 属性定义

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