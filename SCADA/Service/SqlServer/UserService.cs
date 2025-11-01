using SCADA.Service.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class UserService : BaseService
    {
        public UserService(ISqlSugarClient client) : base(client)
        {
        }

        public string GetNamebyID(string ID)
        {
            var result = Client.Queryable<User>()
                .Where(x => x.IdNo == ID)
                .ToList();
            if (result.Any())
            {
                return result.FirstOrDefault().UserName;
            }
            else
            {
                return null;
            }
        }

        public User GetUserbyCardID(string CardID)
        {
            var result = Client.Queryable<User>()
                .Where(x => x.CardId == CardID)
                .ToList();
            if (result.Any())
            {
                return result.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
    }
}