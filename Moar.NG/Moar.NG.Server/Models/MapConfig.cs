using System.Text.Json.Serialization;

namespace Moar.NG.Server.Models;

public record MapConfig
{
    [JsonPropertyName("sniperQuantity")]
    public required int SniperQuantity { get; init; }
    
    [JsonPropertyName("initialSpawnDelay")]
    public required int InitialSpawnDelay { get; init; }
    
    [JsonPropertyName("smoothingDistribution")]
    public required double SmoothingDistribution { get; init; }
    
    [JsonPropertyName("mapCullingNearPointValuePlayer")]
    public required int MapCullingNearPointValuePlayer { get; init; }
    
    [JsonPropertyName("mapCullingNearPointValuePmc")]
    public required int MapCullingNearPointValuePmc { get; init; }
    
    [JsonPropertyName("mapCullingNearPointValueScav")]
    public required int MapCullingNearPointValueScav { get; init; }
    
    [JsonPropertyName("spawnMinDistance")]
    public required int SpawnMinDistance { get; init; }
    
    [JsonPropertyName("pmcWaveCount")]
    public required int PmcWaveCount { get; init; }
    
    [JsonPropertyName("scavWaveCount")]
    public required int ScavWaveCount { get; init; }
    
    [JsonPropertyName("zombieWaveCount")]
    public required int ZombieWaveCount { get; init; }

    [JsonPropertyName("scavHotZones")]
    public required string[] ScavHotZones { get; init; }
    
    [JsonPropertyName("pmcHotZones")]
    public required string[] PmcHotZones { get; init; }
}