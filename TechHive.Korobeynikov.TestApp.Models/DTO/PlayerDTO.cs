using System.Text.Json.Serialization;

namespace TechHive.Korobeynikov.TestApp.Models.DTO;

public class PlayerDTO : SerializedDTO
{
    [JsonPropertyName("udid"), JsonRequired]
    public string UDID { get; set; } = null!;

    [JsonPropertyName("playerId")]
    public Guid? PlayerId { get; set; }

    [JsonPropertyName("resource")]
    public List<ResourceDTO>? Resources { get; set; }
}
