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
        [JsonProperty("device_class")]
        public string? DeviceClass { get; set; }

        [JsonProperty("state_topic")]
        public string? StateTopic { get; set; }

        [JsonProperty("unit_of_measurement")]
        public string? UnitOfMeasurement { get; set; }

        [JsonProperty("unique_id")]
        public string? UniqueID { get; set; }

        [JsonProperty("device")]
        public DeviceDTO? Device { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("state_class")]
        public string? StateClass { get; set; }

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
