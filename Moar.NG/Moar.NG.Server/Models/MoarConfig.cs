using System.Text.Json.Serialization;

namespace Moar.NG.Server.Models;

public record MoarConfig
{
    [JsonPropertyName("enableBotSpawning")]
    public required bool EnableBotSpawning { get; set; }
    
    [JsonPropertyName("spawnSmoothing")]
    public required bool SpawnSmoothing { get; set; }
    
    [JsonPropertyName("pmcDifficulty")]
    public required double PmcDifficulty { get; set; }
    
    [JsonPropertyName("scavDifficulty")]
    public required double ScavDifficulty { get; set; }

    [JsonPropertyName("scavWaveDistribution")]
    public required double ScavWaveDistribution { get; set; }
    
    [JsonPropertyName("scavWaveQuantity")]
    public required double ScavWaveQuantity { get; set; }
    
    [JsonPropertyName("startingPmcs")]
    public required bool StartingPmcs { get; set; }
    
    [JsonPropertyName("pmcWaveDistribution")]
    public required double PmcWaveDistribution { get; set; }
    
    [JsonPropertyName("pmcWaveQuantity")]
    public required double PmcWaveQuantity { get; set; }
    
    [JsonPropertyName("randomSpawns")]
    public required bool RandomSpawns { get; set; }
    
    [JsonPropertyName("zombiesEnabled")]
    public required bool ZombiesEnabled { get; set; }
    
    [JsonPropertyName("zombieWaveDistribution")]
    public required double ZombieWaveDistribution { get; set; }
    
    [JsonPropertyName("zombieWaveQuantity")]
    public required double ZombieWaveQuantity { get; set; }
    
    [JsonPropertyName("maxBotCap")]
    public required int MaxBotCap { get; set; }
    
    [JsonPropertyName("maxBotPerZone")]
    public required int MaxBotPerZone { get; set; }
    
    [JsonPropertyName("sniperGroupChance")]
    public required double SniperGroupChance { get; set; }
    
    [JsonPropertyName("scavGroupChance")]
    public required double ScavGroupChance { get; set; }
    
    [JsonPropertyName("pmcGroupChance")]
    public required double PmcGroupChance { get; set; }
    
    [JsonPropertyName("pmcMaxGroupSize")]
    public required int PmcMaxGroupSize { get; set; }
    
    [JsonPropertyName("scavMaxGroupSize")]
    public required int ScavMaxGroupSize { get; set; }
    
    [JsonPropertyName("sniperMaxGroupSize")]
    public required int SniperMaxGroupSize { get; set; }
    
    [JsonPropertyName("bossOpenZones")]
    public required bool BossOpenZones { get; set; }
    
    [JsonPropertyName("randomRaiderGroup")]
    public required bool RandomRaiderGroup { get; set; }
    
    [JsonPropertyName("randomRaiderGroupChance")]
    public required int RandomRaiderGroupChance { get; set; }
    
    [JsonPropertyName("randomRogueGroup")]
    public required bool RandomRogueGroup { get; set; }
    
    [JsonPropertyName("randomRogueGroupChance")]
    public required int RandomRogueGroupChance { get; set; }
    
    [JsonPropertyName("disableBosses")]
    public required bool DisableBosses { get; set; }
    
    [JsonPropertyName("mainBossChanceBuff")]
    public required int MainBossChanceBuff { get; set; }
    
    [JsonPropertyName("bossInvasion")]
    public required bool BossInvasion { get; set; }
    
    [JsonPropertyName("bossInvasionSpawnChance")]
    public required int BossInvasionSpawnChance { get; set; }
    
    [JsonPropertyName("gradualBossInvasion")]
    public required bool GradualBossInvasion { get; set; }
    
    [JsonPropertyName("debug")]
    public required bool Debug { get; set; }
}