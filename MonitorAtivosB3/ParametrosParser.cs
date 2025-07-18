using System.Text.RegularExpressions;
using MonitorAtivosB3.Entidades;
using MonitorAtivosB3.Excecoes;
using static System.Decimal;

namespace MonitorAtivosB3;

public class ParametrosParser
{
    private const string TickerB3Pattern = @"^[A-Z]{4}\d{1,2}F?$";

    public const string MsgErroNumeroParametrosInvalidos =
        "Parâmetros inválidos. O parâmetros devem possuir o seguinte formato: <TICKER> <VENDA> <COMPRA>. Exemplo: PETR4 22.67 22.59";

    public const string MsgErroCodigoAtivoInvalido =
        "Código de ativo inválido. Era esperado o 'Ticker' do Ativo no formato: 4 letras + 1 ou 2 dígitos, opcionalmente seguido de 'F'. Ex: PETR4, MXRF11F.";

    public const string MsgErroPrecosInvalidos = "O preço de venda esperado deveria ser maior que o preço de compra.";

    public const string MsgErroPrecoCompraInvalido =
        "Era esperado que o preço máximo de compra fosse um valor numérico.";

    public const string MsgErroPrecoVendaInvalido = "Era esperado que o preço mínimo de venda fosse um valor numérico.";

    public ParametrosMonitoramento ParsearParametros(string?[] args)
    {
        if (args?.Length != 3)
        {
            throw new ParametrosInvalidosException(MsgErroNumeroParametrosInvalidos);
        }

        var codigoAtivo = args[0];
        var precoReferenciaVendaArg = args[1];
        var precoReferenciaCompraArg = args[2];

        if (string.IsNullOrEmpty(codigoAtivo) || !Regex.IsMatch(codigoAtivo.ToUpper(), TickerB3Pattern))
        {
            throw new ParametrosInvalidosException(MsgErroCodigoAtivoInvalido);
        }

        if (!TryParse(precoReferenciaVendaArg, out var precoReferenciaVenda) || precoReferenciaVenda <= 0)
        {
            throw new ParametrosInvalidosException(MsgErroPrecoVendaInvalido);
        }

        if (!TryParse(precoReferenciaCompraArg, out var precoReferenciaCompra) || precoReferenciaCompra <= 0)
        {
            throw new ParametrosInvalidosException(MsgErroPrecoCompraInvalido);
        }

        if (precoReferenciaCompra >= precoReferenciaVenda)
        {
            throw new ParametrosInvalidosException(MsgErroPrecosInvalidos);
        }

        return new ParametrosMonitoramento
        {
            CodigoAtivo = codigoAtivo,
            PrecoReferenciaVenda = precoReferenciaVenda,
            PrecoReferenciaCompra = precoReferenciaCompra,
        };
    }
}
