using System.Net;
using System.Text.Json;
using Microsoft.Maui.Networking;

namespace IbgeNomesApp.Services;

public class IbgeApiClient
{
    // Um único HttpClient para o app inteiro (é a forma recomendada).
    private static readonly HttpClient _http = new()
    {
        Timeout = TimeSpan.FromSeconds(20)
    };

    private static readonly JsonSerializerOptions _opcoesJson = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Busca a URL e converte o JSON para o tipo T (ex.: List<Estado>).
    public async Task<T> GetAsync<T>(string url)
    {
        // Erro 1: sem internet
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            throw new ServicoException(
                "Sem conexão com a internet. Verifique sua rede e tente novamente.");
        }

        try
        {
            using var resposta = await _http.GetAsync(url);

            // Erro 2: API respondeu com falha
            if (!resposta.IsSuccessStatusCode)
            {
                int codigo = (int)resposta.StatusCode;

                if (resposta.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ServicoException(
                        "Nenhum dado encontrado para essa localidade.");
                }

                if (codigo >= 500)
                {
                    throw new ServicoException(
                        "A API do IBGE está fora do ar no momento. Tente novamente mais tarde.");
                }

                throw new ServicoException(
                    $"Erro ao consultar a API do IBGE (código {codigo}).");
            }

            string json = await resposta.Content.ReadAsStringAsync();

            // Erro 3: resposta vazia
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ServicoException("A API do IBGE retornou uma resposta vazia.");
            }

            T? dados = JsonSerializer.Deserialize<T>(json, _opcoesJson);

            if (dados is null)
            {
                throw new ServicoException("A API do IBGE retornou dados inválidos.");
            }

            return dados;
        }
        catch (ServicoException)
        {
            throw; // já é um erro nosso, só repassa
        }
        catch (TaskCanceledException)
        {
            // O tempo limite de 20 segundos acabou
            throw new ServicoException(
                "A consulta demorou demais. Verifique sua conexão e tente novamente.");
        }
        catch (HttpRequestException)
        {
            throw new ServicoException(
                "Não foi possível conectar à API do IBGE. Verifique sua conexão.");
        }
        catch (JsonException)
        {
            throw new ServicoException(
                "A resposta da API veio em um formato inesperado.");
        }
    }
}