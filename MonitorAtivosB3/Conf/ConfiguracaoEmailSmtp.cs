namespace MonitorAtivosB3.Conf;

public class ConfiguracaoEmailSmtp
{
    public required string Server { get; init; }
    public required int Port { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public bool EnableSsl { get; init; }


    public required string EmailRemetente { get; init; }
    public required string EmailDestinatario { get; init; }
}
