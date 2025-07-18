using MonitorAtivosB3.Entidades;

namespace MonitorAtivosB3.Services.AcessoDadosAtivos;

public interface IProvedorDadosAtivos
{
    Task<DadosAtivo?> ObterDadosAtivoAsync(string codigoAtivo);
}
