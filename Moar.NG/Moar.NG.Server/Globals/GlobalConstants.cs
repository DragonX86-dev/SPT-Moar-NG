using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace Moar.NG.Server.Globals;

[Injectable(InjectionType.Singleton, TypePriority = OnLoadOrder.PreSptModLoader)]
public class GlobalConstants(ModHelper modHelper) : IOnLoad
{
    public static string[] MapList { get; private set; } = [];
    public static string[] BossNameList { get; private set; } = [];
    
    public static IEnumerable<AdditionalHostilitySettings> DefaultHostility { get; private set; } = [];
    
    public static string ForcedPreset { get; private set; } = "random";


    public Task OnLoad()
    {
        var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

        MapList = modHelper.GetJsonDataFromFile<string[]>(modPath, Path.Combine("data", "mapList.json"));
        BossNameList = modHelper.GetJsonDataFromFile<string[]>(modPath, Path.Combine("data", "bossList.json"));

        return Task.CompletedTask;
    }
}