using IbgeNomesApp.Models;

namespace IbgeNomesApp.Services;

public class LocalidadesService
{
    private const string UrlBase = "https://servicodados.ibge.gov.br/api/v1/localidades";

    private readonly IbgeApiClient _api = new();

    // Lista os 27 estados, em ordem alfabética.
    public async Task<List<Estado>> BuscarEstadosAsync()
    {
        var estados = await _api.GetAsync<List<Estado>>($"{UrlBase}/estados?orderBy=nome");

        if (estados.Count == 0)
        {
            throw new ServicoException("A API não retornou nenhum estado.");
        }

        return estados;
    }

    // Lista os municípios de UM estado (a lista do Brasil inteiro tem mais de 5.500).
    public async Task<List<Municipio>> BuscarMunicipiosAsync(int estadoId)
    {
        var municipios = await _api.GetAsync<List<Municipio>>(
            $"{UrlBase}/estados/{estadoId}/municipios?orderBy=nome");

        if (municipios.Count == 0)
        {
            throw new ServicoException("A API não retornou nenhum município para esse estado.");
        }

        return municipios;
    }
}