using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Models
{
    public class KBJson
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("MinValue")]
        public double MinValue { get; set; }

        [JsonProperty("MaxValue")]
        public double MaxValue { get; set; }
    }
}