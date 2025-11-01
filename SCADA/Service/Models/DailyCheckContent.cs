using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models
{
    [SugarTable("MaintenanceCheckContent")]
    public class DailyCheckContent
    {
        [SugarColumn(IsIdentity = true, IsNullable = false, IsPrimaryKey = true, ColumnName = "ID")]
        public int ID { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 200, ColumnName = "CheckContent")]
        public string CheckContent { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 100, ColumnName = "CheckMethod")]
        public string CheckMethod { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 100, ColumnName = "CheckStandard")]
        public string CheckStandard { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "ImageData")]
        public byte[] ImageData { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 50, ColumnName = "Option1")]
        public string Option1 { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 50, ColumnName = "Option2")]
        public string Option2 { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 50, ColumnName = "Option3")]
        public string Option3 { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 50, ColumnName = "Option4")]
        public string Option4 { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "CheckFrequency")]
        public int CheckFrequency { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 20, ColumnName = "Type")]
        public string Type { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 20, ColumnName = "CheckListID")]
        public string CheckListID { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 50, ColumnName = "CheckListName")]
        public string CheckListName { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 20, ColumnName = "MIID")]
        public string MIID { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 50, ColumnName = "MIName")]
        public string MIName { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "ItemID")]
        public int ItemID { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "IsEnabled")]
        public bool IsEnabled { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "CreateDate")]
        public DateTime CreateDate { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "CreateID")]
        public string CreateID { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "CreateName")]
        public string CreateName { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "ReviewDate")]
        public DateTime ReviewDate { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "ReviewID")]
        public string ReviewID { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "ReviewName")]
        public string ReviewName { get; set; }
    }
}