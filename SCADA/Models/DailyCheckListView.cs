using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Models
{
    public class DailyCheckListView
    {
        public bool IsOK { get; set; }
        public int ID { get; set; }
        public string Content { get; set; }
        public string Method { get; set; }
        public string Standard { get; set; }
        public string Value { get; set; }
    }
}