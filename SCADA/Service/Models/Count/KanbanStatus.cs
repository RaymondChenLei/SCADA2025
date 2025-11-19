using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models.Count
{
    [SugarTable("KanbanStatus")]
    public class KanbanStatus
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsNullable = false)]
        public int Id { get; set; }

        [SugarColumn(Length = 10, IsNullable = true)]
        public string Shift { get; set; }

        public DateTime ShiftDate { get; set; }

        [SugarColumn(Length = 20, IsNullable = true)]
        public string KB { get; set; }

        [SugarColumn(Length = 20, IsNullable = true)]
        public string MaterialD { get; set; }

        [SugarColumn(Length = 20, IsNullable = true)]
        public string MaterialE { get; set; }

        [SugarColumn(Length = 20, IsNullable = true)]
        public string MaterialF { get; set; }

        [SugarColumn(Length = 20, IsNullable = true)]
        public string MaterialG { get; set; }

        [SugarColumn(Length = 20, IsNullable = true)]
        public string MaterialH { get; set; }

        [SugarColumn(Length = 20, IsNullable = true)]
        public string MaterialI { get; set; }

        public bool ScanDone { get; set; } = false;
    }
}