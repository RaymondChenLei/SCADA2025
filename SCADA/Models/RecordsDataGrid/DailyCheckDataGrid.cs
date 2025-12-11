using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Models.RecordsDataGrid
{
    public class DailyCheckDataGrid
    {
        public Int64 序号 { get; set; }
        public string 设备 { get; set; }
        public string 点检内容 { get; set; }
        public string 操作者 { get; set; }
        public string 工号 { get; set; }
        public string 数值 { get; set; }
        public bool 是否合格 { get; set; }
        public bool 是否提交 { get; set; }
        public string 创建时间 { get; set; }
        public DateTime 班次日期 { get; set; }
        public string 备注 { get; set; }
    }
}