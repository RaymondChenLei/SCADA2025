using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Manager
{
    public class GlobalSettings
    {
        public long CheckID { get; set; } = 0;
        public string CurrentUserId { get; set; } = "default";
        public string CurrentUserName { get; set; } = "默认用户";
        public string ProductNo { get; set; } = "压接机1#";
        public string Shift { get; set; } = "A班";
        public string KB { get; set; } = "";
        public string COMPort { get; set; } = "COM1";
        public DateTime ShiftDate { get; set; } = DateTime.Now;
        public bool IsNeedDailyCheck { get; set; } = true;
        public bool IsNeedScan { get; set; } = true;

        private static readonly object _lock = new();
        private static GlobalSettings _instance;

        public static GlobalSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}