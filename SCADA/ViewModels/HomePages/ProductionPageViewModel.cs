using Newtonsoft.Json;
using SCADA.Events;
using SCADA.Interface;
using SCADA.Manager;
using SCADA.Models;
using SCADA.Models.Matrix;
using SCADA.Service.Helper;
using SCADA.Service.Models;
using SCADA.Service.Models.Count;
using SCADA.Service.SqlServer;
using SCADA.Service.SqlServer.Count;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace SCADA.ViewModels.HomePages
{
    public class ProductionPageViewModel : BindableBase
    {
        public ProductionPageViewModel(IDialogHostService dialog, IEventAggregator eventAggregator)
        {
            _countstatusservice = new(SQLiteService.Instance.Db);
            _kanbanstatusservice = new(SQLiteService.Instance.Db);
            _sqliteKanbanservice = new(SQLiteService.Instance.Db);
            eventAggregator.GetEvent<TextUpdatedEvent>().Subscribe(text => TargetValue = text + initData.TotalCount);
            eventAggregator.GetEvent<KBChangeEvent>().Subscribe(kb => Kanban = kb);
            AddNG = new(AddNGExecution);
            AddSample = new(AddSampleExecution);
            SubtractNG = new(SubtractNGExecution);
            SubtractSample = new(SubtractSampleExecution);
            ButtonEnable();
            DieParas = [];
            _dialogHostService = dialog;
            InitQty();
            Check = new(CheckValue);
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

        private void ChangeKB()
        {
            if (string.IsNullOrWhiteSpace(Kanban))
            {
                return;
            }
            else
            {
                GlobalSettings.Instance.KB = Kanban;
                var KBinfo = _sqliteKanbanservice.GetInfobyKB(Kanban);
                KanbanTitleA = "看板号：";
                KanbanTitleB = "零件号：";
                KanbanTitleC = "项目名：";
                KanbanInfoA = KBinfo.KanbanNo;
                KanbanInfoB = KBinfo.KanbanName;
                KanbanInfoC = KBinfo.ProjectName;
                var kbjson = JsonConvert.DeserializeObject<List<KBJson>>(KBinfo.JsonContent);
                TimingHelper timing = new();
                timing.TimingSetting(5, out string timingcatagory);
                if (GlobalSettings.Instance.ProductNo.Contains("HSD410"))
                {
                    FreshHSD410KanbanInfo(KBinfo, kbjson);
                }
            }
        }

        private void FreshHSD410KanbanInfo(HSDKanban kBinfo, List<KBJson> kbjson)
        {
            KanbanTitleD = "中心导体1模具：";
            KanbanTitleE = "中心导体1端子：";
            KanbanTitleF = "中心导体2模具：";
            KanbanTitleG = "中心导体2端子：";
            KanbanInfoD = kBinfo.ACenterTerminalDieNo;
            KanbanInfoE = kBinfo.ACenterTerminalNo;
            KanbanInfoF = kBinfo.BCenterTerminalDieNo;
            KanbanInfoG = kBinfo.BCenterTerminalNo;
            KanbanStatus newKB = new()
            {
                KB = kBinfo.KanbanNo,
                MaterialD = KanbanInfoD,
                MaterialE = KanbanInfoE,
                MaterialF = KanbanInfoF,
                MaterialG = KanbanInfoG,
                Shift = GlobalSettings.Instance.Shift,
                ShiftDate = GlobalSettings.Instance.ShiftDate
            };
            _kanbanstatusservice.UpdateKBStatus(newKB);
            if (DieParas is not null)
            {
                DieParas.Clear();
            }
            foreach (var item in kbjson)
            {
                StringDouble value = new()
                {
                    Title = item.Name,
                    Value1 = item.MinValue,
                    Value2 = item.MaxValue
                };
                DieParas.Add(value);
            }
            ChangeOver CO = new();
            if (CO.IfNeedScan(newKB))
            {
                DialogParameters p = new()
                {
                    { "MaterialDName","中心导体1模具" },
                    { "MaterialEName","中心导体1端子" },
                    { "MaterialFName","中心导体2模具" },
                    { "MaterialGName","中心导体2端子" },
                    { "MaterialHName","" },
                    { "MaterialIName","" },
                    { "MaterialDTarget",KanbanInfoD },
                    { "MaterialETarget",KanbanInfoE },
                    { "MaterialFTarget",KanbanInfoF },
                    { "MaterialGTarget",KanbanInfoG },
                    { "MaterialHTarget","" },
                    { "MaterialITarget","" },
                };
                _dialogHostService.ShowDialog("ScanDialog", p);
            }
        }

        private void InitQty()
        {
            initData = _countstatusservice.GetData();
            TargetValue = initData.TotalCount;
            NGQty = initData.NGCount;
            SampleQty = initData.SampleCount;
            GoodQty = initData.GoodCount;
            Kanban = _kanbanstatusservice.GetLastKB().KB;
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

        private void CheckValue()
        {
            List<StringDouble> list = [];
            foreach (var item in DieParas)
            {
                StringDouble value = new()
                {
                    ActualColor = item.ActualColor,
                    ActualValue = item.ActualValue,
                    Title = item.Title,
                    Value1 = item.Value1,
                    Value2 = item.Value2
                };
                list.Add(value);
            }
            DieParas.Clear();
            foreach (StringDouble item in list)
            {
                StringDouble value = new()
                {
                    ActualColor = (item.ActualValue >= item.Value1 && item.ActualValue <= item.Value2) ? Brushes.LightGreen : Brushes.Red,
                    ActualValue = item.ActualValue,
                    Title = item.Title,
                    Value1 = item.Value1,
                    Value2 = item.Value2
                };
                DieParas.Add(value);
            }
        }

        #region 属性定义

        private readonly IDialogHostService _dialogHostService;
        private KanbanStatusService _kanbanstatusservice;
        private CountStatusService _countstatusservice;
        private int _goodqty;
        private bool _isaddngbtnenable;
        private bool _isaddsamplebtnenable;
        private bool _issubtractngbtnenable;
        private bool _issubtractsamplebtnenable;
        private string _kanban = "L9500000";
        private string _kanbaninfoA;
        private string _kanbaninfoB;
        private string _kanbaninfoC;
        private string _kanbaninfoD;
        private string _kanbaninfoE;
        private string _kanbaninfoF;
        private string _kanbaninfoG;
        private string _kanbaninfoH;
        private string _kanbaninfoI;
        private string _kanbantitleA;
        private string _kanbantitleB;
        private string _kanbantitleC;
        private string _kanbantitleD;
        private string _kanbantitleE;
        private string _kanbantitleF;
        private string _kanbantitleG;
        private string _kanbantitleH;
        private string _kanbantitleI;
        private int _ngqty;
        private int _sampleqty;
        private HSDKanbanService _sqliteKanbanservice;
        private int _targetvalue;
        private CountStatus initData;
        public DelegateCommand AddNG { get; set; }
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

        public string Kanban
        {
            get { return _kanban; }
            set { _kanban = value; RaisePropertyChanged(); ChangeKB(); }
        }

        public string KanbanInfoA
        {
            get { return _kanbaninfoA; }
            set { _kanbaninfoA = value; RaisePropertyChanged(); }
        }

        public string KanbanInfoB
        {
            get { return _kanbaninfoB; }
            set { _kanbaninfoB = value; RaisePropertyChanged(); }
        }

        public string KanbanInfoC
        {
            get { return _kanbaninfoC; }
            set { _kanbaninfoC = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<StringDouble> _dieparas;

        public ObservableCollection<StringDouble> DieParas
        {
            get { return _dieparas; }
            set { _dieparas = value; RaisePropertyChanged(); }
        }

        public string KanbanInfoD
        {
            get { return _kanbaninfoD; }
            set { _kanbaninfoD = value; RaisePropertyChanged(); }
        }

        public string KanbanInfoE
        {
            get { return _kanbaninfoE; }
            set { _kanbaninfoE = value; RaisePropertyChanged(); }
        }

        public string KanbanInfoF
        {
            get { return _kanbaninfoF; }
            set { _kanbaninfoF = value; RaisePropertyChanged(); }
        }

        public string KanbanInfoG
        {
            get { return _kanbaninfoG; }
            set { _kanbaninfoG = value; RaisePropertyChanged(); }
        }

        public DelegateCommand Check { get; set; }

        public string KanbanInfoH
        {
            get { return _kanbaninfoH; }
            set { _kanbaninfoH = value; RaisePropertyChanged(); }
        }

        public string KanbanInfoI
        {
            get { return _kanbaninfoI; }
            set { _kanbaninfoI = value; RaisePropertyChanged(); }
        }

        public string KanbanTitleA
        {
            get { return _kanbantitleA; }
            set { _kanbantitleA = value; RaisePropertyChanged(); }
        }

        public string KanbanTitleB
        {
            get { return _kanbantitleB; }
            set { _kanbantitleB = value; RaisePropertyChanged(); }
        }

        public string KanbanTitleC
        {
            get { return _kanbantitleC; }
            set { _kanbantitleC = value; RaisePropertyChanged(); }
        }

        public string KanbanTitleD
        {
            get { return _kanbantitleD; }
            set { _kanbantitleD = value; RaisePropertyChanged(); }
        }

        public string KanbanTitleE
        {
            get { return _kanbantitleE; }
            set { _kanbantitleE = value; RaisePropertyChanged(); }
        }

        public string KanbanTitleF
        {
            get { return _kanbantitleF; }
            set { _kanbantitleF = value; RaisePropertyChanged(); }
        }

        public string KanbanTitleG
        {
            get { return _kanbantitleG; }
            set { _kanbantitleG = value; RaisePropertyChanged(); }
        }

        public string KanbanTitleH
        {
            get { return _kanbantitleH; }
            set { _kanbantitleH = value; RaisePropertyChanged(); }
        }

        public string KanbanTitleI
        {
            get { return _kanbantitleI; }
            set { _kanbantitleI = value; RaisePropertyChanged(); }
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