using System.Text.Json.Serialization;

namespace TechHive.Korobeynikov.TestApp.Models.DTO;

public class Message : SerializedDTO
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;
    [JsonPropertyName("payload")]
    public string? Payload { get; set; } = null!;
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("info")]
    public string? Info { get; set; }
}
