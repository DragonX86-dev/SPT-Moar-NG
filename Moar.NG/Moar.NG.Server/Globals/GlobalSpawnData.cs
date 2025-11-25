using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace Moar.NG.Server.Globals;

[Injectable(InjectionType.Singleton, TypePriority = OnLoadOrder.PreSptModLoader)]
public class GlobalSpawnData(ModHelper modHelper) : IOnLoad
{
    public static Dictionary<string, XYZ[]> PlayerSpawnPoints { get; private set; } = new();
    
    public static Dictionary<string, XYZ[]> PmcSpawnPoints { get; private set; } = new();
    
    public static Dictionary<string, XYZ[]> ScavSpawnPoints { get; private set; } = new();
    
    public static Dictionary<string, XYZ[]> SniperSpawnPoints { get; private set; } = new();
    
    public static Dictionary<string, SpawnPointParam[]> PlayerMapSpawns { get; private set; } = new();
    
    public static Dictionary<string, SpawnPointParam[]> PmcMapSpawns { get; private set; } = new();
    
    public static Dictionary<string, SpawnPointParam[]> ScavMapSpawns { get; private set; } = new();
    
    public static Dictionary<string, SpawnPointParam[]> BossMapSpawns { get; private set; } = new();
    
    public static Dictionary<string, SpawnPointParam[]> SniperMapSpawns { get; private set; } = new();
    
    public static SpawnPointParam PlayerSpawn { get; set; } = null!;
    
    public Task OnLoad()
    {
        var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

        PlayerSpawnPoints = modHelper.GetJsonDataFromFile<Dictionary<string, XYZ[]>>(modPath, Path.Combine("data", "playerSpawns.json"));
        PmcSpawnPoints = modHelper.GetJsonDataFromFile<Dictionary<string, XYZ[]>>(modPath, Path.Combine("data", "pmcSpawns.json"));
        ScavSpawnPoints = modHelper.GetJsonDataFromFile<Dictionary<string, XYZ[]>>(modPath, Path.Combine("data", "scavSpawns.json"));
        SniperSpawnPoints = modHelper.GetJsonDataFromFile<Dictionary<string, XYZ[]>>(modPath, Path.Combine("data", "sniperSpawns.json"));
        
        return Task.CompletedTask;
    }
}