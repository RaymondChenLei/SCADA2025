using MaterialDesignThemes.Wpf;
using NLog.Targets;
using SCADA.Events;
using SCADA.Interface;
using SCADA.Manager;
using SCADA.Service.Helper;
using SCADA.Service.Models.Count;
using SCADA.Service.SqlServer;
using SCADA.Service.SqlServer.Count;
using System.Runtime.ConstrainedExecution;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows;

namespace SCADA.ViewModels.Dialogs
{
    public class ScanDialogViewModel : BindableBase, IDialogHostAware
    {
        public ScanDialogViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ScanDieEvent>().Subscribe(die => DieCodeScanned(die));
            eventAggregator.GetEvent<ScanTerminalEvent>().Subscribe(ter => TerminalCodeScanned(ter));
            _kanbanStatusService = new(SQLiteService.Instance.Db);
            CloseCommand = new(CloseDialog);
            SetImageVis(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        private void SetImageVis(int targetD, int targetE, int targetF, int targetG, int targetH, int targetI, int checkD, int checkE, int checkF, int checkG, int checkH, int checkI, int crossD, int crossE, int crossF, int crossG, int crossH, int crossI)
        {
            TargetDVis = targetD == 1 ? Visibility.Visible : Visibility.Hidden;
            TargetEVis = targetE == 1 ? Visibility.Visible : Visibility.Hidden;
            TargetFVis = targetF == 1 ? Visibility.Visible : Visibility.Hidden;
            TargetGVis = targetG == 1 ? Visibility.Visible : Visibility.Hidden;
            TargetHVis = targetH == 1 ? Visibility.Visible : Visibility.Hidden;
            TargetIVis = targetI == 1 ? Visibility.Visible : Visibility.Hidden;
            CheckDVis = checkD == 1 ? Visibility.Visible : Visibility.Hidden;
            CheckEVis = checkE == 1 ? Visibility.Visible : Visibility.Hidden;
            CheckFVis = checkF == 1 ? Visibility.Visible : Visibility.Hidden;
            CheckGVis = checkG == 1 ? Visibility.Visible : Visibility.Hidden;
            CheckHVis = checkH == 1 ? Visibility.Visible : Visibility.Hidden;
            CheckIVis = checkI == 1 ? Visibility.Visible : Visibility.Hidden;
            CrossDVis = crossD == 1 ? Visibility.Visible : Visibility.Hidden;
            CrossEVis = crossE == 1 ? Visibility.Visible : Visibility.Hidden;
            CrossFVis = crossF == 1 ? Visibility.Visible : Visibility.Hidden;
            CrossGVis = crossG == 1 ? Visibility.Visible : Visibility.Hidden;
            CrossHVis = crossH == 1 ? Visibility.Visible : Visibility.Hidden;
            CrossIVis = crossI == 1 ? Visibility.Visible : Visibility.Hidden;
        }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            var equipment = GlobalSettings.Instance.ProductNo;
            nameD = parameters.GetValue<string>("MaterialDName");
            nameE = parameters.GetValue<string>("MaterialEName");
            nameF = parameters.GetValue<string>("MaterialFName");
            nameG = parameters.GetValue<string>("MaterialGName");
            nameH = parameters.GetValue<string>("MaterialHName");
            nameI = parameters.GetValue<string>("MaterialIName");
            targetD = parameters.GetValue<string>("MaterialDTarget");
            targetE = parameters.GetValue<string>("MaterialETarget");
            targetF = parameters.GetValue<string>("MaterialFTarget");
            targetG = parameters.GetValue<string>("MaterialGTarget");
            targetH = parameters.GetValue<string>("MaterialHTarget");
            targetI = parameters.GetValue<string>("MaterialITarget");
            switch (equipment)
            {
                case "HSD410-2":
                    BackgroundImage = "/Views/Image/H4102.jpg";
                    SetH4102Parameter(nameD, nameE, nameF, nameG, nameH, nameI, targetD, targetE, targetF, targetG, targetH, targetI);
                    Speak($"请扫描{nameD}");
                    break;

                default:
                    break;
            }
        }

        private void Speak(string words)
        {
            var t = new Thread(() => { synth.Speak(words); });
            t.Start();
        }

