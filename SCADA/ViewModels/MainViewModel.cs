using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using SCADA.Events;
using SCADA.Interface;
using SCADA.Manager;
using SCADA.Models;
using SCADA.Service.Helper;
using SCADA.Service.SqlServer;
using SCADA.Service.SqlServer.Timing;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Windows;
using System.Windows.Threading;

namespace SCADA.ViewModels
{
    public class MainViewModel : BindableBase, IConfigureInitialization
    {
        public MainViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            Message = new();
            _machineStatusService = new(SQLiteService.Instance.Db);
            _stopcatalogservice = new(SQLiteService.Instance.Db);
            _SQLServerstopservice = new(SqlService.Instance.Client);
            _sqlserverkanbanService = new(SqlService.Instance.Client);
            _sqlitekanbanService = new(SQLiteService.Instance.Db);
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<CounterZeroizeEvent>().Subscribe(c => Counter = c);
            _getportshelper = new GetPorts();
            serialPort = new SerialPort
            {
                ReceivedBytesThreshold = 8 // 确保有完整数据包才触发
            };
            serialPort.DataReceived += SerialPort_DataReceived;
            timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += Timer_Tick;
            Configure();
            _ = StartStatusCheckLoopAsync();
        }

        public void Configure()
        {
            OpenUpdateNotice = new(UpdateInfoDialog);
            GetAppSetting();
            SynchronizeData();
            MenuBars = [];
            GetMenuBar();
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            Communication();
            CardReaderCommunication();
            ScannerCommunication();
            InitTiming();
            Equipment = GlobalSettings.Instance.ProductNo;
        }

