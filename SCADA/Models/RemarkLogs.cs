using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Models
{
    public class RemarkLogs
    {
        public string Name { get; set; }
        public string UserID { get; set; }
        public DateTime Date { get; set; }
        public string Role { get; set; }
        public string Remark { get; set; }
    }
}