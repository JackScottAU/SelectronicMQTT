using MQTTnet.Client;
using MQTTnet;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SelectronicMQTT.Service.Selectronic;

namespace SelectronicMQTT.Service
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;

        private readonly Timer _timer;

        private readonly Timer _discoveryTimer;

        private readonly Timer _authenticationTimer;

        private readonly SelectLiveService _liveService;

        private readonly IMqttClient _mqttClient;

        private readonly MqttOptions _mqttOptions;

        public Worker(ILogger<Worker> logger, SelectLiveService selectLiveService, IOptions<MqttOptions> options)
        {
            _logger = logger;
            _liveService = selectLiveService;
            _timer = new Timer(ExecuteAsync, null, Timeout.Infinite, Timeout.Infinite);
            _discoveryTimer = new Timer(SendDiscoveryMessages, null, Timeout.Infinite, Timeout.Infinite);
            _authenticationTimer = new Timer(Reauthenticate, null, Timeout.Infinite, Timeout.Infinite);

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
            _authenticationTimer.Change(TimeSpan.FromHours(6), TimeSpan.FromHours(6));

            _logger.LogInformation("Started service.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _discoveryTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _authenticationTimer.Change(Timeout.Infinite, Timeout.Infinite);

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
                SelectJsonResponse? data = _liveService.RawData().Result;

                if (data == null || data.Items == null)
                    return;

                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/load_wh_today", data.Items.load_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/load_wh_total", data.Items.load_wh_total.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/solar_wh_today", data.Items.solar_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/solar_wh_total", data.Items.solar_wh_total.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/grid_in_wh_today", data.Items.grid_in_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/grid_in_wh_total", data.Items.grid_in_wh_total.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/grid_out_wh_today", data.Items.grid_out_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/grid_out_wh_total", data.Items.grid_out_wh_total.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/battery_in_wh_today", data.Items.battery_in_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/battery_in_wh_total", data.Items.battery_in_wh_total.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/battery_out_wh_today", data.Items.battery_out_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/battery_out_wh_total", data.Items.battery_out_wh_total.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);


                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/solar_w", data.Items.solarinverter_w.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/grid_w", data.Items.grid_w.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/load_w", data.Items.load_w.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/battery_w", data.Items.battery_w.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);
                _mqttClient.PublishStringAsync("selectronic/" + _mqttOptions.UniqueID + "/battery_soc", data.Items.battery_soc.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

                _logger.LogInformation("Published statistics messages to MQTT.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch data.");
            }
        }

        private void SendDiscoveryMessages(object? state)
        {
            if(_mqttOptions.SendHomeAssistantDiscoveryMessages)
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
                PublishDiscoveryMessage("grid_w", "Grid Current Wattage", "measurement", "energy", "w");
                PublishDiscoveryMessage("load_w", "Load Current Wattage", "measurement", "energy", "w");
                PublishDiscoveryMessage("battery_w", "Battery Current Wattage", "measurement", "energy", "w");
                PublishDiscoveryMessage("battery_soc", "Battery State of Charge", "measurement", "battery", "%");

                _logger.LogInformation("Published discovery messages to MQTT.");
            }
        }

        private void Reauthenticate(object? state)
        {
            _ = _liveService.Connect();
        }

        private void PublishDiscoveryMessage(string identifier, string name, string stateClass, string deviceClass = "energy", string unit = "kWh")
        {
            // Device information is the same for all entities.
            DeviceDTO device = new()
            {
                Name = "Selectronic Inverter",
                Identifiers = new List<string>() { "selectronic" + _mqttOptions.UniqueID },
            };

            DiscoveryMessageDTO dto = new()
            {
                Name = name,
                StateClass = stateClass,
                DeviceClass = deviceClass,
                StateTopic = "selectronic/" + _mqttOptions.UniqueID + "/" + identifier,
                UnitOfMeasurement = unit,
                UniqueID = identifier + _mqttOptions.UniqueID,
                Device = device,
            };
            
            if(stateClass == "total_increasing")
            {
                // Dodgy AF.
                dto.LastReset = "1970-01-01T00:00:00+00:00";
            }

            _mqttClient.PublishStringAsync("homeassistant/sensor/selectronic" + _mqttOptions.UniqueID + "_"+identifier+"/config", JsonConvert.SerializeObject(dto), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

        }
    }
}
