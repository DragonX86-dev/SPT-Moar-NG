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
    public static BaseConfig BaseConfig { get; private set; } = null!;
    
    public static AdvancedConfig AdvancedConfig { get; private set; } = null!;
    
    public static Dictionary<string, int> PresetWeightings { get; private set; } = new();
    
    public static Dictionary<string, BaseConfig> Presets { get; private set; } = new();
    
    public static Dictionary<string, MapConfig> MapsConfig { get; private set; } = new();
    
    public static Dictionary<string, XYZ[]> PlayerSpawns { get; private set; } = new();
    
    public static Dictionary<string, XYZ[]> PmcSpawns { get; private set; } = new();
    
    public static Dictionary<string, XYZ[]> ScavSpawns { get; private set; } = new();
    
    public static Dictionary<string, XYZ[]> SniperSpawns { get; private set; } = new();
    
    public static HashSet<string> BossZoneList { get; private set; } = [];
    
    public static string[] MapList { get; private set; } = [];
    
    public static Dictionary<int, SpawnPointParam[]> IndexedMapSpawns { get; private set; } = new();
    
    public static SpawnPointParam PlayerSpawn { get; private set; } = null!;
    
    public static string CurrentPreset { get; set; } = "";
     
    public static string ForcedPreset { get; private set; } = "random";
    
    public Task OnLoad()
    {
        var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        
        BaseConfig = modHelper.GetJsonDataFromFile<BaseConfig>(modPath, Path.Combine("data", "config.json"));
        AdvancedConfig = modHelper.GetJsonDataFromFile<AdvancedConfig>(modPath, Path.Combine("data", "advancedConfig.json"));
        PresetWeightings = modHelper.GetJsonDataFromFile<Dictionary<string, int>>(modPath, Path.Combine("data", "presetWeightings.json"));
        Presets = modHelper.GetJsonDataFromFile<Dictionary<string, BaseConfig>>(modPath, Path.Combine("data", "presets.json"));
        MapsConfig = modHelper.GetJsonDataFromFile<Dictionary<string, MapConfig>>(modPath, Path.Combine("data", "mapConfig.json"));
        PlayerSpawns = modHelper.GetJsonDataFromFile<Dictionary<string, XYZ[]>>(modPath, Path.Combine("data", "playerSpawns.json"));
        PmcSpawns = modHelper.GetJsonDataFromFile<Dictionary<string, XYZ[]>>(modPath, Path.Combine("data", "pmcSpawns.json"));
        ScavSpawns = modHelper.GetJsonDataFromFile<Dictionary<string, XYZ[]>>(modPath, Path.Combine("data", "scavSpawns.json"));
        SniperSpawns = modHelper.GetJsonDataFromFile<Dictionary<string, XYZ[]>>(modPath, Path.Combine("data", "sniperSpawns.json"));
        BossZoneList = modHelper.GetJsonDataFromFile<HashSet<string>>(modPath, Path.Combine("data", "bossZoneList.json"));
        MapList = modHelper.GetJsonDataFromFile<string[]>(modPath, Path.Combine("data", "mapList.json"));
            
        return Task.CompletedTask;
    }
}