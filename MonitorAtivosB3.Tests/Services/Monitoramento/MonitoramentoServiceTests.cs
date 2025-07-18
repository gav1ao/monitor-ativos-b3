using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MonitorAtivosB3.Conf;
using MonitorAtivosB3.Entidades;
using MonitorAtivosB3.Services.AcessoDadosAtivos;
using MonitorAtivosB3.Services.Alertas;
using MonitorAtivosB3.Services.Monitoramento;
using Moq;

namespace MonitorAtivosB3.Tests.Services.Monitoramento;

public class MonitoramentoServiceTests
{
    private const string CodigoAtivo = "PETR4";
    private const string NomeAtivo = "Petróleo Brasileiro S.A. - Petrobras";
    private const string Moeda = "BRL";

    private const decimal PrecoReferenciaVenda = 22.57m;
    private const decimal PrecoReferenciaCompra = 22.49m;

    private readonly Mock<ILogger<MonitoramentoService>> _loggerMock = new();
    private readonly Mock<IProvedorDadosAtivos> _provedorDadosMock = new();
    private readonly Mock<IProcessadorAlertas> _processadorAlertasMock = new();
    private readonly Mock<IOptions<ConfiguracaoMonitoramento>> _configuracaoMonitoramentoMock = new();

    private readonly MonitoramentoService _service;

    public MonitoramentoServiceTests()
    {
        var parametrosMonitoramento = new ParametrosMonitoramento
        {
            CodigoAtivo = CodigoAtivo,
            PrecoReferenciaVenda = PrecoReferenciaVenda,
            PrecoReferenciaCompra = PrecoReferenciaCompra,
        };

        _service = new MonitoramentoService(
            _loggerMock.Object,
            _provedorDadosMock.Object,
            _processadorAlertasMock.Object,
            _configuracaoMonitoramentoMock.Object,
            parametrosMonitoramento
        );
    }

    [Fact]
    public async Task MonitorarAtivo_QuandoPrecoAtualMaiorQueReferenciaCompra_NaoDeveGerarAlerta()
    {
        const decimal precoAtual = PrecoReferenciaCompra + 0.01m;

        var dadosAtivo = new DadosAtivo { NomeAtivo = NomeAtivo, Moeda = Moeda, Preco = precoAtual };
        _provedorDadosMock.Setup(p => p.ObterDadosAtivoAsync(It.IsAny<string>()))
            .ReturnsAsync(dadosAtivo);

        _processadorAlertasMock.Setup(a => a.EnviarAlertaAsync(It.IsAny<AlertaAtivoDto>()))
            .Returns(Task.CompletedTask);

        await _service.MonitorarAtivo();

        _processadorAlertasMock.Verify(a => a.EnviarAlertaAsync(It.IsAny<AlertaAtivoDto>()), Times.Never);
    }

    [Fact]
    public async Task MonitorarAtivo_QuandoPrecoAtualIgualReferenciaCompra_NaoDeveGerarAlerta()
    {
        const decimal precoAtual = PrecoReferenciaCompra;

        var dadosAtivo = new DadosAtivo { NomeAtivo = NomeAtivo, Moeda = Moeda, Preco = precoAtual };
        _provedorDadosMock.Setup(p => p.ObterDadosAtivoAsync(It.IsAny<string>()))
            .ReturnsAsync(dadosAtivo);

        _processadorAlertasMock.Setup(a => a.EnviarAlertaAsync(It.IsAny<AlertaAtivoDto>()))
            .Returns(Task.CompletedTask);

        await _service.MonitorarAtivo();

        _processadorAlertasMock.Verify(a => a.EnviarAlertaAsync(It.IsAny<AlertaAtivoDto>()), Times.Never);
    }

    [Fact]
    public async Task MonitorarAtivo_QuandoPrecoAtualMenorQueReferenciaCompra_DeveGerarAlerta()
    {
        const decimal precoAtual = PrecoReferenciaCompra - 10;

        var dadosAtivo = new DadosAtivo { NomeAtivo = NomeAtivo, Moeda = Moeda, Preco = precoAtual };
        _provedorDadosMock.Setup(p => p.ObterDadosAtivoAsync(It.IsAny<string>()))
            .ReturnsAsync(dadosAtivo);

        _processadorAlertasMock.Setup(a => a.EnviarAlertaAsync(It.IsAny<AlertaAtivoDto>()))
            .Returns(Task.CompletedTask);

        await _service.MonitorarAtivo();

        _processadorAlertasMock.Verify(a => a.EnviarAlertaAsync(
            It.Is<AlertaAtivoDto>(alerta =>
                alerta.DadosAtivo == dadosAtivo && alerta.TipoNotificacao == TipoNotificacao.Compra)), Times.Once);
    }

    [Fact]
    public async Task MonitorarAtivo_QuandoPrecoAtualMenorQueReferenciaVenda_NaoDeveGerarAlerta()
    {
        const decimal precoAtual = PrecoReferenciaVenda - 0.01m;

        var dadosAtivo = new DadosAtivo { NomeAtivo = NomeAtivo, Moeda = Moeda, Preco = precoAtual };
        _provedorDadosMock.Setup(p => p.ObterDadosAtivoAsync(It.IsAny<string>()))
            .ReturnsAsync(dadosAtivo);

        _processadorAlertasMock.Setup(a => a.EnviarAlertaAsync(It.IsAny<AlertaAtivoDto>()))
            .Returns(Task.CompletedTask);

        await _service.MonitorarAtivo();

        _processadorAlertasMock.Verify(a => a.EnviarAlertaAsync(It.IsAny<AlertaAtivoDto>()), Times.Never);
    }

    [Fact]
    public async Task MonitorarAtivo_QuandoPrecoAtualIgualReferenciaVenda_NaoDeveGerarAlerta()
    {
        const decimal precoAtual = PrecoReferenciaVenda;

        var dadosAtivo = new DadosAtivo { NomeAtivo = NomeAtivo, Moeda = Moeda, Preco = precoAtual };
        _provedorDadosMock.Setup(p => p.ObterDadosAtivoAsync(It.IsAny<string>()))
            .ReturnsAsync(dadosAtivo);

        _processadorAlertasMock.Setup(a => a.EnviarAlertaAsync(It.IsAny<AlertaAtivoDto>()))
            .Returns(Task.CompletedTask);

        await _service.MonitorarAtivo();

        _processadorAlertasMock.Verify(a => a.EnviarAlertaAsync(It.IsAny<AlertaAtivoDto>()), Times.Never);
    }

    [Fact]
    public async Task MonitorarAtivo_QuandoPrecoAtualMaiiorQueReferenciaVenda_DeveGerarAlerta()
    {
        const decimal precoAtual = PrecoReferenciaVenda + 10m;

        var dadosAtivo = new DadosAtivo { NomeAtivo = NomeAtivo, Moeda = Moeda, Preco = precoAtual };
        _provedorDadosMock.Setup(p => p.ObterDadosAtivoAsync(It.IsAny<string>()))
            .ReturnsAsync(dadosAtivo);

        _processadorAlertasMock.Setup(a => a.EnviarAlertaAsync(It.IsAny<AlertaAtivoDto>()))
            .Returns(Task.CompletedTask);

        await _service.MonitorarAtivo();

        _processadorAlertasMock.Verify(a => a.EnviarAlertaAsync(
            It.Is<AlertaAtivoDto>(alerta =>
                alerta.DadosAtivo == dadosAtivo && alerta.TipoNotificacao == TipoNotificacao.Venda)), Times.Once);
    }
}
