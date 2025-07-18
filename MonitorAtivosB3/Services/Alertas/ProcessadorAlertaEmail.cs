using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MonitorAtivosB3.Conf;
using MonitorAtivosB3.Entidades;
using MonitorAtivosB3.Excecoes;

namespace MonitorAtivosB3.Services.Alertas;

public class ProcessadorAlertaEmail : IProcessadorAlertas
{

    private const string EmailPattern = @"\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}";

    private readonly ILogger<ProcessadorAlertaEmail> _logger;

    private readonly ConfiguracaoEmailSmtp _configuracaoEmailSmtp;

    public ProcessadorAlertaEmail(ILogger<ProcessadorAlertaEmail> logger, IOptions<ConfiguracaoEmailSmtp> configuracaoStmp)
    {

        _logger = logger;
        _configuracaoEmailSmtp = configuracaoStmp.Value;
    }

    public Task EnviarAlertaAsync(AlertaAtivoDto alertaAtivoDto)
    {
        _logger.LogInformation("Solicitação de alerta recebida: {}", alertaAtivoDto);

        EnviarEmail(alertaAtivoDto);

        _logger.LogInformation("Alerta processado!");

        return Task.CompletedTask;
    }

    private string GerarBodyEmail(Entidades.DadosAtivo dadosAtivo, TipoNotificacao tipoNotificacao)
    {
        return $"""
               Prezado(a)
               
               Conforme solicitado, o ativo '{dadosAtivo.NomeAtivo}' está sendo negociado por {dadosAtivo.Moeda} {dadosAtivo.Preco}.
               Dessa forma, é sugerido que seja feita a {tipoNotificacao.ToString().ToUpper()} do mesmo.
               """;
    }

    private void EnviarEmail(AlertaAtivoDto alertaAtivoDto)
    {
        SmtpClient smtpClient;

        try
        {
            smtpClient = new SmtpClient(_configuracaoEmailSmtp.Server, _configuracaoEmailSmtp.Port)
            {
                Credentials = new NetworkCredential(_configuracaoEmailSmtp.Username, _configuracaoEmailSmtp.Password),
                EnableSsl = _configuracaoEmailSmtp.EnableSsl
            };
        }
        catch (Exception ex)
        {
            var msgErro = "Credenciais SMTP inválidas.";
            _logger.LogError(msgErro, ex);

            throw new ParametrosInvalidosException(msgErro);
        }

        var remetente = _configuracaoEmailSmtp.EmailRemetente;
        ValidarEnderecoEmail(remetente);

        var destinatario = _configuracaoEmailSmtp.EmailDestinatario;
        ValidarEnderecoEmail(destinatario);

        var dadosAtivo = alertaAtivoDto.DadosAtivo;
        string assunto = $"Alerta de Preços - {dadosAtivo.NomeAtivo}";
        string body = GerarBodyEmail(dadosAtivo, alertaAtivoDto.TipoNotificacao);

        try
        {
            MailMessage mailMessage = new(remetente, destinatario, assunto, body);
            smtpClient.Send(mailMessage);

            _logger.LogInformation("Email enviado com sucesso para {}.", destinatario);
        }
        catch (Exception ex)
        {
            _logger.LogError("Ocorreu um erro inesperado ao enviar o email: {}", ex.Message);
        }
        finally
        {
            smtpClient.Dispose();
        }
    }

    private void ValidarEnderecoEmail(string enderecoEmail)
    {
        Regex emailRegex = new Regex(EmailPattern);
        if (!emailRegex.IsMatch(enderecoEmail))
        {
            throw new ParametrosInvalidosException($"O endereço de e-mail informado [{enderecoEmail}] é inválido.");
        }
    }

}
