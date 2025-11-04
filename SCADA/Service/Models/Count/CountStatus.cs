using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models.Count
{
    [SugarTable("CountStatus")]
    public class CountStatus
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsNullable = false)]
        public int Id { get; set; }

        [SugarColumn(Length = 20)]
        public string KB { get; set; }

        public int TotalCount { get; set; }
        public int GoodCount { get; set; }
        public int NGCount { get; set; }
        public int SampleCount { get; set; }
    }
}