using System.Text.Json.Serialization;

namespace Moar.NG.Server.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<SpawnType>))]
public enum SpawnType
{
    [JsonStringEnumMemberName("player")]
    Player,
    [JsonStringEnumMemberName("pmc")]
    Pmc,
    [JsonStringEnumMemberName("boss")]
    Boss,
    [JsonStringEnumMemberName("scav")]
    Scavage,
    [JsonStringEnumMemberName("sniper")]
    Sniper
}