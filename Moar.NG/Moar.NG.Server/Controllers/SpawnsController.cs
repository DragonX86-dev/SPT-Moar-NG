using Moar.NG.Server.Globals;
using Moar.NG.Server.Models;
using Moar.NG.Server.Models.Enums;
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
                GlobalValues.ScavSpawnPoints[map] =
                    RemoveClosestSpawnsFromCustomBots(GlobalValues.ScavSpawnPoints, map);
                GlobalValues.PmcSpawnPoints[map] =
                    RemoveClosestSpawnsFromCustomBots(GlobalValues.PmcSpawnPoints, map);
                GlobalValues.PlayerSpawnPoints[map] =
                    RemoveClosestSpawnsFromCustomBots(GlobalValues.PlayerSpawnPoints, map);
                GlobalValues.SniperSpawnPoints[map] =
                    RemoveClosestSpawnsFromCustomBots(GlobalValues.SniperSpawnPoints, map);
            }

            var limit = GlobalValues.MapsConfig[map].SpawnMinDistance;

            var playerSpawns = CleanClosest(
                BuildCustomPlayerSpawnPoints(map, locationsDict[map].Base.SpawnPointParams!.ToList()),
                GlobalValues.MapsConfig[map].MapCullingNearPointValuePlayer
            );

            scavSpawns = CleanClosest(
                AddCustomBotSpawnPoints(scavSpawns, map),
                GlobalValues.MapsConfig[map].MapCullingNearPointValueScav
            ).Select(point =>
            {
                if (point.ColliderParams!.Properties!.Radius < limit)
                {
                    point.ColliderParams.Properties.Radius = limit;
                }

                return point.Categories!.Any() ? point with
                {
                    BotZoneName = point.BotZoneName,
                    Categories = ["Bot"],
                    Sides = ["Savage"],
                    CorePointId = 1,
                } : point;
            }).ToList();
            
            pmcSpawns = CleanClosest(
                AddCustomPmcSpawnPoints(pmcSpawns, map),
                GlobalValues.MapsConfig[map].MapCullingNearPointValuePmc
            ).Select(point =>
            {
                if (point.ColliderParams!.Properties!.Radius < limit)
                {
                    point.ColliderParams.Properties.Radius = limit;
                }

                return point.Categories!.Any() ? point with
                {
                    BotZoneName = GetClosestZone(scavSpawns, point.Position!),
                    Categories = ["Coop", new Random().NextDouble() < 0.5 ? "Group" : "Opposite"],
                    Sides = ["Pmc"],
                    CorePointId = 0
                } : point;
            }).ToList();
            
            sniperSpawns = AddCustomSniperSpawnPoints(sniperSpawns, map);

            var mapSpawn = 
            sniperSpawns.Select(e => new MoarSpawnPoint
            {
                SpawnPointParam = e,
                Type = SpawnType.Sniper
            }).Concat(pmcSpawns.Select(e => new MoarSpawnPoint
            {
                SpawnPointParam = e,
                Type = SpawnType.Pmc
            })).Concat(scavSpawns.Select(e => new MoarSpawnPoint
            {
                SpawnPointParam = e,
                Type = SpawnType.Scavage
            })).Concat(playerSpawns.Select(e => new MoarSpawnPoint
            {
                SpawnPointParam = e,
                Type = SpawnType.Player
            })).Concat(bossSpawns.Select(e => new MoarSpawnPoint
            {
                SpawnPointParam = e,
                Type = SpawnType.Boss
            })).ToArray();
            
            locationsDict[map].Base.SpawnPointParams = mapSpawn.Select(e => e.SpawnPointParam).ToList();
            GlobalValues.NamedMapSpawns[map] = mapSpawn;
        }
    }
}