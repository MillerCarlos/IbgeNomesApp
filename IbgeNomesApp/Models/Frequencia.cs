using System.Globalization;
using System.Text.Json.Serialization;

namespace IbgeNomesApp.Models;

public class Frequencia
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("frequencia")]
    public long Quantidade { get; set; }

    [JsonIgnore]
    public string NomeFormatado =>
        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Nome.ToLower());

    [JsonIgnore]
    public string Descricao => $"{NomeFormatado} ({Quantidade:N0})";
}