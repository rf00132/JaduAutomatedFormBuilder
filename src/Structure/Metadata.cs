using System.Text.Json.Serialization;

namespace JaduFromJson.Structure;
public class Metadata(int id, string title, string instructions, string[] contentPlaceholders)
{
    [JsonPropertyName("id")]
    public int Id { get; private set; } = id;

    [JsonPropertyName("title")]
    public string Title { get; private set; } = title;

    [JsonPropertyName("instructions")]
    public string Instructions { get; private set; } = instructions;

    [JsonPropertyName("contentPlaceholders")]
    public string[] ContentPlaceholders { get; private set; } = contentPlaceholders;
}