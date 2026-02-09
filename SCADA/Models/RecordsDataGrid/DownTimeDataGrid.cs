using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Models.RecordsDataGrid
{
    public class DownTimeDataGrid
    {
        public long 序号 { get; set; }
        public string 设备 { get; set; }
        public string 设备类别 { get; set; }
        public string 停机分类 { get; set; }
        public string 停机名称 { get; set; }
        public string 开始时间 { get; set; }
        public string 结束时间 { get; set; }
        public float 持续时间 { get; set; }
        public string 工号 { get; set; }
        public string 看板号 { get; set; }
        public DateTime 班次日期 { get; set; }
    }
}