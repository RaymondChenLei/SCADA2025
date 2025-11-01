using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models
{
    [SugarTable("DailyCheckRecord")]
    public class DailyCheckRecord
    {
        [SugarColumn(IsIdentity = false, IsNullable = false, IsPrimaryKey = true, ColumnName = "ID")]
        public Int64 ID { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 50, ColumnName = "DeviceID")]
        public string DeviceID { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 80, ColumnName = "CheckName")]
        public string CheckName { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 20, ColumnName = "UserName")]
        public string UserName { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = 255, ColumnName = "CreateUserID")]
        public string CreateUserID { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 20, ColumnName = "Value")]
        public string Value { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "IsOK")]
        public bool IsOK { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "IsSubmit")]
        public bool IsSubmit { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "ShiftDate")]
        public DateTime ShiftDate { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 255, ColumnName = "Remark")]
        public string Remark { get; set; }
    }
}