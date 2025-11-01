using SCADA.Views;
using SCADA.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;
using SCADA.Views.HomePages;
using SCADA.ViewModels.HomePages;

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
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
            containerRegistry.RegisterForNavigation<DailyCheckPage, DailyCheckPageViewModel>();
            containerRegistry.RegisterForNavigation<DownTimePage, DownTimePageViewModel>();
            containerRegistry.RegisterForNavigation<MaintenancePage, MaintenancePageViewModel>();
            containerRegistry.RegisterForNavigation<ProductionPage, ProductionPageViewModel>();
        }
    }
}