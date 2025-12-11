using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models
{
    [SugarTable("InspectionRecord")]
    public class Inspection
    {
        [SugarColumn(IsIdentity = false, IsNullable = false, IsPrimaryKey = true, ColumnName = "ID")]
        public Int64 ID { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 20, ColumnName = "DeviceID")]
        public string DeviceID { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 20, ColumnName = "CheckType")]
        public string CheckType { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 2000, ColumnName = "JsonContent")]
        public string JsonContent { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "varchar", Length = 255, ColumnName = "CreateUserID")]
        public string CreateUserID { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 20, ColumnName = "KanbanNo")]
        public string KanbanNo { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "StartTime")]
        public DateTime StartTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "EndTime")]
        public DateTime EndTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "ShiftDate")]
        public DateTime ShiftDate { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "ScannerRecordID")]
        public Int64 ScannerRecordID { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "IsGood")]
        public bool IsGood { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "WqGood")]
        public bool WqGood { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 30, ColumnName = "DieNo1")]
        public string DieNo1 { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 30, ColumnName = "DieNo2")]
        public string DieNo2 { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 30, ColumnName = "DieNo3")]
        public string DieNo3 { get; set; }
    }
}