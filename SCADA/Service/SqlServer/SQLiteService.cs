using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class SQLiteService
    {
        public SQLiteService()
        {
            Db = new(GetConnectionConfig());
        }

        private ConnectionConfig GetConnectionConfig()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "LocalDB.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
            return new ConnectionConfig()
            {
                ConnectionString = $"Data Source={dbPath};",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                }
            };
        }

        private static readonly Lazy<SQLiteService> _instance = new(() => new SQLiteService());
        public static SQLiteService Instance => _instance.Value;
        public SqlSugarScope Db { get; }
    }
}