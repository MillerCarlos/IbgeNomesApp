using IbgeNomesApp.Models;

namespace IbgeNomesApp.Services;

public class NomesService
{
    private const string UrlBase = "https://servicodados.ibge.gov.br/api/v3/nomes/2022/localidade";

    private readonly IbgeApiClient _api = new();

    // Busca os nomes mais comuns de uma localidade (id de estado ou de município).
    // "quantidade" é quantos nomes mostrar em cada lista.
    public async Task<NomesLocalidade> BuscarNomesAsync(int idLocalidade, int quantidade = 10)
    {
        var dados = await _api.GetAsync<NomesLocalidade>($"{UrlBase}/{idLocalidade}");

        if (dados.EstaVazio)
        {
            throw new ServicoException(
                "O IBGE não tem dados de nomes para essa localidade.");
        }

        // Processamento: ordena do mais frequente para o menos e corta no limite.
        dados.Homens = Top(dados.Homens, quantidade);
        dados.Mulheres = Top(dados.Mulheres, quantidade);
        dados.Sobrenomes = Top(dados.Sobrenomes, quantidade);

        return dados;
    }

    private static List<Frequencia> Top(List<Frequencia> lista, int quantidade)
    {
        return lista
            .OrderByDescending(f => f.Quantidade)
            .Take(quantidade)
            .ToList();
    }
}