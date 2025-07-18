using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MonitorAtivosB3.Conf;
using MonitorAtivosB3.Entidades;
using MonitorAtivosB3.Excecoes;
using MonitorAtivosB3.Services.AcessoDadosAtivos;
using MonitorAtivosB3.Services.Alertas;

namespace MonitorAtivosB3.Services.Monitoramento;

public class MonitoramentoService : BackgroundService
{
    private readonly ILogger<MonitoramentoService> _logger;

    private readonly IProvedorDadosAtivos _provedorDadosAtivos;
    private readonly IProcessadorAlertas _processadorAlertas;
    private readonly ConfiguracaoMonitoramento _configuracaoMonitoramento;
    private readonly ParametrosMonitoramento _parametrosMonitoramento;

    public MonitoramentoService(
        ILogger<MonitoramentoService> logger,
        IProvedorDadosAtivos provedorDadosAtivos,
        IProcessadorAlertas processadorAlertas,
        IOptions<ConfiguracaoMonitoramento> configuracaoMonitoramento,
        ParametrosMonitoramento parametrosMonitoramento
    )
    {
        _logger = logger;
        _provedorDadosAtivos = provedorDadosAtivos;
        _processadorAlertas = processadorAlertas;
        _configuracaoMonitoramento = configuracaoMonitoramento.Value;

        _parametrosMonitoramento = parametrosMonitoramento;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await MonitorarAtivo();

            await Task.Delay(_configuracaoMonitoramento.FrequenciaMonitoramentoPrecosMinutos * 60000, stoppingToken);
        }
    }

    public async Task MonitorarAtivo()
    {
        var codigoAtivo = _parametrosMonitoramento.CodigoAtivo;

        try
        {
            DadosAtivo? dadosAtivo = await _provedorDadosAtivos.ObterDadosAtivoAsync(codigoAtivo);

            if (dadosAtivo?.Preco == null)
            {
                _logger.LogWarning("Nenhum dado foi retornado para o ativo [{}]. Aguarde novas execuções...",
                    codigoAtivo
                );
                await Task.CompletedTask;
                return;
            }

            var precoAtual = dadosAtivo.Preco;
            _logger.LogInformation("Ativo [{}] - Preço atual [{}].", codigoAtivo, precoAtual);

            if (precoAtual > _parametrosMonitoramento.PrecoReferenciaVenda)
            {
                _logger.LogInformation(
                    "Preço atual é maior que o preço de referência para venda [{}]. Solicitando envio de alerta...",
                    _parametrosMonitoramento.PrecoReferenciaVenda
                );

                var alertaAtivo = new AlertaAtivoDto
                {
                    DadosAtivo = dadosAtivo, TipoNotificacao = TipoNotificacao.Venda
                };
                await _processadorAlertas.EnviarAlertaAsync(alertaAtivo);
            }
            else if (precoAtual < _parametrosMonitoramento.PrecoReferenciaCompra)
            {
                _logger.LogInformation(
                    "Preço atual é menor que o preço de referência para compra [{}]. Solicitando envio de alerta...",
                    _parametrosMonitoramento.PrecoReferenciaVenda
                );

                var alertaAtivo = new AlertaAtivoDto
                {
                    DadosAtivo = dadosAtivo, TipoNotificacao = TipoNotificacao.Compra
                };
                await _processadorAlertas.EnviarAlertaAsync(alertaAtivo);
            }
            else
            {
                _logger.LogWarning("Nenhum alerta será enviado...");
            }
        }
        catch (ParametrosInvalidosException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "Erro ao monitorar o ativo [{}]. Um novo processamento será feito em instantes. Exceção: {}",
                codigoAtivo, ex.Message
            );
        }
    }
}
