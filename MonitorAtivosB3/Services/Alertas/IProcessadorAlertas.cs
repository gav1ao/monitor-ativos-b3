using MonitorAtivosB3.Entidades;

namespace MonitorAtivosB3.Services.Alertas;

public interface IProcessadorAlertas
{
    Task EnviarAlertaAsync(AlertaAtivoDto alertaAtivoDto);
}
