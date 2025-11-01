using SCADA.Service.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class EquipmentCatalogService : BaseService
    {
        public EquipmentCatalogService(ISqlSugarClient client) : base(client)
        {
            SetTable<EquipmentCatalog>("EquipmentCatalog");
        }
    }
}