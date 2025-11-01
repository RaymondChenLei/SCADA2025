using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;
using System.IO.Ports;
using SCADA.Events;

namespace SCADA.ViewModels.HomePages
{
    public class ProductionPageViewModel : BindableBase
    {
        public ProductionPageViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TextUpdatedEvent>().Subscribe(text => TargetValue = text);
        }

        #region 属性定义

        private int _targetvalue;

        public int TargetValue
        {
            get { return _targetvalue; }
            set { _targetvalue = value; RaisePropertyChanged(); }
        }

        #endregion 属性定义
    }
}