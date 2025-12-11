using MaterialDesignThemes.Wpf;
using SCADA.ViewModels;
using SCADA.ViewModels.RecordsPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCADA.Views.RecordsPages
{
    /// <summary>
    /// ProductionRecordView.xaml 的交互逻辑
    /// </summary>
    public partial class ProductionRecordView : UserControl
    {
        public ProductionRecordView()
        {
            InitializeComponent();
        }

        public void CalendarDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!Calendar.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

        ((ProductionRecordViewModel)DataContext).Date = Calendar.SelectedDate.Value;
        }

        public void CalendarDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        => Calendar.SelectedDate = ((ProductionRecordViewModel)DataContext).Date;
    }
}