using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using Moar.NG.Server.Models;
using Moar.NG.Server.Stuff;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Servers;
using static Moar.NG.Server.Stuff.Utils;

namespace Moar.NG.Server.Globals;

[Injectable (InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader)]
public class GlobalValues(ModHelper modHelper) : IOnLoad
{
    public static BaseConfig BaseConfig { get; private set; } = null!;
    
    public static AdvancedConfig AdvancedConfig { get; private set; } = null!;
    
    public static Dictionary<string, BaseConfig> Presets { get; private set; } = new();
    
    public static Dictionary<string, int> PresetWeightings { get; private set; } = new();
    
    public static Dictionary<int, SpawnPointParam[]> IndexedMapSpawns { get; private set; } = new();
    
    public static SpawnPointParam PlayerSpawn { get; private set; } = null!;
    
    public static string CurrentPreset { get; set; } = "";
     
    public static string ForcedPreset { get; private set; } = "random";
    
    public Task OnLoad()
    {
        var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        
        BaseConfig = modHelper.GetJsonDataFromFile<BaseConfig>(modPath, Path.Combine("Data", "config.json"));
        AdvancedConfig = modHelper.GetJsonDataFromFile<AdvancedConfig>(modPath, Path.Combine("Data", "advancedConfig.json"));
        PresetWeightings = modHelper.GetJsonDataFromFile<Dictionary<string, int>>(modPath, Path.Combine("Data", "presetWeightings.json"));
        Presets = modHelper.GetJsonDataFromFile<Dictionary<string, BaseConfig>>(modPath, Path.Combine("Data", "presets.json"));
        
        return Task.CompletedTask;
    }
}