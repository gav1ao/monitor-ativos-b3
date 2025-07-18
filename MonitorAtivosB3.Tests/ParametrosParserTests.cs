using MonitorAtivosB3.Excecoes;

namespace MonitorAtivosB3.Tests;

public class ParametrosParserTests
{
    private readonly ParametrosParser _parametrosParser;

    public ParametrosParserTests()
    {
        _parametrosParser = new ParametrosParser();
    }

    [Fact]
    public void ParsearParametros_NumeroParametrosInvalidos_DeveLancarExcecao()
    {
        string[] args = [];

        var exception = Assert.Throws<ParametrosInvalidosException>(() => _parametrosParser.ParsearParametros(args));
        Assert.Equal(ParametrosParser.MsgErroNumeroParametrosInvalidos, exception.Message);
    }

    [Fact]
    public void ParsearParametros_CodigoAtivoInvalido_DeveLancarExcecao()
    {
        const string codigoAtivo = "CODIGO_INVALIDO";
        const string precoVenda = "22.67";
        const string precoCompra = "22.59";


        string[] args = [codigoAtivo, precoVenda, precoCompra];

        var exception = Assert.Throws<ParametrosInvalidosException>(() => _parametrosParser.ParsearParametros(args));
        Assert.Equal(ParametrosParser.MsgErroCodigoAtivoInvalido, exception.Message);
    }

    [Fact]
    public void ParsearParametros_PrecoVendaInvalido_DeveLancarExcecao()
    {
        const string codigoAtivo = "PETR4";
        const string precoVenda = "PRECO_VENDA_INVALIDO";
        const string precoCompra = "80.88";


        string[] args = [codigoAtivo, precoVenda, precoCompra];

        var exception = Assert.Throws<ParametrosInvalidosException>(() => _parametrosParser.ParsearParametros(args));
        Assert.Equal(ParametrosParser.MsgErroPrecoVendaInvalido, exception.Message);
    }

    [Fact]
    public void ParsearParametros_PrecoCompraInvalido_DeveLancarExcecao()
    {
        const string codigoAtivo = "PETR4";
        const string precoVenda = "10.0";
        const string precoCompra = "PRECO_COMPRA_INVALIDO";


        string[] args = [codigoAtivo, precoVenda, precoCompra];

        var exception = Assert.Throws<ParametrosInvalidosException>(() => _parametrosParser.ParsearParametros(args));
        Assert.Equal(ParametrosParser.MsgErroPrecoCompraInvalido, exception.Message);
    }

    [Fact]
    public void ParsearParametros_PrecosInconsistentes_DeveLancarExcecao()
    {
        const string codigoAtivo = "PETR4";
        const string precoVenda = "10.0";
        const string precoCompra = "90.0";


        string[] args = [codigoAtivo, precoVenda, precoCompra];

        var exception = Assert.Throws<ParametrosInvalidosException>(() => _parametrosParser.ParsearParametros(args));
        Assert.Equal(ParametrosParser.MsgErroPrecosInvalidos, exception.Message);
    }

    [Fact]
    public void ParsearParametros_ParametrosConsistentes_DeveRetornarParametrosParseados()
    {
        const string codigoAtivo = "PETR4";
        const string precoVenda = "22.67";
        const string precoCompra = "22.59";

        string[] args = [codigoAtivo, precoVenda, precoCompra];

        var parametrosParseados = _parametrosParser.ParsearParametros(args);

        Assert.Equal(codigoAtivo, parametrosParseados.CodigoAtivo);
        Assert.Equal(decimal.Parse(precoVenda), parametrosParseados.PrecoReferenciaVenda);
        Assert.Equal(decimal.Parse(precoCompra), parametrosParseados.PrecoReferenciaCompra);
    }
}
