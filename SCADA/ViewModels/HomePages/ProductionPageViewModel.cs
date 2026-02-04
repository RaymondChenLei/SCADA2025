using IdGen;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using SCADA.Events;
using SCADA.Interface;
using SCADA.Manager;
using SCADA.Models;
using SCADA.Models.Matrix;
using SCADA.Service.Helper;
using SCADA.Service.Models;
using SCADA.Service.Models.Count;
using SCADA.Service.Models.Timing;
using SCADA.Service.SqlServer;
using SCADA.Service.SqlServer.Count;
using SCADA.Service.SqlServer.Timing;
using System.Collections.ObjectModel;
using System.Windows;
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
            _inspectionservice = new(SQLiteService.Instance.Db);
            _machinestatusservice = new(SQLiteService.Instance.Db);
            _KanbanCountService = new(SQLiteService.Instance.Db);
            eventAggregator.GetEvent<TextUpdatedEvent>().Subscribe(text => TargetValue = text + initData.TotalCount);
            eventAggregator.GetEvent<KBChangeEvent>().Subscribe(kb => ChangeKB(kb, true));
            AddNG = new(AddNGExecution);
            AddSample = new(AddSampleExecution);
            SubtractNG = new(SubtractNGExecution);
            SubtractSample = new(SubtractSampleExecution);
            ChangeMaterial = new(ChangingMaterial);
            ButtonEnable();
            DieParasOrange = [];
            DieParasBlue = [];
            DieParasBrown = [];
            DieParasGreen = [];
            _dialogHostService = dialog;
            _eventAggregator = eventAggregator;
            InitQty();
            SaveCommand = new(SaveValue);
            StartSample = new(SampleExccution);
            Message = new();
        }

        private void ChangingMaterial()
        {
            MessageBoxResult result = MessageBox.Show
                    (
                    $"确定要更换原材料吗？",
                    "更换原材料确认",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );
            if (result == MessageBoxResult.Yes)
            {
                DialogParameters p = new()
                {
                    { "Type","Material"},
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
            else
            {
                result = MessageBoxResult.No;
            }
        }

        private void AddNGExecution()
        {
            try
            {
                NGQty += 1;
                UpdateQty();
                ButtonEnable();
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        private void AddSampleExecution()
        {
            try
            {
                SampleQty += 1;
                UpdateQty();
                ButtonEnable();
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
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

        private void ChangeKB(string kanban, bool isScan)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(kanban))
                {
                    return;
                }
                else if (_kanbanstatusservice.GetLastKB().KB == kanban)
                {
                    return;
                }
                else
                {
                    if (isScan)
                    {
                        MessageBoxResult result = MessageBox.Show
                        (
                        $"确定切换为看板{kanban}吗？",
                        "切换看板确认",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                        );
                        if (result == MessageBoxResult.Yes)
                        {
                            ChangingKB(kanban);
                        }
                        else
                        {
                            result = MessageBoxResult.No;
                        }
                    }
                    else
                    {
                        ChangingKB(kanban);
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        private void ChangingKB(string KB)
        {
            try
            {
                if (TargetValue > 0)
                {
                    SaveLastKB();
                }
                CounterZeroize(KB);
                GlobalSettings.Instance.KB = KB;
                Kanban = KB;
                var KBinfo = _sqliteKanbanservice.GetInfobyKB(KB);
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
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        private bool CheckValue(List<StringDouble> list)
        {
            bool value = true;
            foreach (var item in list)
            {
                if (item.ActualValue < item.Value1 || item.ActualValue > item.Value2)
                {
                    value = false;
                }
            }
            return value;
        }

        private void CounterZeroize(string KB)
        {
            try
            {
                _eventAggregator.GetEvent<CounterZeroizeEvent>().Publish(0);
                GoodQty = 0;
                NGQty = 0;
                SampleQty = 0;
                TargetValue = 0;
                CountStatus qty = new()
                {
                    GoodCount = 0,
                    NGCount = 0,
                    SampleCount = 0,
                    TotalCount = 0,
                    KB = KB
                };
                _countstatusservice.UpdateStatus(qty);
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        private void FreshHSD410KanbanInfo(HSDKanban kBinfo, List<KBJson> kbjson)
        {
            try
            {
                KanbanTitleD = "中心导体1模具：";
                KanbanTitleE = "中心导体1端子：";
                KanbanTitleF = "中心导体2模具：";
                KanbanTitleG = "中心导体2端子：";
                KanbanInfoD = kBinfo.ACenterTerminalDieNo;
                KanbanInfoE = kBinfo.ACenterTerminalNo;
                KanbanInfoF = kBinfo.BCenterTerminalDieNo;
                KanbanInfoG = kBinfo.BCenterTerminalNo;
                if (GlobalSettings.Instance.ScanDialog)
                {
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
                    _kanbanstatusservice.UpdatePartsKBStatus(newKB);
                    if (DieParasOrange is not null)
                    {
                        DieParasOrange.Clear();
                    }
                    if (DieParasBlue is not null)
                    {
                        DieParasBlue.Clear();
                    }
                    if (DieParasBrown is not null)
                    {
                        DieParasBrown.Clear();
                    }
                    if (DieParasGreen is not null)
                    {
                        DieParasGreen.Clear();
                    }
                    foreach (var item in kbjson)
                    {
                        StringDouble value = new()
                        {
                            Title = item.Name,
                            Value1 = item.MinValue,
                            Value2 = item.MaxValue
                        };
                        DieParasOrange.Add(value);
                    }
                    foreach (var item in kbjson)
                    {
                        StringDouble value = new()
                        {
                            Title = item.Name,
                            Value1 = item.MinValue,
                            Value2 = item.MaxValue
                        };
                        DieParasBlue.Add(value);
                    }
                    foreach (var item in kbjson)
                    {
                        StringDouble value = new()
                        {
                            Title = item.Name,
                            Value1 = item.MinValue,
                            Value2 = item.MaxValue
                        };
                        DieParasBrown.Add(value);
                    }
                    foreach (var item in kbjson)
                    {
                        StringDouble value = new()
                        {
                            Title = item.Name,
                            Value1 = item.MinValue,
                            Value2 = item.MaxValue
                        };
                        DieParasGreen.Add(value);
                    }
                    ChangeOver CO = new();
                    if (CO.IfNeedScan(newKB))
                    {
                        DialogParameters p = new()
                {
                    { "Type","All"},
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
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        private void InitQty()
        {
            try
            {
                var kb = _kanbanstatusservice.GetLastKB().KB;
                ChangeKB(kb, _kanbanstatusservice.IfKBScanDone());
                initData = _countstatusservice.GetData();
                TargetValue = initData.TotalCount;
                NGQty = initData.NGCount;
                SampleQty = initData.SampleCount;
                GoodQty = initData.GoodCount;
                Kanban = _kanbanstatusservice.GetLastKB().KB;
                if (!_kanbanstatusservice.IfKBScanDone())
                {
                    DialogParameters p = new()
                {
                    { "Type","All"},
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
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        private void SampleExccution()
        {
            try
            {
                IsSaveBtnEnable = true;
                TimingHelper timing = new();
                timing.TimingSetting(12, out string timingcatagory);
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        private void SaveLastKB()
        {
            try
            {
                var listKB = _countstatusservice.GetData();
                var generator = new IdGenerator(0);
                KanbanCount record = new()
                {
                    ID = generator.CreateId(),
                    DeviceID = GlobalSettings.Instance.ProductNo,
                    PassCount = listKB.GoodCount,
                    NgCount = listKB.NGCount,
                    CreateUserID = GlobalSettings.Instance.CurrentUserId,
                    CreateTime = DateTime.Now,
                    KanbanNo = listKB.KB,
                    UpdateTime = DateTime.Now,
                    ShiftDate = GlobalSettings.Instance.ShiftDate,
                    Isfinish = true
                };
                _KanbanCountService.InsertKanban(record);
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => { Message.Enqueue(ex.Message); });
            }
        }

        private void SaveValue()
        {
            try
            {
                List<StringDouble> listOra = [];
                List<StringDouble> listBlu = [];
                List<StringDouble> listBro = [];
                List<StringDouble> listGre = [];
                foreach (var item in DieParasOrange)
                {
                    StringDouble value = new()
                    {
                        ActualColor = item.ActualColor,
                        ActualValue = item.ActualValue,
                        Title = item.Title,
                        Value1 = item.Value1,
                        Value2 = item.Value2
                    };
                    listOra.Add(value);
                }
                DieParasOrange.Clear();
                foreach (StringDouble item in listOra)
                {
                    StringDouble value = new()
                    {
                        ActualColor = (item.ActualValue >= item.Value1 && item.ActualValue <= item.Value2) ? Brushes.LightGreen : Brushes.Red,
                        ActualValue = item.ActualValue,
                        Title = item.Title,
                        Value1 = item.Value1,
                        Value2 = item.Value2
                    };
                    DieParasOrange.Add(value);
                }

                foreach (var item in DieParasBlue)
                {
                    StringDouble value = new()
                    {
                        ActualColor = item.ActualColor,
                        ActualValue = item.ActualValue,
                        Title = item.Title,
                        Value1 = item.Value1,
                        Value2 = item.Value2
                    };
                    listBlu.Add(value);
                }
                DieParasBlue.Clear();
                foreach (StringDouble item in listBlu)
                {
                    StringDouble value = new()
                    {
                        ActualColor = (item.ActualValue >= item.Value1 && item.ActualValue <= item.Value2) ? Brushes.LightGreen : Brushes.Red,
                        ActualValue = item.ActualValue,
                        Title = item.Title,
                        Value1 = item.Value1,
                        Value2 = item.Value2
                    };
                    DieParasBlue.Add(value);
                }

                foreach (var item in DieParasBrown)
                {
                    StringDouble value = new()
                    {
                        ActualColor = item.ActualColor,
                        ActualValue = item.ActualValue,
                        Title = item.Title,
                        Value1 = item.Value1,
                        Value2 = item.Value2
                    };
                    listBro.Add(value);
                }
                DieParasBrown.Clear();
                foreach (StringDouble item in listBro)
                {
                    StringDouble value = new()
                    {
                        ActualColor = (item.ActualValue >= item.Value1 && item.ActualValue <= item.Value2) ? Brushes.LightGreen : Brushes.Red,
                        ActualValue = item.ActualValue,
                        Title = item.Title,
                        Value1 = item.Value1,
                        Value2 = item.Value2
                    };
                    DieParasBrown.Add(value);
                }

                foreach (var item in DieParasGreen)
                {
                    StringDouble value = new()
                    {
                        ActualColor = item.ActualColor,
                        ActualValue = item.ActualValue,
                        Title = item.Title,
                        Value1 = item.Value1,
                        Value2 = item.Value2
                    };
                    listGre.Add(value);
                }
                DieParasGreen.Clear();
                foreach (StringDouble item in listGre)
                {
                    StringDouble value = new()
                    {
                        ActualColor = (item.ActualValue >= item.Value1 && item.ActualValue <= item.Value2) ? Brushes.LightGreen : Brushes.Red,
                        ActualValue = item.ActualValue,
                        Title = item.Title,
                        Value1 = item.Value1,
                        Value2 = item.Value2
                    };
                    DieParasGreen.Add(value);
                }
                if (!CheckValue(listGre))
                {
                    Task.Factory.StartNew(() => Message.Enqueue("绿色样件不合格！请重新测量或制作样件！"));
                }
                else if (!CheckValue(listBro))
                {
                    Task.Factory.StartNew(() => Message.Enqueue("粽色样件不合格！请重新测量或制作样件！"));
                }
                else if (!CheckValue(listBlu))
                {
                    Task.Factory.StartNew(() => Message.Enqueue("蓝色样件不合格！请重新测量或制作样件！"));
                }
                else if (!CheckValue(listOra))
                {
                    Task.Factory.StartNew(() => Message.Enqueue("橙色样件不合格！请重新测量或制作样件！"));
                }
                else
                {
                    //中心导体1=>橙色，2=>绿色，3=>蓝色，4=>粽色
                    InspectionHSDJson json = new()
                    {
                        中心导体1剥皮长度 = Math.Round(listOra.Where(x => x.Title == "剥皮长度").First().ActualValue, 3).ToString(),
                        中心导体1压接高度 = Math.Round(listOra.Where(x => x.Title == "压接高度").First().ActualValue, 3).ToString(),
                        中心导体1压接宽度 = Math.Round(listOra.Where(x => x.Title == "压接宽度").First().ActualValue, 3).ToString(),
                        中心导体1拉力 = Math.Round(listOra.Where(x => x.Title == "拉力").First().ActualValue, 3).ToString(),
                        中心导体2剥皮长度 = Math.Round(listGre.Where(x => x.Title == "剥皮长度").First().ActualValue, 3).ToString(),
                        中心导体2压接高度 = Math.Round(listGre.Where(x => x.Title == "压接高度").First().ActualValue, 3).ToString(),
                        中心导体2压接宽度 = Math.Round(listGre.Where(x => x.Title == "压接宽度").First().ActualValue, 3).ToString(),
                        中心导体2拉力 = Math.Round(listGre.Where(x => x.Title == "拉力").First().ActualValue, 3).ToString(),
                        中心导体3剥皮长度 = Math.Round(listBlu.Where(x => x.Title == "剥皮长度").First().ActualValue, 3).ToString(),
                        中心导体3压接高度 = Math.Round(listBlu.Where(x => x.Title == "压接高度").First().ActualValue, 3).ToString(),
                        中心导体3压接宽度 = Math.Round(listBlu.Where(x => x.Title == "压接宽度").First().ActualValue, 3).ToString(),
                        中心导体3拉力 = Math.Round(listBlu.Where(x => x.Title == "拉力").First().ActualValue, 3).ToString(),
                        中心导体4剥皮长度 = Math.Round(listBro.Where(x => x.Title == "剥皮长度").First().ActualValue, 3).ToString(),
                        中心导体4压接高度 = Math.Round(listBro.Where(x => x.Title == "压接高度").First().ActualValue, 3).ToString(),
                        中心导体4压接宽度 = Math.Round(listBro.Where(x => x.Title == "压接宽度").First().ActualValue, 3).ToString(),
                        中心导体4拉力 = Math.Round(listBro.Where(x => x.Title == "拉力").First().ActualValue, 3).ToString()
                    };
                    var test = Type;
                    var generator = new IdGenerator(0);
                    Inspection record = new()
                    {
                        ID = generator.CreateId(),
                        DeviceID = GlobalSettings.Instance.ProductNo,
                        CheckType = Type,
                        JsonContent = JsonConvert.SerializeObject(json),
                        CreateUserID = GlobalSettings.Instance.CurrentUserId,
                        KanbanNo = GlobalSettings.Instance.KB,
                        StartTime = _machinestatusservice.GetStartTime(),
                        EndTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        ShiftDate = GlobalSettings.Instance.ShiftDate,
                        ScannerRecordID = 0,
                        IsGood = IsVisualOK,
                        WqGood = IsBendingOK,
                        DieNo1 = KanbanInfoD,
                        DieNo2 = KanbanInfoF,
                        DieNo3 = ""
                    };
                    _inspectionservice.InsertNewRecord(record);
                    Task.Factory.StartNew(() => Message.Enqueue("保存成功！"));
                    TimingHelper timing = new();
                    timing.TimingSetting(5, out string timingcatagory);
                    IsSaveBtnEnable = false;
                }
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        private void SubtractNGExecution()
        {
            try
            {
                NGQty -= 1;
                UpdateQty();
                ButtonEnable();
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        private void SubtractSampleExecution()
        {
            try
            {
                SampleQty -= 1;
                UpdateQty();
                ButtonEnable();
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        private void UpdateQty()
        {
            try
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
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        #region 属性定义

        private readonly IDialogHostService _dialogHostService;
        private readonly IEventAggregator _eventAggregator;
        private CountStatusService _countstatusservice;
        private ObservableCollection<StringDouble> _dieparablue;
        private ObservableCollection<StringDouble> _dieparabrown;
        private ObservableCollection<StringDouble> _dieparagreen;
        private ObservableCollection<StringDouble> _dieparasorange;
        private int _goodqty;
        private InspectionService _inspectionservice;
        private bool _isaddngbtnenable;
        private bool _isaddsamplebtnenable;
        private bool _isbendingOK;
        private bool _issavebtnenable = false;
        private bool _issubtractngbtnenable;
        private bool _issubtractsamplebtnenable;
        private bool _isvisualOK;
        private string _kanban = "L9500000";
        private KanbanCountService _KanbanCountService;
        private string _kanbaninfoA;
        private string _kanbaninfoB;
        private string _kanbaninfoC;
        private string _kanbaninfoD;
        private string _kanbaninfoE;
        private string _kanbaninfoF;
        private string _kanbaninfoG;
        private string _kanbaninfoH;
        private string _kanbaninfoI;
        private KanbanStatusService _kanbanstatusservice;
        private string _kanbantitleA;
        private string _kanbantitleB;
        private string _kanbantitleC;
        private string _kanbantitleD;
        private string _kanbantitleE;
        private string _kanbantitleF;
        private string _kanbantitleG;
        private string _kanbantitleH;
        private string _kanbantitleI;
        private MachineStatusService _machinestatusservice;
        private SnackbarMessageQueue _message;
        private int _ngqty;
        private int _sampleqty;
        private HSDKanbanService _sqliteKanbanservice;
        private int _targetvalue;
        private string _type = "首检";
        private CountStatus initData;
        public DelegateCommand AddNG { get; set; }

        public DelegateCommand AddSample { get; set; }

        public DelegateCommand ChangeMaterial { get; set; }

        public ObservableCollection<StringDouble> DieParasBlue
        {
            get { return _dieparablue; }
            set { _dieparablue = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<StringDouble> DieParasBrown
        {
            get { return _dieparabrown; }
            set { _dieparabrown = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<StringDouble> DieParasGreen
        {
            get { return _dieparagreen; }
            set { _dieparagreen = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<StringDouble> DieParasOrange
        {
            get { return _dieparasorange; }
            set { _dieparasorange = value; RaisePropertyChanged(); }
        }

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

        public bool IsBendingOK
        {
            get { return _isbendingOK; }
            set { _isbendingOK = value; RaisePropertyChanged(); }
        }

        public bool IsSaveBtnEnable
        {
            get { return _issavebtnenable; }
            set { _issavebtnenable = value; RaisePropertyChanged(); }
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

        public bool IsVisualOK
        {
            get { return _isvisualOK; }
            set { _isvisualOK = value; RaisePropertyChanged(); }
        }

        public string Kanban
        {
            get { return _kanban; }
            set { _kanban = value; RaisePropertyChanged(); }
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

        public SnackbarMessageQueue Message
        {
            get { return _message; }
            set { _message = value; RaisePropertyChanged(); }
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

        public DelegateCommand SaveCommand { get; set; }

        public DelegateCommand StartSample { get; set; }

        public DelegateCommand SubtractNG { get; set; }

        public DelegateCommand SubtractSample { get; set; }

        public int TargetValue
        {
            get { return _targetvalue; }
            set { _targetvalue = value; RaisePropertyChanged(); UpdateQty(); }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; RaisePropertyChanged(); }
        }

        #endregion 属性定义
    }
}