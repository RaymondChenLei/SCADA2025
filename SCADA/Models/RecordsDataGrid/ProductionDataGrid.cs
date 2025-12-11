using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Models.RecordsDataGrid
{
    public class ProductionDataGrid
    {
        public Int64 序号 { get; set; }
        public string 设备 { get; set; }
        public int 合格数 { get; set; }
        public int 不合格数 { get; set; }
        public int 总数 { get; set; }
        public string 看板号 { get; set; }
        public string 工号 { get; set; }
        public DateTime 班次日期 { get; set; }
        public bool 是否完成 { get; set; }
    }
}