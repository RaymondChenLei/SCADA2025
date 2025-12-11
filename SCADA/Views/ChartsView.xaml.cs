using MaterialDesignThemes.Wpf;
using SCADA.ViewModels;
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

namespace SCADA.Views
{
    /// <summary>
    /// ChartsView.xaml 的交互逻辑
    /// </summary>
    public partial class ChartsView : UserControl
    {
        public ChartsView()
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

        ((ChartsViewModel)DataContext).Date = Calendar.SelectedDate.Value;
        }

        public void CalendarDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        => Calendar.SelectedDate = ((ChartsViewModel)DataContext).Date;
    }
}