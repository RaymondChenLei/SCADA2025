using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;
using System.IO.Ports;
using SCADA.Events;
using SCADA.Service.SqlServer.Count;
using SCADA.Service.SqlServer;
using SCADA.Service.Models.Count;

namespace SCADA.ViewModels.HomePages
{
    public class ProductionPageViewModel : BindableBase
    {
        public ProductionPageViewModel(IEventAggregator eventAggregator)
        {
            _countstatusservice = new(SQLiteService.Instance.Db);
            InitQty();
            eventAggregator.GetEvent<TextUpdatedEvent>().Subscribe(text => TargetValue = text + initData.TotalCount);
            AddNG = new(AddNGExecution);
            AddSample = new(AddSampleExecution);
            SubtractNG = new(SubtractNGExecution);
            SubtractSample = new(SubtractSampleExecution);
            ButtonEnable();
        }

        private void InitQty()
        {
            initData = _countstatusservice.GetData();
            TargetValue = initData.TotalCount;
            NGQty = initData.NGCount;
            SampleQty = initData.SampleCount;
            GoodQty = initData.GoodCount;
        }

        private void ButtonEnable()
        {
            if (TargetValue - NGQty - SampleQty == 0)
            {
                IsAddNGBtnEnable = false;
                IsAddSampleBtnEnable = false;
            }
            else
            {
                IsAddNGBtnEnable = true;
                IsAddSampleBtnEnable = true;
            }
            if (NGQty == 0)
            {
                IsSubtractNGBtnEnable = false;
            }
            else
            {
                IsSubtractNGBtnEnable = true;
            }
            if (SampleQty == 0)
            {
                IsSubtractSampleBtnEnable = false;
            }
            else
            {
                IsSubtractSampleBtnEnable = true;
            }
        }

        private void AddNGExecution()
        {
            NGQty += 1;
            UpdateQty();
            ButtonEnable();
        }

        private void AddSampleExecution()
        {
            SampleQty += 1;
            UpdateQty();
            ButtonEnable();
        }

        private void SubtractNGExecution()
        {
            NGQty -= 1;
            UpdateQty();
            ButtonEnable();
        }

        private void SubtractSampleExecution()
        {
            SampleQty -= 1;
            UpdateQty();
            ButtonEnable();
        }

        private void UpdateQty()
        {
            GoodQty = TargetValue - NGQty - SampleQty;
            CountStatus count = new()
            {
                KB = Kanban,
                TotalCount = TargetValue,
                GoodCount = GoodQty,
                NGCount = NGQty,
                SampleCount = SampleQty
            };
            _countstatusservice.UpdateStatus(count);
            ButtonEnable();
        }

        #region 属性定义

        private CountStatus initData;
        private int _goodqty;
        private bool _isaddngbtnenable;
        private bool _isaddsamplebtnenable;
        private bool _issubtractngbtnenable;
        private bool _issubtractsamplebtnenable;
        private int _ngqty;
        private int _sampleqty;
        private int _targetvalue;
        private CountStatusService _countstatusservice;
        public DelegateCommand AddNG { get; set; }
        private string _kanban = "L9500000";

        public string Kanban
        {
            get { return _kanban; }
            set { _kanban = value; RaisePropertyChanged(); }
        }

        public DelegateCommand AddSample { get; set; }

        public int GoodQty
        {
            get { return _goodqty; }
            set { _goodqty = value; RaisePropertyChanged(); }
        }

        public bool IsAddNGBtnEnable
        {
            get { return _isaddngbtnenable; }
            set { _isaddngbtnenable = value; RaisePropertyChanged(); }
        }

        public bool IsAddSampleBtnEnable
        {
            get { return _isaddsamplebtnenable; }
            set { _isaddsamplebtnenable = value; RaisePropertyChanged(); }
        }

        public bool IsSubtractNGBtnEnable
        {
            get { return _issubtractngbtnenable; }
            set { _issubtractngbtnenable = value; RaisePropertyChanged(); }
        }

        public bool IsSubtractSampleBtnEnable
        {
            get { return _issubtractsamplebtnenable; }
            set { _issubtractsamplebtnenable = value; RaisePropertyChanged(); }
        }

        public int NGQty
        {
            get { return _ngqty; }
            set { _ngqty = value; RaisePropertyChanged(); }
        }

        public int SampleQty
        {
            get { return _sampleqty; }
            set { _sampleqty = value; RaisePropertyChanged(); }
        }

        public DelegateCommand SubtractNG { get; set; }

        public DelegateCommand SubtractSample { get; set; }

        public int TargetValue
        {
            get { return _targetvalue; }
            set { _targetvalue = value; RaisePropertyChanged(); UpdateQty(); }
        }

        #endregion 属性定义
    }
}