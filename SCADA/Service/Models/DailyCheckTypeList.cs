using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models
{
    [SugarTable("DailyCheckTypeList")]
    public class DailyCheckTypeList
    {
        [SugarColumn(IsIdentity = true, IsNullable = false, ColumnName = "ID")]
        public int ID { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", IsPrimaryKey = true, Length = 50, ColumnName = "Type")]
        public string Type { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "Status")]
        public string Status { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "CreateDate")]
        public DateTime CreateDate { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "CreateName")]
        public string CreateName { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "ReviewDate")]
        public DateTime ReviewDate { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "ReviewName")]
        public string ReviewName { get; set; }
    }
}