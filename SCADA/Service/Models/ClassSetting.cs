using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models
{
    [SugarTable("ClassSetting")]
    public class ClassSetting
    {
        [SugarColumn(IsPrimaryKey = true, IsNullable = false)]
        public long Id { get; set; }

        [SugarColumn(Length = 50)]
        public string ClassName { get; set; }

        [SugarColumn(Length = 50)]
        public string StartTime { get; set; }

        [SugarColumn(Length = 50)]
        public string EndTime { get; set; }
    }
}