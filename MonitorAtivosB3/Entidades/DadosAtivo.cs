using System.Text.Json.Serialization;

namespace MonitorAtivosB3.Entidades;

public class DadosAtivo
{
    [JsonPropertyName("longName")]
    public string? NomeAtivo { get; set; }

    [JsonPropertyName("currency")]
    public string? Moeda { get; set; }

    [JsonPropertyName("regularMarketPrice")]
    public decimal? Preco { get; set; }

}
