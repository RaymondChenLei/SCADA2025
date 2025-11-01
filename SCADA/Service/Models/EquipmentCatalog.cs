using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.Models
{
    [SugarTable("EquipmentCatalog")]
    public class EquipmentCatalog
    {
        [SugarColumn(IsIdentity = true, IsNullable = false, IsPrimaryKey = true, ColumnName = "ID")]
        public int ID { get; set; }

        public string ProductNo { get; set; }
        public string MachineName { get; set; }
        public string EquipmentType { get; set; }
    }
}