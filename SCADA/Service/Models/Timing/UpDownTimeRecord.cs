using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models.Timing
{
    [SugarTable("UpDownTimeRecord")]
    public class UpDownTimeRecord
    {
        public int ID { get; set; }
        public string ProductNo { get; set; }
        public string MachineName { get; set; }
        public string TimingCatagory { get; set; }
        public string TimingName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string OperatorID { get; set; }
        public string KanbanNo { get; set; }
        public DateTime ShiftDate { get; set; }
    }
}