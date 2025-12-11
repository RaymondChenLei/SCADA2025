using SCADA.Interface;
using SCADA.Service.Helper;
using SCADA.ViewModels;
using SCADA.ViewModels.Dialogs;
using SCADA.ViewModels.HomePages;
using SCADA.ViewModels.RecordsPages;
using SCADA.Views;
using SCADA.Views.Dialogs;
using SCADA.Views.HomePages;
using SCADA.Views.RecordsPages;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SCADA
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        //protected override void OnInitialized()
        //{
        //}

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDialogHostService, DialogHostService>();
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
            containerRegistry.RegisterForNavigation<DailyCheckPage, DailyCheckPageViewModel>();
            containerRegistry.RegisterForNavigation<DownTimePage, DownTimePageViewModel>();
            containerRegistry.RegisterForNavigation<ProductionPage, ProductionPageViewModel>();
            containerRegistry.RegisterForNavigation<ScanDialog, ScanDialogViewModel>();
            containerRegistry.RegisterForNavigation<RecordsView, RecordsViewModel>();
            containerRegistry.RegisterForNavigation<ProcessView, ProcessViewModel>();
            containerRegistry.RegisterForNavigation<ChartsView, ChartsViewModel>();
            containerRegistry.RegisterForNavigation<SettingView, SettingViewModel>();
            containerRegistry.RegisterForNavigation<DailyCheckRecordView, DailyCheckRecordViewModel>();
            containerRegistry.RegisterForNavigation<DowntimeRecordView, DowntimeRecordViewModel>();
            containerRegistry.RegisterForNavigation<InspectionRecordView, InspectionRecordViewModel>();
            containerRegistry.RegisterForNavigation<MaintenanceRecordView, MaintenanceRecordViewModel>();
            containerRegistry.RegisterForNavigation<ProductionRecordView, ProductionRecordViewModel>();
        }
    }
}