using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MonitorAtivosB3.Conf;
using MonitorAtivosB3.Entidades;
using MonitorAtivosB3.Excecoes;

namespace MonitorAtivosB3.Services.AcessoDadosAtivos;

public class ProvedorBraveApi : IProvedorDadosAtivos
{
    private readonly ILogger<ProvedorBraveApi> _logger;

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializer;

    public ProvedorBraveApi(ILogger<ProvedorBraveApi> logger, IOptions<ConfiguracaoBraveApi> configuracaoBraveApi)
    {
        _logger = logger;

        _httpClient = new HttpClient { BaseAddress = new Uri(configuracaoBraveApi.Value.BaseUrl) };
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", configuracaoBraveApi.Value.Token);

        _jsonSerializer = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<DadosAtivo?> ObterDadosAtivoAsync(string codigoAtivo)
    {
        try
        {
            _logger.LogDebug("Solicitando dados de [{}]...", codigoAtivo);
            var response = await _httpClient.GetAsync($"https://brapi.dev/api/quote/{codigoAtivo}");
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var resultadoApi = JsonSerializer.Deserialize<ResultadoDadosAtivo>(json, _jsonSerializer);

                return resultadoApi?.Resultados?.FirstOrDefault();
            }

            var statusCode = response.StatusCode;
            if (statusCode == HttpStatusCode.NotFound)
            {
                var msgErro = $"Ativo [{codigoAtivo}] não encontrado.";
                _logger.LogError(msgErro);

                throw new ParametrosInvalidosException(msgErro);
            }

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("message", out var msg))
            {
                _logger.LogError("Erro da API: ({}) - {}", statusCode, msg.GetString());
                return null;
            }

            response.EnsureSuccessStatusCode();
        }
        catch (ParametrosInvalidosException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Ocorreu um erro inesperado ao solicitar os dados do ativo [{}]: {}]", codigoAtivo, ex.Message
            );
        }

        return null;
    }
}