        private static string GetSerialPortFriendlyName(string portName)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    $"SELECT Caption FROM Win32_SerialPort WHERE DeviceID LIKE '%{portName}%'")
                )
                {
                    foreach (ManagementObject portInfo in searcher.Get())
                    {
                        return portInfo["Caption"].ToString();
                    }
                }
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    $"SELECT Caption FROM Win32_PnPEntity WHERE Name LIKE '%{portName}%'")
                )
                {
                    foreach (ManagementObject pnpDevice in searcher.Get())
                    {
                        return pnpDevice["Caption"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                return "查询失败";
            }
            return "未找到设备信息";
        }

        private int Bit(byte Data0, int temp)
        {
            int[] bittemp = new int[8];
            int i, Data = Data0;

            for (i = 0; i <= temp; i++)
            {
                if (Data % 2 == 1)//判断第一位是0或1
                    bittemp[i] = 1;
                else
                    bittemp[i] = 0;

                Data = (int)(Data / 2);
            }
            return bittemp[temp];
        }

        private void CardReaderCommunication()
        {
            try
            {
                GetSerialPortInfo();
                if (_cardreadercominfo == null)
                {
                    return;
                }
                else
                {
                    if (!_cardreaderPort.IsOpen)
                    {
                        _cardreaderPort = new(_cardreadercominfo, 9600, Parity.None, 8, StopBits.One)
                        {
                            DtrEnable = true,
                            RtsEnable = true,
                            ReceivedBytesThreshold = 1
                        };
                        _cardreaderPort.Open();
                        CardReaderComStatus = "连接成功";
                        _cardreaderPort.DataReceived += CardReaderPort_DataReceived;
                    }
                }
            }
            catch (Exception)
            {
                CardReaderComStatus = "连接错误";
                return;
            }
        }

        private void CardReaderPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(CardReaderPortRead));
            System.Threading.Thread.Sleep(150);
        }

        private void CardReaderPortRead()
        {
            string receivedData = _cardreaderPort.ReadExisting();
            ToLogin login = new();
            login.Login(receivedData, out string UserName, out string UserID);
            GlobalSettings.Instance.CurrentUserId = UserID;
            GlobalSettings.Instance.CurrentUserName = UserName;
            Name = UserName;
            ID = UserID;
            Task.Factory.StartNew(() => Message.Enqueue("登录成功！"));
            GlobalSettings.Instance.IsNeedDailyCheck = login.IfNeedDailyCheck();
            if (GlobalSettings.Instance.IsNeedDailyCheck)
            {
                Task.Factory.StartNew(() => Message.Enqueue("请完成点检！"));
                //还要扫描
            }
        }

        private void ChangeButtonKind()
        {
            if (MainWindowsState == WindowState.Maximized)
            {
                MaxButtonKind = PackIconKind.WindowRestore;
            }
            else
            {
                MaxButtonKind = PackIconKind.Maximize;
            }
        }

        private void Communication()
        {
            var ports = _getportshelper.GetPortsWithDescription();
            if (ports.Count > 0)
            {
                var port = GlobalSettings.Instance.COMPort;
                if (!serialPort.IsOpen)
                {
                    if (port != null)
                    {
                        serialPort.PortName = port;
                        serialPort.BaudRate = 9600;
                        serialPort.DataBits = 8;
                        serialPort.StopBits = (StopBits)1;
                        serialPort.Parity = Parity.None;
                        serialPort.DtrEnable = true;
                        serialPort.RtsEnable = true;
                        try
                        {
                            serialPort.Open();
                            ComStatus = "已连接";
                            timer.Start();
                        }
                        catch (Exception)
                        {
                            ComStatus = "连接错误";
                            return;
                        }
                        ComInfo = port;
                    }
                    else
                    {
                        ComStatus = "未连接";
                    }
                }
                else
                {
                    ComStatus = "已连接";
                }
            }
            else
            {
                ComStatus = "未连接";
            }
        }

        private void DataCheck()
        {
            if (Data != null)
            {
                if (Data[0] == 0) return;
                if (Data[2] == 0x10)
                {
                    if (Bit((Data[4]), 0) == 1)
                    {
                        Action actionYES = () =>
                        {
                            SensorStatus = "有信号";
                            if (previousIOStatus != "有信号")
                            {
                                Counter++;
                                var nowstopID = _machineStatusService.GetStatus().StopID;
                                var repireID = _stopcatalogservice.GetStopID("维修");
                                TimingHelper timing = new();
                                if (nowstopID != repireID)
                                {
                                    timing.TimingSetting(1, out string timingcatagory);
                                    TimingCatagory = timingcatagory;
                                }
                            }
                            previousIOStatus = "有信号";
                        };
                        Dispatcher dispatcherYES = Application.Current.Dispatcher;
                        dispatcherYES.Invoke(actionYES);
                    }
                    else if (Bit((Data[4]), 0) == 0)
                    {
                        Action actioNO = () =>
                        {
                            SensorStatus = "无信号";
                            previousIOStatus = "无信号";
                        };
                        Dispatcher dispatcherNO = Application.Current.Dispatcher;
                        dispatcherNO.Invoke(actioNO);
                    }
                    else
                    {
                        Action actioX = () =>
                        {
                            SensorStatus = "未知";
                        };
                        Dispatcher dispatcherX = Application.Current.Dispatcher;
                        dispatcherX.Invoke(actioX);
                    }
                }
            }
        }

        private void GetAppSetting()
        {
            LocalSetting localSetting = new();
            string jsonFilePath;
            jsonFilePath = localSetting.AppSettingFilename;
            if (!File.Exists(localSetting.AppSettingFilename))
            {
                Config setting = new();
                JsonHelper.WrtToFile(jsonFilePath, setting);
            }
            var appsetting = JsonHelper.GetData<Config>(jsonFilePath);
            try
            {
                GlobalSettings.Instance.ProductNo = appsetting.ProductNo;
                GlobalSettings.Instance.COMPort = appsetting.RelayModulePort;
                _machineStatusService.UpdateStatus(appsetting.ProductNo);
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
            }
        }

        private void GetMenuBar()
        {
            LocalSetting localSetting = new();
            string jsonFilePath;
            jsonFilePath = localSetting.OperatorMenubarFilename;
            if (!File.Exists(localSetting.OperatorMenubarFilename))
            {
                List<MenuBar> initMenubars =
                    [
                    new(){ SelectedIcon=PackIconKind.HomeOutline, UnselectedIcon=PackIconKind.Home, Title="主页", NameSpace="HomeView"},
                    new(){ SelectedIcon=PackIconKind.ClockOutline, UnselectedIcon=PackIconKind.Clock, Title="记录", NameSpace="RecordsView"},
                    new(){ SelectedIcon=PackIconKind.FolderEyeOutline, UnselectedIcon=PackIconKind.FolderEye, Title="工艺", NameSpace="ProcessView"},
                    new(){ SelectedIcon=PackIconKind.ChartBoxOutline, UnselectedIcon=PackIconKind.ChartBox, Title="图表", NameSpace="ChartsView"},
                    new(){ SelectedIcon=PackIconKind.AccountOutline, UnselectedIcon=PackIconKind.Account, Title="设置", NameSpace="SettingView"},
                    ];
                JsonHelper.WrtToFile(jsonFilePath, initMenubars);
            }
            var menubardata = JsonHelper.GetData<ObservableCollection<MenuBar>>(jsonFilePath);
            try
            {
                if (menubardata != null)
                {
                    foreach (var item in menubardata)
                    {
                        MenuBars.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex));
            }
        }

        private void GetSerialPortInfo()
        {
            foreach (string activePort in SerialPort.GetPortNames())
            {
                string friendlyName = GetSerialPortFriendlyName(activePort);
                if (!string.IsNullOrWhiteSpace(friendlyName) && friendlyName.Contains("CH340"))
                {
                    _cardreadercominfo = activePort;
                }
                if (!string.IsNullOrWhiteSpace(friendlyName) && friendlyName.Contains("14XX"))
                {
                    _scannercominfo = activePort;
                }
            }
        }

        private void InitTiming()
        {
            TimingHelper timing = new();
            timing.TimingSetting(0, out string timingcatagory);
            TimingCatagory = timingcatagory;
        }

        private void Navigate(MenuBar bar)
        {
            if (bar == null || string.IsNullOrWhiteSpace(bar.NameSpace))
                return;
            _regionManager.Regions[PrismManager.MainWindowRegionName].RequestNavigate(bar.NameSpace);
        }

        private async Task PortRead()
        {
            try
            {
                Data = new byte[8];
                await Task.Run(() =>
                {
                    if (serialPort.BytesToRead >= 8)
                    {
                        try
                        {
                            serialPort.Read(Data, 0, 8);
                        }
                        catch (Exception ex)
                        {
                            Task.Factory.StartNew(() => Message.Enqueue(ex));
                        }
                    }
                });

                if ((Data[0] + Data[1] + Data[2] + Data[3] + Data[4] + Data[5] + Data[6]) % 256 == Data[7] && Data[0] == 0x22)
                {
                    Interlocked.Decrement(ref noReplyCounts);
                    if (Interlocked.Read(ref noReplyCounts) == -2)
                        Interlocked.Exchange(ref noReplyCounts, 0);
                }
                else
                {
                    Data = new byte[8];
                }
            }
            catch (Exception ex)
            {
                Data = new byte[8];
                Message.Enqueue($"数据读取错误: {ex.Message}");
            }
        }

        private void SannerPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(ScannerPortRead));
            System.Threading.Thread.Sleep(150);
        }

        private void ScannerCommunication()
        {
            try
            {
                GetSerialPortInfo();
                if (_scannercominfo == null)
                {
                    return;
                }
                else
                {
                    if (!_scannerPort.IsOpen)
                    {
                        _scannerPort = new(_scannercominfo, 9600, Parity.None, 8, StopBits.One)
                        {
                            DtrEnable = true,
                            RtsEnable = true,
                            ReceivedBytesThreshold = 1
                        };
                        _scannerPort.Open();
                        ScannerComStatus = "连接成功";
                        _scannerPort.DataReceived += SannerPort_DataReceived;
                    }
                }
            }
            catch (Exception)
            {
                ScannerComStatus = "连接错误";
                return;
            }
        }

        private void ScannerPortRead()
        {
            string receivedData = _scannerPort.ReadExisting();
            string InitialLetter = receivedData.Substring(0, 1);
            if (InitialLetter == "L" || InitialLetter == "Y")
            {
                if (receivedData == GlobalSettings.Instance.KB)
                {
                    Task.Factory.StartNew(() => Message.Enqueue("扫描看板与当前看板一致！"));
                }
                else
                {
                    GlobalSettings.Instance.KB = receivedData;
                    _eventAggregator.GetEvent<KBChangeEvent>().Publish(receivedData);
                }
            }
            else if (InitialLetter == "S")
            {
                _eventAggregator.GetEvent<ScanTerminalEvent>().Publish(receivedData);
            }
            else if (InitialLetter == "J")
            {
                _eventAggregator.GetEvent<ScanDieEvent>().Publish(receivedData);
            }
        }

        private async void SendBuff(byte leixing, byte data1, byte data2, byte data3, byte data4)
        {
            if (Interlocked.Read(ref noReplyCounts) > -1)
                Interlocked.Increment(ref noReplyCounts);
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (serialPort.IsOpen)
                {
                    try
                    {
                        List<byte> Byteout = [];
                        int sum;
                        Byteout.Add(0x55);
                        Byteout.Add(1);
                        Byteout.Add(leixing);
                        Byteout.Add(data1);
                        Byteout.Add(data2);
                        Byteout.Add(data3);
                        Byteout.Add(data4);
                        sum = 0;
                        for (int i = 0; i <= 6; i++)
                            sum += Byteout[i];

                        Byteout.Add(Convert.ToByte(sum % 256));
                        await Task.Run(() =>
                        {
                            serialPort.DiscardInBuffer();
                            serialPort.Write(Byteout.ToArray(), 0, Byteout.Count);
                        });
                    }
                    catch (Exception ex)
                    {
                        Message.Enqueue($"发送失败: {ex.Message}");
                    }
                }
            });
        }

        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await Task.Run(() => PortRead()); // 在后台线程执行读取
                });
            }
            catch (Exception ex)
            {
                Message.Enqueue($"串口读取错误: {ex.Message}");
            }
        }

        private async Task StartStatusCheckLoopAsync()
        {
            while (!_disposed)  // 需要添加类级别的私有字段 bool _disposed
            {
                try
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        ComStatus = serialPort.IsOpen ? "已连接" : "未连接";
                        UpdateDateTime();

                        //只有当未连接时才尝试自动连接

                        if (!serialPort.IsOpen)
                        {
                            Communication();
                        }
                        if (_cardreadercominfo == null)
                        {
                            CardReaderComStatus = "未发现硬件";
                        }
                        else
                        {
                            CardReaderComStatus = _cardreaderPort.IsOpen ? "已连接" : "未连接";
                            if (!_cardreaderPort.IsOpen)
                            {
                                CardReaderCommunication();
                            }
                            ScannerComStatus = _scannerPort.IsOpen ? "已连接" : "未连接";
                            if (!_scannerPort.IsOpen)
                            {
                                ScannerCommunication();
                            }
                        }
                    });
                    await Task.Delay(1000);  // 每秒检查一次
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"状态检查异常: {ex.Message}");

                    // 防止异常导致循环退出，延迟后继续
                    await Task.Delay(5000);
                }
            }
        }

        private void SynchronizeData()
        {
            var sqldata = _SQLServerstopservice.GetAllData();
            var sqlkabban = _sqlserverkanbanService.GetKanban(GlobalSettings.Instance.ProductNo);
            _stopcatalogservice.UpdataAllData(sqldata);
            _sqlitekanbanService.UpdataAllKanban(sqlkabban);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                DataCheck();
                SendBuff(0x10, 0, 0, 0, 0);
            }
            if (Interlocked.Read(ref noReplyCounts) >= 10)
            {
                Interlocked.Exchange(ref noReplyCounts, -1);
            }
        }

        private void UpdateDateTime()
        {
            TikClock = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var status = _machineStatusService.GetStatus();
            var name = _stopcatalogservice.GetStopName(status.StopID);
            var time = status.StartTime;
            TimeSpan timeDiff = DateTime.Now - time;
            Time = $"{timeDiff.Minutes}分{timeDiff.Seconds}秒";
            TimingCatagory = name;
            if (status.StopID == 1 && timeDiff.TotalSeconds >= 10)
            {
                InitTiming();
            }
        }

        private void UpdateInfoDialog()
        {
            string message =
                "版本号：1.2.25.2"
                + Environment.NewLine +
                "更新说明："
                + Environment.NewLine +
                "1. 完善了计数功能；"
                + Environment.NewLine +
                "版本号：1.2.25.3"
                + Environment.NewLine +
                "更新说明："
                + Environment.NewLine +
                "1. 完善了计时功能；";
            MessageBox.Show(message, "更新公告", MessageBoxButton.OK);
        }

        #region 属性定义

        private static long noReplyCounts = 0;
        private readonly CancellationTokenSource _cts = new();
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private string _cardreadercominfo;
        private string _cardreadercomstatus;
        private SerialPort _cardreaderPort = new();
        private string _cominfo;
        private string _comstatus;
        private int _counter;
        private bool _disposed = false;
        private string _equipment;
        private GetPorts _getportshelper;
        private string _id;
        private MachineStatusService _machineStatusService;
        private WindowState _mainwindowsstate;
        private PackIconKind _maxbuttonkind = PackIconKind.Maximize;
        private ObservableCollection<MenuBar> _menubars;
        private SnackbarMessageQueue _message;
        private string _name;
        private string _scannercominfo;
        private string _scannercomstatus;
        private SerialPort _scannerPort = new();
        private string _sensorstatus;
        private HSDKanbanService _sqlitekanbanService;
        private HSDKanbanService _sqlserverkanbanService;
        private StopCatelogService _SQLServerstopservice;
        private StopCatelogService _stopcatalogservice;
        private string _tikclock;
        private string _time;
        private string _timingcatagory;
        private byte[] Data = null;
        private string previousIOStatus;
        private SerialPort serialPort = new();
        private DispatcherTimer timer;

        public string CardReaderComInfo
        {
            get { return _cardreadercominfo; }
            set { _cardreadercominfo = value; RaisePropertyChanged(); }
        }

        public string CardReaderComStatus
        {
            get { return _cardreadercomstatus; }
            set { _cardreadercomstatus = value; RaisePropertyChanged(); }
        }

        public string ComInfo
        {
            get { return _cominfo; }
            set { _cominfo = value; RaisePropertyChanged(); }
        }

        public string ComStatus
        {
            get { return _comstatus; }
            set { _comstatus = value; RaisePropertyChanged(); }
        }

        public int Counter
        {
            get { return _counter; }
            set
            {
                if (_counter != value)
                {
                    _counter = value;
                    RaisePropertyChanged(nameof(Counter));
                    _eventAggregator.GetEvent<TextUpdatedEvent>().Publish(value);
                }
            }
        }

        public string Equipment
        {
            get { return _equipment; }
            set { _equipment = value; RaisePropertyChanged(); }
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged(); }
        }

        public WindowState MainWindowsState
        {
            get { return _mainwindowsstate; }
            set { _mainwindowsstate = value; RaisePropertyChanged(); ChangeButtonKind(); }
        }

        public PackIconKind MaxButtonKind
        {
            get { return _maxbuttonkind; }
            set { _maxbuttonkind = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<MenuBar> MenuBars
        {
            get { return _menubars; }
            set { _menubars = value; RaisePropertyChanged(); }
        }

        public SnackbarMessageQueue Message
        {
            get { return _message; }
            set { _message = value; RaisePropertyChanged(); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<MenuBar> NavigateCommand { get; set; }
        public DelegateCommand OpenUpdateNotice { get; set; }

        public string ScannerComInfo
        {
            get { return _scannercominfo; }
            set { _scannercominfo = value; RaisePropertyChanged(); }
        }

        public string ScannerComStatus
        {
            get { return _scannercomstatus; }
            set { _scannercomstatus = value; RaisePropertyChanged(); }
        }

        public string SensorStatus
        {
            get { return _sensorstatus; }
            set { _sensorstatus = value; RaisePropertyChanged(); }
        }

        public string TikClock
        {
            get { return _tikclock; }
            set { _tikclock = value; RaisePropertyChanged(); }
        }

        public string Time
        {
            get { return _time; }
            set { _time = value; RaisePropertyChanged(); }
        }

        public string TimingCatagory
        {
            get { return _timingcatagory; }
            set { _timingcatagory = value; RaisePropertyChanged(); _eventAggregator.GetEvent<OnTimingCategoryChanged>().Publish(value); }
        }

        private CancellationToken _cancellationToken => _cts.Token;

        #endregion 属性定义
    }
}