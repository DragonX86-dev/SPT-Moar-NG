using Moar.NG.Server.Globals;
using Moar.NG.Server.Stuff;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Servers;

namespace Moar.NG.Server.Extensions;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader)]
public class SpawnsExtension(DatabaseServer databaseServer) : IOnLoad
{
    public Task OnLoad()
    {
        if (GlobalValues.BaseConfig.EnableBotSpawning != true) return Task.CompletedTask;
        
        SetupSpawns();

        return Task.CompletedTask;
    }

    private void SetupSpawns()
    {
        var locationsDict = databaseServer.GetTables().Locations.GetDictionary();

        foreach (var (map, idx) in Constants.MapList.Select((item, i) => (item, i)))
        {
            // Get all unique zones
            var allZones = locationsDict[map]
                .Base
                .SpawnPointParams!
                .Where(point => !string.IsNullOrEmpty(point.BotZoneName))
                .Select(point => point.BotZoneName)
                .Distinct()
                .ToList();

            locationsDict[map].Base.OpenZones = string.Join(",", allZones);

            var bossSpawns = new List<SpawnPointParam>();
            var scavSpawns = new List<SpawnPointParam>();
            var sniperSpawns = new List<SpawnPointParam>();
            var pmcSpawns = new List<SpawnPointParam>();
        }
    }
}