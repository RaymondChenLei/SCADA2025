using SCADA.Interface;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class BaseService : IBaseService
    {
        protected ISqlSugarClient Client { get; set; }

        public BaseService(ISqlSugarClient client)
        {
            Client = client;
        }

        public T Find<T>(string id) where T : class
        {
            return Client.Queryable<T>().InSingle(id);
        }

        public ISugarQueryable<T> Query<T>(Expression<Func<T, bool>> funcWhere) where T : class
        {
            return Client.Queryable<T>().Where(funcWhere);
        }

        protected void SetTable<T>(string tableName) where T : class
        {
            Client.CodeFirst.InitTables(typeof(T));
        }

        public T Insert<T>(T t) where T : class, new()
        {
            throw new NotImplementedException();
        }
    }
}