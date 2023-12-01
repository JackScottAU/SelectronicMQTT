using ConsoleApp1.Selectronic;
using MQTTnet.Client;
using MQTTnet;
using Microsoft.Extensions.Options;

namespace WorkerService1
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;

        private readonly Timer _timer;

        private readonly SelectLiveService _liveService;

        private readonly IMqttClient _mqttClient;

        private readonly MqttOptions _mqttOptions;

        public Worker(ILogger<Worker> logger, SelectLiveService selectLiveService, IOptions<MqttOptions> options)
        {
            _logger = logger;
            _liveService = selectLiveService;
            _timer = new Timer(ExecuteAsync, null, Timeout.Infinite, Timeout.Infinite);

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

            _logger.LogInformation("Started service.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            var disconnect = new MqttClientDisconnectOptions()
            {
                Reason = MqttClientDisconnectOptionsReason.NormalDisconnection,
            };

            await _mqttClient.DisconnectAsync(disconnect, CancellationToken.None);
        }

        private void ExecuteAsync(object? state)
        {
            SelectJsonResponse data = _liveService.RawData().Result;

            _mqttClient.PublishStringAsync("selectronic/load_wh_today", data.items.load_wh_today.ToString(), MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce, true);

        }
    }
}