using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models
{
    [SugarTable("HsdKanban")]
    public class HSDKanban
    {
        [SugarColumn(IsIdentity = true, IsNullable = false, IsPrimaryKey = true, ColumnName = "Id")]
        public int Id { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 50, ColumnName = "KanbanName")]
        public string KanbanName { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 20, ColumnName = "KanbanNo")]
        public string KanbanNo { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 50, ColumnName = "ACenterTerminalDieNo")]
        public string ACenterTerminalDieNo { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 20, ColumnName = "ACenterTerminalNo")]
        public string ACenterTerminalNo { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 50, ColumnName = "BCenterTerminalDieNo")]
        public string BCenterTerminalDieNo { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 20, ColumnName = "BCenterTerminalNo")]
        public string BCenterTerminalNo { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 100, ColumnName = "ProjectName")]
        public string ProjectName { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 100, ColumnName = "CustomerName")]
        public string CustomerName { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "CreateTime")]
        public DateTime CreateTime { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "CreateUserId")]
        public int CreateUserId { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 20, ColumnName = "Type")]
        public string Type { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 20, ColumnName = "DeviceID")]
        public string DeviceID { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 1000, ColumnName = "JsonContent")]
        public string JsonContent { get; set; }
    }
}