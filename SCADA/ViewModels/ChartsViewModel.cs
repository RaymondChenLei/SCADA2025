using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.ViewModels
{
    public class ChartsViewModel : BindableBase
    {
        public ChartsViewModel()
        {
            Message = new();
        }

        #region 属性定义

        private SnackbarMessageQueue _message;

        public SnackbarMessageQueue Message
        {
            get { return _message; }
            set { _message = value; RaisePropertyChanged(); }
        }

        private DateTime _date = DateTime.Today.AddDays(-1);

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; RaisePropertyChanged(); }
        }

        #endregion 属性定义
    }
}