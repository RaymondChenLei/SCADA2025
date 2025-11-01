using SCADA.Manager;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class SqlService
    {
        public SqlService()
        {
            Client = new SqlSugarClient(SqlConnection());
        }

        public static SqlService Instance { get; private set; } = new SqlService();
        public SqlSugarClient Client { get; private set; }

        public ConnectionConfig SqlConnection()
        {
            LocalSetting localSetting = new();
            bool ifTrusted = localSetting.ServerTrustedConnection;
            string datasource = localSetting.ServerDataSource;
            string initialcatalog = localSetting.ServerInitialCatalog;
            string trustedinitialcatalog = localSetting.TrustedServerInitialCatalog;
            string userid = localSetting.ServerUserID;
            string password = localSetting.ServerPassword;
            if (ifTrusted == true)
            {
                ConnectionConfig connectionConfig = new()
                {
                    ConnectionString = $"Server=127.0.0.1;Database={initialcatalog};Trusted_Connection=true;TrustServerCertificate=true",
                    IsAutoCloseConnection = true,
                    DbType = DbType.SqlServer
                };
                return connectionConfig;
            }
            else
            {
                ConnectionConfig connectionConfig = new()
                {
                    ConnectionString = $"Data Source={datasource};Initial Catalog={trustedinitialcatalog};Persist Security Info=True;User ID={userid};Pwd={password};TrustServerCertificate=true",
                    IsAutoCloseConnection = true,
                    DbType = DbType.SqlServer
                };
                return connectionConfig;
            }
        }
    }
}