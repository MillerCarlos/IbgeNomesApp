using System.Text.Json.Serialization;

namespace IbgeNomesApp.Models;

public class NomesLocalidade
{
    [JsonPropertyName("localidade")]
    public int Localidade { get; set; }

    [JsonPropertyName("mulheres")]
    public List<Frequencia> Mulheres { get; set; } = new();

    [JsonPropertyName("homens")]
    public List<Frequencia> Homens { get; set; } = new();

    [JsonPropertyName("sobrenomes")]
    public List<Frequencia> Sobrenomes { get; set; } = new();
}