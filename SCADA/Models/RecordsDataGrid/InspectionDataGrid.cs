using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Models.RecordsDataGrid
{
    public class InspectionDataGrid : InspectionHSDJson
    {
        public Int64 序号 { get; set; }
        public string 设备 { get; set; }
        public string 看板号 { get; set; }
        public string 检测次序 { get; set; }
        public string 工号 { get; set; }
        public string 开始时间 { get; set; }
        public string 结束时间 { get; set; }
        public float 持续时间 { get; set; }
        public DateTime 班次日期 { get; set; }
        public bool 外观 { get; set; }
        public bool 弯曲 { get; set; }
        public string 模具1 { get; set; }
        public string 模具2 { get; set; }
        public string 模具3 { get; set; }
    }
}