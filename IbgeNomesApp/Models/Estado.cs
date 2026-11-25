using System.Text.Json.Serialization;

namespace IbgeNomesApp.Models;

public class Regiao
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("sigla")]
    public string Sigla { get; set; } = string.Empty;

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;
}

public class Estado
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("sigla")]
    public string Sigla { get; set; } = string.Empty;

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("regiao")]
    public Regiao? Regiao { get; set; }

    // O Picker usa este texto para mostrar cada item na lista
    public override string ToString() => $"{Nome} ({Sigla})";
}