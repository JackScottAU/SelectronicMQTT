using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService1
{
    public class DiscoveryMessageDTO
    {
        public string device_class { get; set; }

        public string state_topic { get; set; }

        public string unit_of_measurement { get; set; }

        public string unique_id { get; set; }

        public DeviceDTO device { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public string state_class { get; set; }

        [JsonProperty("last_reset", NullValueHandling = NullValueHandling.Ignore)]
        public string? last_reset { get; set; }
    }

    public class DeviceDTO
    {
        public string name { get; set; }

        public IEnumerable<string> identifiers { get; set; }
    }
}