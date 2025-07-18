using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonitorAtivosB3.Conf;
using MonitorAtivosB3.Services.AcessoDadosAtivos;
using MonitorAtivosB3.Services.Alertas;
using MonitorAtivosB3.Services.Monitoramento;

namespace MonitorAtivosB3;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = false;
                    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                    options.SingleLine = true;
                });
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<ConfiguracaoBraveApi>(context.Configuration.GetSection("BraveApi"));
                services.Configure<ConfiguracaoEmailSmtp>(context.Configuration.GetSection("Email"));
                services.Configure<ConfiguracaoMonitoramento>(context.Configuration.GetSection("Monitoramento"));

                var parametrosMonitoramento = new ParametrosParser().ParsearParametros(args);
                services.AddSingleton(parametrosMonitoramento);

                services.AddSingleton<IProvedorDadosAtivos, ProvedorBraveApi>();
                services.AddSingleton<IProcessadorAlertas, ProcessadorAlertaEmail>();
                services.AddHostedService<MonitoramentoService>();
            })
            .Build();

        await host.RunAsync();
    }
}
