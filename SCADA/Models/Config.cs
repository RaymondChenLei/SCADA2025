using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Models
{
    public class Config
    {
        public string MachineType { get; set; } = "压接机";
        public string ProductNo { get; set; } = "压接机1#";
        public string RelayModulePort { get; set; } = "COM1";
    }
}