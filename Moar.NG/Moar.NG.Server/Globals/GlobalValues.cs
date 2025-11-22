using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using Moar.NG.Server.Models;
using Moar.NG.Server.Stuff;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using static Moar.NG.Server.Stuff.Utils;

namespace Moar.NG.Server.Globals;

[Injectable (InjectionType.Singleton, TypePriority = OnLoadOrder.PreSptModLoader)]
public class GlobalValues(ModHelper modHelper, DatabaseServer databaseServer) : IOnLoad
{
    public static BaseConfig BaseConfig { get; private set; } = null!;
    
    public static AdvancedConfig AdvancedConfig { get; private set; } = null!;
    
    public static Dictionary<string, int> PresetWeightings { get; private set; } = null!;
    
    public static LocationBase[] LocationsBase { get; private set; } = null!;
    
    public static Dictionary<int, SpawnPointParam[]> IndexedMapSpawns { get; private set; } = null!;
    
    public static SpawnPointParam PlayerSpawn { get; private set; } = null!;
    
    public static string CurrentPreset { get; set; } = "";
     
    public static string ForcedPreset { get; private set; } = "random";
    
    public Task OnLoad()
    {
        var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        
        BaseConfig = modHelper.GetJsonDataFromFile<BaseConfig>(modPath, Path.Combine("data", "config.json"));
        AdvancedConfig = modHelper.GetJsonDataFromFile<AdvancedConfig>(modPath, Path.Combine("data", "advancedConfig.json"));
        PresetWeightings = modHelper.GetJsonDataFromFile<Dictionary<string, int>>(modPath, Path.Combine("data", "presetWeightings.json"));

        LoadOrResetLocationsBase(databaseServer);
        
        return Task.CompletedTask;
    }

    public static void LoadOrResetLocationsBase(DatabaseServer databaseServer)
    {
        var locationsDict = databaseServer.GetTables().Locations.GetDictionary();

        LocationsBase = Constants.MapList.Select(mapName => DeepCopy(locationsDict[mapName].Base)).ToArray();
    }
}