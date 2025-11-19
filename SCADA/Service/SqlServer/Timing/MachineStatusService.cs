using SCADA.Manager;
using SCADA.Service.Models.Timing;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Service.SqlServer.Timing
{
    public class MachineStatusService : BaseService
    {
        public MachineStatusService(ISqlSugarClient client) : base(client)
        {
            SetTable<MachineStatus>("MachineStatus");
        }

        public int GetTimeCatagory()
        {
            var productNo = GlobalSettings.Instance.ProductNo;
            var result = Client.Queryable<MachineStatus>()
                .Where(x => x.ProductNo == productNo)
                .ToList();
            if (result.Any())
            {
                return result.FirstOrDefault().StopID;
            }
            else
            {
                return 0;
            }
        }

        public MachineStatus GetStatus()
        {
            var productNo = GlobalSettings.Instance.ProductNo;
            var result = Client.Queryable<MachineStatus>()
                .Where(x => x.ProductNo == productNo)
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

        public void SetStatus(int stopID)
        {
            MachineStatus status = new()
            {
                StopID = stopID,
                ProductNo = GlobalSettings.Instance.ProductNo,
                KB = GlobalSettings.Instance.KB,
                StartTime = DateTime.Now,
            };
            Client.Storageable(status).WhereColumns(x => x.ProductNo).ExecuteCommand();
        }

        public DateTime GetStartTime()
        {
            var productNo = GlobalSettings.Instance.ProductNo;
            var result = Client.Queryable<MachineStatus>()
                .Where(x => x.ProductNo == productNo)
                .ToList();
            if (result.Any())
            {
                return result.FirstOrDefault().StartTime;
            }
            else
            {
                return DateTime.Now;
            }
        }

        public void UpdateStatus(string productNo)
        {
            var result = Client.Queryable<MachineStatus>().Where(x => x.ProductNo == productNo).ToList();
            if (result.Any())
            {
                return;
            }
            else
            {
                MachineStatus record = new()
                {
                    ProductNo = productNo,
                    StartTime = DateTime.Now,
                    StopID = 0
                };
                Client.Insertable(record).ExecuteCommand();
            }
        }
    }
}