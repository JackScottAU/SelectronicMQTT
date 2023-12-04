using ConsoleApp1.Selectronic;
using MQTTnet.Client;
using MQTTnet;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkerService1
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;

        private readonly Timer _timer;

        private readonly Timer _discoveryTimer;

        private readonly SelectLiveService _liveService;

        private readonly IMqttClient _mqttClient;

        private readonly MqttOptions _mqttOptions;

        public Worker(ILogger<Worker> logger, SelectLiveService selectLiveService, IOptions<MqttOptions> options)
        {
            _logger = logger;
            _liveService = selectLiveService;
            _timer = new Timer(ExecuteAsync, null, Timeout.Infinite, Timeout.Infinite);
            _discoveryTimer = new Timer(SendDiscoveryMessages, null, Timeout.Infinite, Timeout.Infinite);

            var mqttFactory = new MqttFactory();

            _mqttClient = mqttFactory.CreateMqttClient();

            _mqttOptions = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            

            // connect here.
            await _liveService.Connect();

            
                
                
            // Use builder classes where possible in this project.
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(_mqttOptions.Hostname).WithCredentials(_mqttOptions.Username, _mqttOptions.Password).Build();

            // This will throw an exception if the server is not available.
            // The result from this message returns additional data which was sent 
            // from the server. Please refer to the MQTT protocol specification for details.
            await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);



            _timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(1));
            _discoveryTimer.Change(TimeSpan.Zero, TimeSpan.FromHours(1));

            _logger.LogInformation("Started service.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _discoveryTimer.Change(Timeout.Infinite, Timeout.Infinite);

            var disconnect = new MqttClientDisconnectOptions()
            {
                Reason = MqttClientDisconnectOptionsReason.NormalDisconnection,
            };

            await _mqttClient.DisconnectAsync(disconnect, CancellationToken.None);
        }

        private void ExecuteAsync(object? state)
        {
            try
            {
                SelectJsonResponse data = _liveService.RawData().Result;

                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/load_wh_today", data.items.load_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/load_wh_total", data.items.load_wh_total.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/solar_wh_today", data.items.solar_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/solar_wh_total", data.items.solar_wh_total.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/grid_in_wh_today", data.items.grid_in_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/grid_in_wh_total", data.items.grid_in_wh_total.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/grid_out_wh_today", data.items.grid_out_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/grid_out_wh_total", data.items.grid_out_wh_total.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/battery_in_wh_today", data.items.battery_in_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/battery_in_wh_total", data.items.battery_in_wh_total.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/battery_out_wh_today", data.items.battery_out_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/battery_out_wh_total", data.items.battery_out_wh_total.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);


                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/solar_w", data.items.solarinverter_w.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/battery_soc", data.items.battery_soc.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

                _logger.LogInformation("Published statistics messages to MQTT.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch data.");
            }
        }

        private void SendDiscoveryMessages(object? state)
        {
            PublishDiscoveryMessage("load_wh_today", "Load kWh Today", "total_increasing");
            PublishDiscoveryMessage("load_wh_total", "Load kWh Total", "total_increasing");
            PublishDiscoveryMessage("solar_wh_today", "Solar kWh Today", "total_increasing");
            PublishDiscoveryMessage("solar_wh_total", "Solar kWh Total", "total_increasing");
            PublishDiscoveryMessage("grid_in_wh_today", "Grid In kWh Today", "total_increasing");
            PublishDiscoveryMessage("grid_in_wh_total", "Grid In kWh Total", "total_increasing");
            PublishDiscoveryMessage("grid_out_wh_today", "Grid Out kWh Today", "total_increasing");
            PublishDiscoveryMessage("grid_out_wh_total", "Grid Out kWh Total", "total_increasing");
            PublishDiscoveryMessage("battery_in_wh_today", "Battery In kWh Today", "total_increasing");
            PublishDiscoveryMessage("battery_in_wh_total", "Battery In kWh Total", "total_increasing");
            PublishDiscoveryMessage("battery_out_wh_today", "Battery Out kWh Today", "total_increasing");
            PublishDiscoveryMessage("battery_out_wh_total", "Battery Out kWh Total", "total_increasing");
            PublishDiscoveryMessage("solar_w", "Solar Current Wattage", "measurement", "energy", "w");
            PublishDiscoveryMessage("battery_soc", "Battery State of Charge", "measurement", "battery", "%");

            _logger.LogInformation("Published discovery messages to MQTT.");
        }

        private void PublishDiscoveryMessage(string identifier, string name, string stateClass, string deviceClass = "energy", string unit = "kWh")
        {
            // Device information is the same for all entities.
            DeviceDTO device = new()
            {
                name = "Selectronic Inverter",
                identifiers = new List<string>() { "selectronic" + _mqttOptions.UniqueID },
            };

            DiscoveryMessageDTO dto = new()
            {
                Name = name,
                state_class = stateClass,
                device_class = deviceClass,
                state_topic = "selectronic/" + _mqttOptions.UniqueID + "/" + identifier,
                unit_of_measurement = unit,
                unique_id = identifier + _mqttOptions.UniqueID,
                device = device,
            };
            
            if(stateClass == "total_increasing")
            {
                // Dodgy AF.
                dto.last_reset = "1970-01-01T00:00:00+00:00";
            }

            _mqttClient.PublishStringAsync("homeassistant/sensor/selectronic" + _mqttOptions.UniqueID + "_"+identifier+"/config", JsonConvert.SerializeObject(dto), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

        }
    }
}
