using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO.Ports;

namespace SCADA.Service.Helper
{
    public class GetPorts
    {
        public List<(string PortName, string Description)> GetPortsWithDescription()
        {
            var results = new List<(string PortName, string Description)>();
            var portNames = SerialPort.GetPortNames();
            using (var searcher = new ManagementObjectSearcher(
                "SELECT Name, Caption FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\""))
            {
                foreach (var device in searcher.Get())
                {
                    string name = device["Name"]?.ToString();
                    string caption = device["Caption"]?.ToString();

                    if (name?.Contains("(COM") == true)
                    {
                        int start = name.IndexOf("(COM") + 1;
                        int end = name.IndexOf(")", start);
                        string comPort = name.Substring(start, end - start);

                        string description = caption ?? name;
                        if (description.Contains("(COM"))
                        {
                            description = description.Substring(0, description.IndexOf("(COM")).Trim();
                        }

                        if (portNames.Contains(comPort))
                        {
                            results.Add((PortName: comPort, Description: description));
                        }
                    }
                }
            }

            return results.OrderBy(x => x.PortName).ToList();
        }
    }
}