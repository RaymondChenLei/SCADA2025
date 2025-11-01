using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models
{
    [SugarTable("User")]
    public class User
    {
        [SugarColumn(IsIdentity = true, IsNullable = false, IsPrimaryKey = true, ColumnName = "Id")]
        public int Id { get; set; }

        [SugarColumn(IsNullable = false, ColumnDataType = "nvarchar", Length = 100, ColumnName = "UserName")]
        public string UserName { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = -1, ColumnName = "UserPower")]
        public string UserPower { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 50, ColumnName = "Password")]
        public string Password { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 100, ColumnName = "RealName")]
        public string RealName { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 100, ColumnName = "IdNo")]
        public string IdNo { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 100, ColumnName = "CardId")]
        public string CardId { get; set; }

        [SugarColumn(IsNullable = false, ColumnName = "IsAdmin")]
        public int IsAdmin { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 500, ColumnName = "UserGroupKey")]
        public string UserGroupKey { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar", Length = 50, ColumnName = "Shift")]
        public string Shift { get; set; }
    }
}