using MaterialDesignThemes.Wpf;
using SCADA.Manager;
using SCADA.Models;
using SCADA.Service.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace SCADA.ViewModels
{
    public class HomeViewModel : BindableBase
    {
        public HomeViewModel(IRegionManager regionManager)
        {
            HomeMenubar = [];
            _regionManager = regionManager;
            Message = new();
            HomeNavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            GetMenuBar();
        }

        private void GetMenuBar()
        {
            LocalSetting localsetting = new();
            string jsonfilepath = localsetting.HomeMenubarFilename;
            if (!File.Exists(jsonfilepath))
            {
                List<MenuBar> menuitems =
                    [
                    new (){ Title="点检",SelectedIcon = PackIconKind.EyeOutline, UnselectedIcon = PackIconKind.Eye, NameSpace="DailyCheckPage"},
                    new (){ Title="生产",SelectedIcon = PackIconKind.Restart, UnselectedIcon = PackIconKind.MotionPauseOutline, NameSpace="ProductionPage"},
                    new (){ Title="停机",SelectedIcon = PackIconKind.TimerPlayOutline, UnselectedIcon = PackIconKind.TimerPlay, NameSpace="DownTimePage"},
                    ];
                JsonHelper.WrtToFile(jsonfilepath, menuitems);
            }
            var HomeMenuBarData = JsonHelper.GetData<ObservableCollection<MenuBar>>(jsonfilepath);
            try
            {
                if (HomeMenuBarData != null)
                {
                    foreach (var item in HomeMenuBarData)
                    {
                        HomeMenubar.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex));
            }
        }

        private void Navigate(MenuBar bar)
        {
            if (bar == null || string.IsNullOrWhiteSpace(bar.NameSpace))
                return;
            _regionManager.Regions[PrismManager.HomeRegionName].RequestNavigate(bar.NameSpace);
        }

        #region 属性定义

        private readonly IRegionManager _regionManager;
        private SnackbarMessageQueue _message;

        public SnackbarMessageQueue Message
        {
            get { return _message; }
            set { _message = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<MenuBar> _homemenubar;

        public ObservableCollection<MenuBar> HomeMenubar
        {
            get { return _homemenubar; }
            set { _homemenubar = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<MenuBar> HomeNavigateCommand { get; set; }

        #endregion 属性定义
    }
}