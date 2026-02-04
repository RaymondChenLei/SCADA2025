using MaterialDesignThemes.Wpf;
using SCADA.Manager;
using SCADA.Models;
using SCADA.Service.Helper;
using SCADA.Service.Models;
using SCADA.Service.SqlServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Text.Json;
using NLog;

namespace SCADA.ViewModels.HomePages
{
    public class DailyCheckPageViewModel : BindableBase
    {
        public DailyCheckPageViewModel()
        {
            _dailycheckcontentservice = new(SQLiteService.Instance.Db);
            _dailycheckrecordservice = new(SQLiteService.Instance.Db);
            _dailycheckreviewservice = new(SQLiteService.Instance.Db);
            Message = new();
            ContentList = [];
            contents = [];
            LoadContent();
            CheckImage = new DelegateCommand<DailyCheckListView>(LoadImage);
            Save = new DelegateCommand(SaveRecord);
            Submit = new DelegateCommand(SubmitRecord);
            StartTiming = new(StartDailyCheck);
        }

        private void StartDailyCheck()
        {
            TimingHelper timing = new();
            timing.TimingSetting(2, out string timingcatagory);
        }

        private void LoadContent()
        {
            ContentList.Clear();
            contents = _dailycheckcontentservice.GetApprovedContent(GlobalSettings.Instance.ProductNo);
            if (contents.Count() == 0)
            {
                Task.Factory.StartNew(() => Message.Enqueue("未找到已批准的" + type + "检查表"));
                return;
            }
            else
            {
                EquipmentNo = GlobalSettings.Instance.ProductNo;
                Remark = contents
                    .Where(x => x.ItemID == 0)
                    .Where(x => x.CheckListID != null)
                    .Select(x => x.CheckContent).ToList().FirstOrDefault();
                MINo = contents.Where(x => x.MIID != null).FirstOrDefault().MIID;
                DocNo = contents.Where(x => x.CheckListID != null).FirstOrDefault().CheckListID;
                foreach (var item in contents.Where(x => x.ItemID != 0))
                {
                    DailyCheckListView listview = new()
                    {
                        IsOK = false,
                        Content = item.CheckContent,
                        Method = item.CheckMethod,
                        ID = item.ItemID,
                        Standard = item.CheckStandard
                    };
                    ContentList.Add(listview);
                }
            }
        }

        private void LoadImage(DailyCheckListView view)
        {
            try
            {
                DailyCheckContent value;
                var values = contents
                    .Where(x => x.ItemID != 0)
                    .Where(x => x.ItemID == view.ID)
                    .ToList();
                if (values.Any())
                {
                    value = values.FirstOrDefault();
                }
                else
                {
                    value = new();
                }
                if (value.ImageData != null)
                {
                    BitmapImage bitmapImage = new();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = new MemoryStream(value.ImageData);
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    ImageSource = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
                _logger.Error(ex, "点检保存失败！");
            }
        }

        private void SaveRecord()
        {
            try
            {
                if (ContentList.Count > 0)
                {
                    List<DailyCheckRecord> records = [];
                    GlobalSettings.Instance.CurrentUserId = "3334";
                    GlobalSettings.Instance.ShiftDate = DateTime.Today;
                    GlobalSettings.Instance.CurrentUserName = "陈磊";
                    foreach (var item in ContentList)
                    {
                        DailyCheckRecord record = new()
                        {
                            CreateUserID = GlobalSettings.Instance.CurrentUserId,
                            DeviceID = GlobalSettings.Instance.ProductNo,
                            ShiftDate = GlobalSettings.Instance.ShiftDate,
                            CheckName = contents.FirstOrDefault().CheckListName,
                            Remark = Problem,
                            IsOK = item.IsOK,
                            IsSubmit = false,
                            Value = item.Value,
                            UserName = GlobalSettings.Instance.CurrentUserName,
                            CreateTime = DateTime.Now
                        };
                        records.Add(record);
                    }
                    _dailycheckrecordservice.InsertRecord(records, out long id);
                    GlobalSettings.Instance.CheckID = id;
                    Task.Factory.StartNew(() => Message.Enqueue("已保存！"));
                }
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() => Message.Enqueue(ex.Message));
                _logger.Error(ex, "点检保存失败！");
            }
        }

