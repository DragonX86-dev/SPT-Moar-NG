using System.Collections.ObjectModel;
using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using Moar.NG.Server.Models;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace Moar.NG.Server.Globals;

[Injectable (InjectionType.Singleton, TypePriority = OnLoadOrder.PreSptModLoader)]
public class GlobalValues(ModHelper modHelper) : IOnLoad
{
    public static MoarConfig MoarConfig { get; private set; } = null!;
    
    public static AdvancedConfig AdvancedConfig { get; private set; } = null!;
    
    public static Dictionary<string, int> PresetWeightings { get; private set; } = new();
    
    public static Dictionary<string, Dictionary<string, dynamic>> Presets { get; private set; } = new();
    
    public static Dictionary<string, MapConfig> MapsConfig { get; private set; } = new();
    
    public static Dictionary<string, XYZ[]> PlayerSpawnPoints { get; private set; } = new();
    
    
    public static Dictionary<string, XYZ[]> PmcSpawnPoints { get; private set; } = new();
    
    public static Dictionary<string, XYZ[]> ScavSpawnPoints { get; private set; } = new();
    
    
    public static Dictionary<string, XYZ[]> SniperSpawnPoints { get; private set; } = new();
    
    
    public static HashSet<string> BossZoneList { get; private set; } = [];
    
    public static string[] MapList { get; private set; } = [];
    
    public static Dictionary<string, MoarSpawnPoint[]> NamedMapSpawns { get; private set; } = new();
    
    public static SpawnPointParam PlayerSpawn { get; private set; } = null!;
    
    public static string CurrentPreset { get; set; } = "";
     
    public static string ForcedPreset { get; private set; } = "random";
    
    public Task OnLoad()
    {
        var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        
        MoarConfig = modHelper.GetJsonDataFromFile<MoarConfig>(modPath, Path.Combine("data", "config.json"));
        AdvancedConfig = modHelper.GetJsonDataFromFile<AdvancedConfig>(modPath, Path.Combine("data", "advancedConfig.json"));
        PresetWeightings = modHelper.GetJsonDataFromFile<Dictionary<string, int>>(modPath, Path.Combine("data", "presetWeightings.json"));
        Presets = modHelper.GetJsonDataFromFile<Dictionary<string, Dictionary<string, dynamic>>>(modPath, Path.Combine("data", "presets.json"));
        MapsConfig = modHelper.GetJsonDataFromFile<Dictionary<string, MapConfig>>(modPath, Path.Combine("data", "mapConfig.json"));
        PlayerSpawnPoints = modHelper.GetJsonDataFromFile<Dictionary<string, XYZ[]>>(modPath, Path.Combine("data", "playerSpawns.json"));
        PmcSpawnPoints = modHelper.GetJsonDataFromFile<Dictionary<string, XYZ[]>>(modPath, Path.Combine("data", "pmcSpawns.json"));
        ScavSpawnPoints = modHelper.GetJsonDataFromFile<Dictionary<string, XYZ[]>>(modPath, Path.Combine("data", "scavSpawns.json"));
        SniperSpawnPoints = modHelper.GetJsonDataFromFile<Dictionary<string, XYZ[]>>(modPath, Path.Combine("data", "sniperSpawns.json"));
        BossZoneList = modHelper.GetJsonDataFromFile<HashSet<string>>(modPath, Path.Combine("data", "bossZoneList.json"));
        MapList = modHelper.GetJsonDataFromFile<string[]>(modPath, Path.Combine("data", "mapList.json"));
            
        return Task.CompletedTask;
    }
}