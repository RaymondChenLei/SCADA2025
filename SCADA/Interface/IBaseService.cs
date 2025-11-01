using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Interface
{
    public interface IBaseService
    {
        #region Query

        T Find<T>(string id) where T : class;

        ISugarQueryable<T> Query<T>(Expression<Func<T, bool>> funcWhere) where T : class;

        #endregion Query

        #region Add

        T Insert<T>(T t) where T : class, new();

        #endregion Add
    }
}