        private void SubmitRecord()
        {
            if (ContentList.Count > 0)
            {
                List<DailyCheckRecord> records = [];
                List<DailyCheckReview> reviews = [];
                GlobalSettings.Instance.CurrentUserId = "3334";
                GlobalSettings.Instance.ShiftDate = DateTime.Today;
                GlobalSettings.Instance.CurrentUserName = "陈磊";
                GlobalSettings.Instance.Shift = "A班";
                foreach (var item in ContentList)
                {
                    DailyCheckRecord record = new()
                    {
                        CreateUserID = GlobalSettings.Instance.CurrentUserId,
                        DeviceID = GlobalSettings.Instance.ProductNo,
                        ShiftDate = GlobalSettings.Instance.ShiftDate,
                        CheckName = contents.FirstOrDefault().CheckListName,
                        Remark = Problem,
                        IsOK = item.IsOK,
                        IsSubmit = true,
                        Value = item.Value,
                        UserName = GlobalSettings.Instance.CurrentUserName,
                        CreateTime = DateTime.Now
                    };
                    records.Add(record);
                }
                _dailycheckrecordservice.InsertRecord(records, out long id);
                GlobalSettings.Instance.CheckID = id;
                string ProblemJson = "";
                if (!string.IsNullOrWhiteSpace(Problem))
                {
                    RemarkLogs log = new()
                    {
                        Name = GlobalSettings.Instance.CurrentUserName,
                        UserID = GlobalSettings.Instance.CurrentUserId,
                        Role = "Operator",
                        Remark = Problem,
                        Date = DateTime.Now
                    };
                    List<RemarkLogs> logs = [];
                    logs.Add(log);
                    var options = new JsonSerializerOptions
                    {
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = true
                    };
                    ProblemJson = JsonSerializer.Serialize(logs, options);
                }
                DailyCheckReview review = new()
                {
                    SID = id,
                    Shift = GlobalSettings.Instance.Shift,
                    OperatorDate = DateTime.Now,
                    OperatorID = GlobalSettings.Instance.CurrentUserId,
                    OperatorName = GlobalSettings.Instance.CurrentUserName,
                    Remark = ProblemJson
                };
                _dailycheckreviewservice.InsertRecord(review);

                Task.Factory.StartNew(() => Message.Enqueue("已提交！"));
            }
        }

        #region 属性定义

        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();
        private ObservableCollection<DailyCheckListView> _contentlist;
        private DailyCheckContentService _dailycheckcontentservice;
        private DailyCheckRecordService _dailycheckrecordservice;
        private DailyCheckReviewService _dailycheckreviewservice;
        private string _DocNo;
        private string _EquipmentNo;
        private ImageSource _imagesource;
        private bool _issubmitbtnenable = true;
        private SnackbarMessageQueue _message;
        private string _MINo;
        private string _problem;
        private string _remark;
        private string _submitcontent = "提交";
        private List<DailyCheckContent> contents;
        private string type;
        public DelegateCommand<DailyCheckListView> CheckImage { get; set; }

        public ObservableCollection<DailyCheckListView> ContentList
        {
            get { return _contentlist; }
            set { _contentlist = value; RaisePropertyChanged(); }
        }

        public string DocNo
        {
            get { return _DocNo; }
            set { _DocNo = value; RaisePropertyChanged(); }
        }

        public string EquipmentNo
        {
            get { return _EquipmentNo; }
            set { _EquipmentNo = value; RaisePropertyChanged(); }
        }

        public ImageSource ImageSource
        {
            get { return _imagesource; }
            set { _imagesource = value; RaisePropertyChanged(); }
        }

        public bool IsSubmitBtnEnable
        {
            get { return _issubmitbtnenable; }
            set { _issubmitbtnenable = value; RaisePropertyChanged(); }
        }

        public SnackbarMessageQueue Message
        {
            get { return _message; }
            set { _message = value; RaisePropertyChanged(); }
        }

        public string MINo
        {
            get { return _MINo; }
            set { _MINo = value; RaisePropertyChanged(); }
        }

        public string Problem
        {
            get { return _problem; }
            set { _problem = value; RaisePropertyChanged(); }
        }

        public string Remark
        {
            get { return _remark; }
            set { _remark = value; RaisePropertyChanged(); }
        }

        public DelegateCommand Save { get; set; }
        public DelegateCommand StartTiming { get; set; }
        public DelegateCommand Submit { get; set; }

        public string SubmitContent
        {
            get { return _submitcontent; }
            set { _submitcontent = value; RaisePropertyChanged(); }
        }

        #endregion 属性定义
    }
}