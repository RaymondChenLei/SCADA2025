using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models
{
    [SugarTable("LoginRecord")]
    public class LoginRecord
    {
        [SugarColumn(IsIdentity = false, IsNullable = false, IsPrimaryKey = true, ColumnName = "ID")]
        public Int64 ID { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 20, ColumnName = "DeviceID")]
        public string DeviceID { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 20, ColumnName = "LoginUserID")]
        public string LoginUserID { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 20, ColumnName = "KanbanNo")]
        public string KanbanNo { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "StartTime")]
        public DateTime StartTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "EndTime")]
        public DateTime EndTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "ShiftDate")]
        public DateTime ShiftDate { get; set; }
    }
}