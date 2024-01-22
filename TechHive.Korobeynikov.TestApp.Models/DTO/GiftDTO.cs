using System.Text.Json.Serialization;

namespace TechHive.Korobeynikov.TestApp.Models.DTO;

public class GiftDTO : SerializedDTO
{
    [JsonPropertyName("playerId"), JsonRequired]
    public Guid PlayerId { get; set; }

    [JsonPropertyName("friendId"), JsonRequired]
    public Guid FriendId { get; set; }

    [JsonPropertyName("resource"), JsonRequired]
    public ResourceDTO Resource { get; set; } = null!;
}
