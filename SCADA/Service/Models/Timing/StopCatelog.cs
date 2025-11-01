using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models.Timing
{
    [SugarTable("StopCatelog")]
    public class StopCatelog
    {
        [SugarColumn(IsIdentity = true, IsNullable = false, IsPrimaryKey = true)]
        public int ID { get; set; }

        public int StopID { get; set; }

        [SugarColumn(Length = 50)]
        public string StopName { get; set; }

        [SugarColumn(Length = 50)]
        public string StopCatagory { get; set; }
    }
}