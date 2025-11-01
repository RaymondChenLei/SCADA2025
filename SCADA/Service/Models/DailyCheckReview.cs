using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models
{
    [SugarTable("DailyCheckReview")]
    public class DailyCheckReview
    {
        [SugarColumn(IsIdentity = true, IsNullable = false, ColumnName = "ID")]
        public int ID { get; set; }

        [SugarColumn(IsIdentity = false, IsNullable = false, IsPrimaryKey = true, ColumnName = "SID")]
        public Int64 SID { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 10, ColumnName = "Shift")]
        public string Shift { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 10, ColumnName = "OperatorID")]
        public string OperatorID { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 10, ColumnName = "OperatorName")]
        public string OperatorName { get; set; }

        [SugarColumn(ColumnName = "OperatorDate")]
        public DateTime OperatorDate { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "TLID")]
        public string TLID { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "TLName")]
        public string TLName { get; set; }

        [SugarColumn(ColumnName = "TLDate")]
        public DateTime TLDate { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "QCID")]
        public string QCID { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "QCName")]
        public string QCName { get; set; }

        [SugarColumn(ColumnName = "QCDate")]
        public DateTime QCDate { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "MEID")]
        public string MEID { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 10, ColumnName = "MEName")]
        public string MEName { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "MEDate")]
        public DateTime MEDate { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = -1, ColumnName = "Remark")]
        public string Remark { get; set; }
    }
}