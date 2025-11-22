using System.Text.Json.Serialization;

namespace Moar.NG.Server.Models;

public record AdvancedConfig
{
    [JsonPropertyName("activate_spawn_culling_on_server_start")]
    public required bool ActivateSpawnCullingOnServerStart { get; set; }

    [JsonPropertyName("marksman_difficulty_changes")]
    public required bool MarksmanDifficultyChanges { get; set; }
    
    [JsonPropertyName("enable_boss_performance_improvements")]
    public required bool EnableBossPerformanceImprovements { get; set; }
    
    [JsonPropertyName("spawn_point_area_target")]
    public required int SpawnPointAreaTarget { get; set; }
    
    [JsonPropertyName("show_map_culling_debug")]
    public required bool ShowMapCullingDebug { get; set; }
}