        private void CloseDialog()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No));
        }

        private void DieCodeScanned(string die)
        {
            switch (_scanSequence)
            {
                case "D":
                    if (!string.IsNullOrWhiteSpace(TargetE))
                    {
                        if (TargetD == die)
                        {
                            ScannedD = die;
                            _scanSequence = "E";
                            SetImageVis(0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                            Speak($"{nameD}扫描成功！请扫描{nameE}");
                        }
                        else
                        {
                            ScannedD = die;
                            SetImageVis(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
                            Speak($"{nameD}扫描错误！");
                        }
                    }
                    break;

                case "E":
                    break;

                case "F":
                    if (!string.IsNullOrWhiteSpace(TargetF))
                    {
                        if (TargetF == die)
                        {
                            ScannedF = die;
                            _scanSequence = "G";
                            Speak($"{nameF}扫描成功！请扫描{nameG}");
                            SetImageVis(0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                        }
                        else
                        {
                            ScannedF = die;
                            SetImageVis(0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
                            Speak($"{nameF}扫描错误！");
                        }
                    }
                    break;

                case "G":
                    break;

                case "H":
                    break;

                case "I":
                    break;
            }
        }

        private void SetH4102Parameter(string nameD, string nameE, string nameF, string nameG, string nameH, string nameI, string targetD, string targetE, string targetF, string targetG, string targetH, string targetI)
        {
            NameD = nameD;
            NameE = nameE;
            NameF = nameF;
            NameG = nameG;
            TargetD = targetD;
            TargetE = targetE;
            TargetF = targetF;
            TargetG = targetG;
        }

        private async Task TerminalCodeScanned(string ter)
        {
            APIInterfaceHelper api = new();
            switch (_scanSequence)
            {
                case "D":
                    break;

                case "E":
                    if (!string.IsNullOrWhiteSpace(TargetE))
                    {
                        if (TargetE == await api.CallGetRemoteServerCodeAsync(ter))
                        {
                            ScannedE = ter;
                            _scanSequence = "F";
                            SetImageVis(0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                            Speak($"{nameE}扫描成功！请扫描{nameF}");
                        }
                        else
                        {
                            ScannedE = ter;
                            SetImageVis(0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
                            Speak($"{nameE}扫描错误！");
                        }
                    }
                    break;

                case "F":
                    break;

                case "G":
                    if (!string.IsNullOrWhiteSpace(TargetG))
                    {
                        if (TargetG == await api.CallGetRemoteServerCodeAsync(ter))
                        {
                            Speak($"{nameG}扫描成功！扫描完成！");
                            KanbanStatus data = new()
                            {
                                KB = GlobalSettings.Instance.KB,
                                MaterialD = targetD,
                                MaterialE = targetE,
                                MaterialF = targetF,
                                MaterialG = targetG,
                                MaterialH = targetH,
                                MaterialI = targetI,
                                Shift = GlobalSettings.Instance.Shift,
                                ShiftDate = GlobalSettings.Instance.ShiftDate,
                                ScanDone = true
                            };
                            _kanbanStatusService.UpdateKBStatus(data);
                            CloseDialog();
                        }
                        else
                        {
                            ScannedG = ter;
                            SetImageVis(0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0);
                            Speak($"{nameG}扫描错误！");
                        }
                    }
                    break;

                case "H":
                    break;

                case "I":
                    break;
            }
        }

        #region 属性定义

        private SpeechSynthesizer synth = new();
        private string nameD;
        private string nameE;
        private string nameF;
        private string nameG;
        private string nameH;
        private string nameI;
        private string targetD;
        private string targetE;
        private string targetF;
        private string targetG;
        private string targetH;
        private string targetI;
        private string _backgroundImage;
        private Visibility _checkDvis = Visibility.Hidden;
        private Visibility _checkEvis = Visibility.Hidden;
        private Visibility _checkFvis = Visibility.Hidden;
        private Visibility _checkGvis = Visibility.Hidden;
        private Visibility _checkHvis = Visibility.Hidden;
        private Visibility _checkIVis = Visibility.Hidden;
        private Visibility _crossDvis = Visibility.Hidden;
        private Visibility _crossEvis = Visibility.Hidden;
        private Visibility _crossFvis = Visibility.Hidden;
        private Visibility _crossGvis = Visibility.Hidden;
        private Visibility _crossHvis = Visibility.Hidden;
        private Visibility _crossIVis = Visibility.Hidden;
        private bool _D_OK;
        private bool _E_OK;
        private bool _F_OK;
        private bool _G_OK;
        private bool _H_OK;
        private bool _I_OK;
        private string _nameD;
        private string _nameE;
        private string _nameF;
        private string _nameG;
        private string _nameH;
        private string _nameI;
        private string _scanDieCode;
        private string _scannedD;
        private string _scannedE;
        private string _scannedF;
        private string _scannedG;
        private string _scannedH;
        private string _scannedI;
        private string _scanSequence = "D";
        private string _scanTerminalCode;
        private string _targetD;
        private Visibility _targetDvis = Visibility.Hidden;
        private string _targetE;
        private Visibility _targetEvis = Visibility.Hidden;
        private string _targetF;
        private Visibility _targetFvis = Visibility.Hidden;
        private string _targetG;
        private Visibility _targetGvis = Visibility.Hidden;
        private string _targetH;
        private Visibility _targetHvis = Visibility.Hidden;
        private string _targetI;
        private Visibility _targetIVis = Visibility.Hidden;
        private KanbanStatusService _kanbanStatusService;

        public string BackgroundImage
        {
            get { return _backgroundImage; }
            set { _backgroundImage = value; RaisePropertyChanged(); }
        }

        public DelegateCommand CancelCommand { get; set; }

        public Visibility CheckDVis
        {
            get { return _checkDvis; }
            set { _checkDvis = value; RaisePropertyChanged(); }
        }

        public Visibility CheckEVis
        {
            get { return _checkEvis; }
            set { _checkEvis = value; RaisePropertyChanged(); }
        }

        public Visibility CheckFVis
        {
            get { return _checkFvis; }
            set { _checkFvis = value; RaisePropertyChanged(); }
        }

        public Visibility CheckGVis
        {
            get { return _checkGvis; }
            set { _checkGvis = value; RaisePropertyChanged(); }
        }

        public Visibility CheckHVis
        {
            get { return _checkHvis; }
            set { _checkHvis = value; RaisePropertyChanged(); }
        }

        public Visibility CheckIVis
        {
            get { return _checkIVis; }
            set { _checkIVis = value; RaisePropertyChanged(); }
        }

        public DelegateCommand CloseCommand { get; set; }

        public Visibility CrossDVis
        {
            get { return _crossDvis; }
            set { _crossDvis = value; RaisePropertyChanged(); }
        }

        public Visibility CrossEVis
        {
            get { return _crossEvis; }
            set { _crossEvis = value; RaisePropertyChanged(); }
        }

        public Visibility CrossFVis
        {
            get { return _crossFvis; }
            set { _crossFvis = value; RaisePropertyChanged(); }
        }

        public Visibility CrossGVis
        {
            get { return _crossGvis; }
            set { _crossGvis = value; RaisePropertyChanged(); }
        }

        public Visibility CrossHVis
        {
            get { return _crossHvis; }
            set { _crossHvis = value; RaisePropertyChanged(); }
        }

        public Visibility CrossIVis
        {
            get { return _crossIVis; }
            set { _crossIVis = value; RaisePropertyChanged(); }
        }

        public string DialogHostName { get; set; }

        public string NameD
        {
            get { return _nameD; }
            set { _nameD = value; RaisePropertyChanged(); }
        }

        public string NameE
        {
            get { return _nameE; }
            set { _nameE = value; RaisePropertyChanged(); }
        }

        public string NameF
        {
            get { return _nameF; }
            set { _nameF = value; RaisePropertyChanged(); }
        }

        public string NameG
        {
            get { return _nameG; }
            set { _nameG = value; RaisePropertyChanged(); }
        }

        public string NameH
        {
            get { return _nameH; }
            set { _nameH = value; RaisePropertyChanged(); }
        }

        public string NameI
        {
            get { return _nameI; }
            set { _nameI = value; RaisePropertyChanged(); }
        }

        public DelegateCommand SaveCommand { get; set; }

        public string ScannedD
        {
            get { return _scannedD; }
            set { _scannedD = value; RaisePropertyChanged(); }
        }

        public string ScannedE
        {
            get { return _scannedE; }
            set { _scannedE = value; RaisePropertyChanged(); }
        }

        public string ScannedF
        {
            get { return _scannedF; }
            set { _scannedF = value; RaisePropertyChanged(); }
        }

        public string ScannedG
        {
            get { return _scannedG; }
            set { _scannedG = value; RaisePropertyChanged(); }
        }

        public string ScannedH
        {
            get { return _scannedH; }
            set { _scannedH = value; RaisePropertyChanged(); }
        }

        public string ScannedI
        {
            get { return _scannedI; }
            set { _scannedI = value; RaisePropertyChanged(); }
        }

        public string TargetD
        {
            get { return _targetD; }
            set { _targetD = value; RaisePropertyChanged(); }
        }

        public Visibility TargetDVis
        {
            get { return _targetDvis; }
            set { _targetDvis = value; RaisePropertyChanged(); }
        }

        public string TargetE
        {
            get { return _targetE; }
            set { _targetE = value; RaisePropertyChanged(); }
        }

        public Visibility TargetEVis
        {
            get { return _targetEvis; }
            set { _targetEvis = value; RaisePropertyChanged(); }
        }

        public string TargetF
        {
            get { return _targetF; }
            set { _targetF = value; RaisePropertyChanged(); }
        }

        public Visibility TargetFVis
        {
            get { return _targetFvis; }
            set { _targetFvis = value; RaisePropertyChanged(); }
        }

        public string TargetG
        {
            get { return _targetG; }
            set { _targetG = value; RaisePropertyChanged(); }
        }

        public Visibility TargetGVis
        {
            get { return _targetGvis; }
            set { _targetGvis = value; RaisePropertyChanged(); }
        }

        public string TargetH
        {
            get { return _targetH; }
            set { _targetH = value; RaisePropertyChanged(); }
        }

        public Visibility TargetHVis
        {
            get { return _targetHvis; }
            set { _targetHvis = value; RaisePropertyChanged(); }
        }

        public string TargetI
        {
            get { return _targetI; }
            set { _targetI = value; RaisePropertyChanged(); }
        }

        public Visibility TargetIVis
        {
            get { return _targetIVis; }
            set { _targetIVis = value; RaisePropertyChanged(); }
        }

        #endregion 属性定义
    }
}