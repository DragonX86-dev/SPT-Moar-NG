using System.Text.Json.Serialization;
using Moar.NG.Server.Routes.Models.Enums;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Utils;

namespace Moar.NG.Server.Routes.Models;

public class AddSpawnRequestData : IRequestData
{
    [JsonPropertyName("map")]
    public required string Map { get; set; }

    [JsonPropertyName("position")]
    public required XYZ Position { get; set; }
    
    [JsonPropertyName("type")]
    public required SpawnType SpawnType { get; set; }
}