using System.Text.Json.Serialization;

namespace TechHive.Korobeynikov.TestApp.Models.DTO;

public class ResourceDTO : SerializedDTO
{
    [JsonPropertyName("type"), JsonRequired]
    public EResourceType Type { get; set; }

    [JsonPropertyName("value"), JsonRequired]
    public int Value { get; set; }
}