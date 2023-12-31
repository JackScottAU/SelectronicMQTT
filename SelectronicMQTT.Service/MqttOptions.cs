﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectronicMQTT.Service
{
    public class MqttOptions
    {
        public string? Hostname { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public bool SendHomeAssistantDiscoveryMessages { get; set; }

        public string? UniqueID { get; set; }
    }
}
