using SCADA.Service.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer
{
    public class DailyCheckReviewService : BaseService
    {
        public DailyCheckReviewService(ISqlSugarClient client) : base(client)
        {
            SetTable<Models.DailyCheckReview>("DailyCheckReview");
        }

        public void InsertRecord(DailyCheckReview review)
        {
            Client.Storageable(review).ExecuteCommand();
        }
    }
}