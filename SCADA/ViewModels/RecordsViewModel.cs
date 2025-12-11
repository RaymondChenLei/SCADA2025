using MaterialDesignThemes.Wpf;
using SCADA.Manager;
using SCADA.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.ViewModels
{
    public class RecordsViewModel : BindableBase
    {
        public RecordsViewModel(IRegionManager regionManager)
        {
            RecordsMenubar = [];
            _regionManager = regionManager;
            Message = new();
            RecordsNavigateCommand = new(Navigate);
            GetMenuBar();
        }

        private void GetMenuBar()
        {
            try
            {
                RecordsMenubar.Add(
                    new() { Title = "生产记录", SelectedIcon = PackIconKind.ReceiptTextEditOutline, UnselectedIcon = PackIconKind.ReceiptTextEdit, NameSpace = "ProductionRecordView" }
                    );
                RecordsMenubar.Add(
                    new() { Title = "点检记录", SelectedIcon = PackIconKind.FileDocumentCheckOutline, UnselectedIcon = PackIconKind.FileDocumentCheck, NameSpace = "DailyCheckRecordView" }
                    );
                RecordsMenubar.Add(
                    new() { Title = "检验记录", SelectedIcon = PackIconKind.PencilRulerOutline, UnselectedIcon = PackIconKind.PencilRuler, NameSpace = "InspectionRecordView" }
                    );
                RecordsMenubar.Add(
                    new() { Title = "停机记录", SelectedIcon = PackIconKind.CogPauseOutline, UnselectedIcon = PackIconKind.CogPause, NameSpace = "DowntimeRecordView" }
                    );
                RecordsMenubar.Add(
                    new() { Title = "维修记录", SelectedIcon = PackIconKind.WrenchCogOutline, UnselectedIcon = PackIconKind.WrenchCog, NameSpace = "MaintenanceRecordView" }
                    );
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        private void Navigate(MenuBar bar)
        {
            if (bar == null || string.IsNullOrWhiteSpace(bar.NameSpace))
                return;
            _regionManager.Regions[PrismManager.RecordsRegionName].RequestNavigate(bar.NameSpace);
        }

        #region 属性定义

        private readonly IRegionManager _regionManager;
        private SnackbarMessageQueue _message;
        private ObservableCollection<MenuBar> _RecordsMenubar;

        public SnackbarMessageQueue Message
        {
            get { return _message; }
            set { _message = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<MenuBar> RecordsMenubar
        {
            get { return _RecordsMenubar; }
            set { _RecordsMenubar = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<MenuBar> RecordsNavigateCommand { get; set; }

        #endregion 属性定义
    }
}