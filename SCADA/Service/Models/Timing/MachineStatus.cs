using SqlSugar;

namespace SCADA.Service.Models.Timing
{
    [SugarTable("MachineStatus")]
    public class MachineStatus
    {
        [SugarColumn(IsIdentity = true, IsNullable = false, IsPrimaryKey = true)]
        public int ID { get; set; }

        public int StopID { get; set; }

        [SugarColumn(Length = 100)]
        public string ProductNo { get; set; }

        [SugarColumn(IsNullable = true)]
        public string KB { get; set; }

        public DateTime StartTime { get; set; }
    }
}