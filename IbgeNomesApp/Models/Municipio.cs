using System.Text.Json.Serialization;

namespace IbgeNomesApp.Models;

public class Municipio
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    public override string ToString() => Nome;
}