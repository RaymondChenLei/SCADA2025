using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models.Count
{
    [SugarTable("KanbanCount")]
    public class KanbanCount
    {
        [SugarColumn(IsIdentity = false, IsNullable = false, IsPrimaryKey = true, ColumnName = "ID")]
        public Int64 ID { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 20, ColumnName = "DeviceID")]
        public string DeviceID { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "PassCount")]
        public int PassCount { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "NgCount")]
        public int NgCount { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = 255, ColumnName = "CreateUserID")]
        public string CreateUserID { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 20, ColumnName = "KanbanNo")]
        public string KanbanNo { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "UpdateTime")]
        public DateTime UpdateTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "ShiftDate")]
        public DateTime ShiftDate { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "Isfinish")]
        public bool Isfinish { get; set; }
    }
}