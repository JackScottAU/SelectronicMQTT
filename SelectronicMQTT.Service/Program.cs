using SelectronicMQTT.Service.Selectronic;

namespace SelectronicMQTT.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Services.Configure<SelectLiveConfiguration>(builder.Configuration.GetSection("Selectronic"));
            builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection("Mqtt"));

            builder.Services.AddSingleton<SelectLiveService>();

            builder.Services.AddHostedService<Worker>();

            IHost host = builder.Build();
            host.Run();
        }
    }
}
