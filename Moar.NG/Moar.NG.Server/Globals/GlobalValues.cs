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
    
    public static HashSet<string> BossZoneList { get; private set; } = [];
    
    public static string CurrentPreset { get; set; } = "";
     
    
    public Task OnLoad()
    {
        var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        
        MoarConfig = modHelper.GetJsonDataFromFile<MoarConfig>(modPath, Path.Combine("data", "config.json"));
        AdvancedConfig = modHelper.GetJsonDataFromFile<AdvancedConfig>(modPath, Path.Combine("data", "advancedConfig.json"));
        PresetWeightings = modHelper.GetJsonDataFromFile<Dictionary<string, int>>(modPath, Path.Combine("data", "presetWeightings.json"));
        Presets = modHelper.GetJsonDataFromFile<Dictionary<string, Dictionary<string, dynamic>>>(modPath, Path.Combine("data", "presets.json"));
        MapsConfig = modHelper.GetJsonDataFromFile<Dictionary<string, MapConfig>>(modPath, Path.Combine("data", "mapConfig.json"));
        BossZoneList = modHelper.GetJsonDataFromFile<HashSet<string>>(modPath, Path.Combine("data", "bossZoneList.json"));
            
        return Task.CompletedTask;
    }
}