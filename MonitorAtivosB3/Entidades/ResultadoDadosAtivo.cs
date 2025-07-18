using System.Text.Json.Serialization;

namespace MonitorAtivosB3.Entidades;

public class ResultadoDadosAtivo
{
    [JsonPropertyName("results")]
    public List<DadosAtivo>? Resultados { get; init; }
}
