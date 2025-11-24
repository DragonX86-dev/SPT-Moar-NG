using Moar.NG.Server.Globals;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;
using static Moar.NG.Server.Stuff.Utils.CommonUtils;
using static Moar.NG.Server.Stuff.Utils.SpawnUtils;

namespace Moar.NG.Server.Controllers;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader)]
public class SpawnsController(
    JsonUtil jsonUtil,
    DatabaseServer databaseServer,
    ISptLogger<SpawnsController> logger) : IOnLoad
{
    public Task OnLoad()
    {
        if (!GlobalValues.MoarConfig.EnableBotSpawning) return Task.CompletedTask;

        SetupSpawns();

        return Task.CompletedTask;
    }

    private void SetupSpawns()
    {
        var locationsDict = databaseServer.GetTables().Locations.GetDictionary();
        var indexedMapSpawns = new Dictionary<int, SpawnPointParam[]>();

        foreach (var (map, idx) in GlobalValues.MapList.Select((item, i) => (item, i)))
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

            var bossZoneList = GlobalValues.BossZoneList;

            foreach (var point in Shuffle(locationsDict[map].Base.SpawnPointParams!))
            {
                if (point.Categories!.Contains("Boss") || bossZoneList.Contains(point.BotZoneName!))
                {
                    bossSpawns.Add(point);
                }
                else if (point.BotZoneName!.Contains("snipe") || (map != "lighthouse" && point.DelayToCanSpawnSec > 40))
                {
                    sniperSpawns.Add(point);
                }
                else if (!string.IsNullOrEmpty(point.Infiltration) || point.Categories!.Contains("Coop"))
                {
                    pmcSpawns.Add(point);
                }
                else
                {
                    scavSpawns.Add(point);
                }
            }

            // Вынести в конфиг спавна снайперов
            if (map.Contains("Sandbox"))
            {
                var random = new Random();
                var zones = new[] { "ZoneSandSnipeCenter", "ZoneSandSnipeCenter2" };

                for (var i = 0; i < sniperSpawns.Count; i++)
                {
                    var point = sniperSpawns[i];

                    if (i < 2)
                    {
                        point.BotZoneName = random.Next(2) == 0 ? zones[0] : zones[1];
                    }
                    else
                    {
                        point.BotZoneName = zones[i % zones.Length];
                    }

                    sniperSpawns[i] = point;
                }
            }

            if (GlobalValues.AdvancedConfig.ActivateSpawnCullingOnServerStart)
            {
                GlobalValues.ScavSpawns[map] =
                    RemoveClosestSpawnsFromCustomBots(GlobalValues.ScavSpawns, scavSpawns, map);
                GlobalValues.PmcSpawns[map] =
                    RemoveClosestSpawnsFromCustomBots(GlobalValues.PmcSpawns, pmcSpawns, map);
                GlobalValues.PlayerSpawns[map] =
                    RemoveClosestSpawnsFromCustomBots(GlobalValues.PlayerSpawns, pmcSpawns, map);
                GlobalValues.SniperSpawns[map] =
                    RemoveClosestSpawnsFromCustomBots(GlobalValues.SniperSpawns, sniperSpawns, map);
            }

            var limit = GlobalValues.MapsConfig[map].SpawnMinDistance;

            var playerSpawns = CleanClosest(
                BuildCustomPlayerSpawnPoints(map, locationsDict[map].Base.SpawnPointParams!.ToArray()),
                GlobalValues.MapsConfig[map].MapCullingNearPointValuePlayer
            );

            scavSpawns = CleanClosest(
                AddCustomBotSpawnPoints(scavSpawns.ToArray(), map),
                GlobalValues.MapsConfig[map].MapCullingNearPointValueScav
            ).Select(point =>
            {
                if (point.ColliderParams!.Properties!.Radius < limit)
                {
                    point.ColliderParams.Properties.Radius = limit;
                }

                return point.Categories!.Any() ? point with
                {
                    Categories = ["Bot"],
                    Sides = ["Savage"],
                    CorePointId =1,
                } : point;
            }).ToList();
            

        }
    }
}