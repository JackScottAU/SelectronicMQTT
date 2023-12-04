using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectronicMQTT.Service
{
    public class DiscoveryMessageDTO
    {
        public string? device_class { get; set; }

        public string? state_topic { get; set; }

        public string? unit_of_measurement { get; set; }

        public string? unique_id { get; set; }

        public DeviceDTO? device { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        public string? state_class { get; set; }

        [JsonProperty("last_reset", NullValueHandling = NullValueHandling.Ignore)]
        public string? LastReset { get; set; }
    }

    public class DeviceDTO
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("identifiers")]
        public IEnumerable<string>? Identifiers { get; set; }
    }
}
