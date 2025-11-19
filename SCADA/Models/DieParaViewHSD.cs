using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Models
{
    public class DieParaViewHSD
    {
        public int Id { get; set; }
        public string 端子号 { get; set; }
        public string 端子类型 { get; set; }
        public string 模具号 { get; set; }
        public string 设备 { get; set; }
        public double 剥皮长度Min { get; set; }
        public double 剥皮长度Max { get; set; }
        public double 压接高度Min { get; set; }
        public double 压接高度Max { get; set; }
        public double 压接宽度Min { get; set; }
        public double 压接宽度Max { get; set; }
        public double 拉力Min { get; set; }
        public double 拉力Max { get; set; }
    }
}