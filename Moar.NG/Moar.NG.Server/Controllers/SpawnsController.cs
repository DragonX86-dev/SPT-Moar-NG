using Moar.NG.Server.Globals;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using static Moar.NG.Server.Utils.CommonUtils;
using static Moar.NG.Server.Utils.SpawnUtils;

namespace Moar.NG.Server.Controllers;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader)]
public class SpawnsController(
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

        foreach (var map in GlobalConstants.MapList)
        {
            var bossSpawns = new List<SpawnPointParam>();
            var scavSpawns = new List<SpawnPointParam>();
            var sniperSpawns = new List<SpawnPointParam>();
            var pmcSpawns = new List<SpawnPointParam>();

            var bossZoneList = GlobalConstants.BossZoneList;

            foreach (var point in Shuffle(locationsDict[map].Base.SpawnPointParams!))
            {
                if (point.Categories!.Contains("Boss") || bossZoneList.Contains(point.BotZoneName!))
                {
                    bossSpawns.Add(point);
                }
                else if (point.BotZoneName!.Contains("snipe", StringComparison.CurrentCultureIgnoreCase) 
                         || (map != "lighthouse" && point.DelayToCanSpawnSec > 40))
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

            if (GlobalValues.AdvancedConfig.ActivateSpawnCullingOnServerStart)
            {
                GlobalSpawnData.ScavSpawnPoints[map] =
                    RemoveClosestSpawnsFromCustomBots(GlobalSpawnData.ScavSpawnPoints, map);
                GlobalSpawnData.PmcSpawnPoints[map] =
                    RemoveClosestSpawnsFromCustomBots(GlobalSpawnData.PmcSpawnPoints, map);
                GlobalSpawnData.PlayerSpawnPoints[map] =
                    RemoveClosestSpawnsFromCustomBots(GlobalSpawnData.PlayerSpawnPoints, map);
                GlobalSpawnData.SniperSpawnPoints[map] =
                    RemoveClosestSpawnsFromCustomBots(GlobalSpawnData.SniperSpawnPoints, map);
            }

            var limit = GlobalValues.MapConfig[map].SpawnMinDistance;

            var playerSpawns = CleanClosest(
                BuildCustomPlayerSpawnPoints(map, locationsDict[map].Base.SpawnPointParams!.ToList()),
                GlobalValues.MapConfig[map].MapCullingNearPointValuePlayer
            );

            scavSpawns = CleanClosest(
                AddCustomBotSpawnPoints(scavSpawns, map),
                GlobalValues.MapConfig[map].MapCullingNearPointValueScav
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
                    CorePointId = 1
                } : point;
            }).ToList();
            
            pmcSpawns = CleanClosest(
                AddCustomPmcSpawnPoints(pmcSpawns, map),
                GlobalValues.MapConfig[map].MapCullingNearPointValuePmc
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
            
            GlobalSpawnData.PlayerMapSpawns[map] = playerSpawns.ToArray();
            GlobalSpawnData.PmcMapSpawns[map] = pmcSpawns.ToArray();
            GlobalSpawnData.ScavMapSpawns[map] = scavSpawns.ToArray();
            GlobalSpawnData.BossMapSpawns[map] = bossSpawns.ToArray();
            GlobalSpawnData.SniperMapSpawns[map] = sniperSpawns.ToArray();
            
            locationsDict[map].Base.SpawnPointParams = sniperSpawns
                .Concat(pmcSpawns).Concat(scavSpawns).Concat(playerSpawns).Concat(bossSpawns).ToArray();
            
            // Get all unique zones
            var allZones = locationsDict[map]
                .Base
                .SpawnPointParams!
                .Where(point => !string.IsNullOrEmpty(point.BotZoneName))
                .Select(point => point.BotZoneName)
                .Distinct()
                .ToList();

            locationsDict[map].Base.OpenZones = string.Join(",", allZones);
        }
    }
}