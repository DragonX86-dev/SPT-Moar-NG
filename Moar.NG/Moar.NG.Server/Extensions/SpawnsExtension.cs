using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Servers;
using Moar.NG.Server.Globals;
using SPTarkov.Server.Core.Models.Common;
using static Moar.NG.Server.Stuff.Utils;


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
            
            foreach (var point in Shuffle(locationsDict[map].Base.SpawnPointParams!.ToList()))
            {
                if (point.Categories!.Contains("Boss") || 
                    (point.BotZoneName != null && bossZoneList.Contains(point.BotZoneName)))
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
            
            if (map == "sandbox") {
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
                // Add ```advancedConfig.ActivateSpawnCullingOnServerStart``` block
            }

            var limit = GlobalValues.MapsConfig[map].SpawnMinDistance;
            
            var playerSpawns = BuildCustomPlayerSpawnPoints(
                map,
                locationsDict[map].Base.SpawnPointParams!
            );
            




        }
        
        
    }

    private static IEnumerable<SpawnPointParam> BuildCustomPlayerSpawnPoints(string mapName, IEnumerable<SpawnPointParam> refSpawns)
    {
        var playerOnlySpawns = refSpawns
            .Where(item => !string.IsNullOrEmpty(item.Infiltration) && 
                           item.Categories?.FirstOrDefault() == "Player")
            .Select(point =>
            {
                point.ColliderParams!.Properties!.Radius = 1;
                point.Position!.Y += 0.5f;
            
                // Копируем record с изменениями
                return point with 
                {
                    Id = new MongoId(),
                    BotZoneName = "",
                    Sides = ["Pmc"]
                };
            })
            .ToArray();
        
        if (GlobalValues.PlayerSpawns[mapName].Length == 0) {
            return playerOnlySpawns;
        }

        var playerSpawns = GlobalValues.PlayerSpawns[mapName].Select(point =>
            new SpawnPointParam {
                Id = new MongoId().ToString(),
                BotZoneName = "",
                Categories = ["Player"],
                ColliderParams = new ColliderParams
                {
                    Parent = "SpawnSphereParams",
                    Properties = new ColliderProperties
                    {
                        Center = new XYZ
                        {
                            X = 0,
                            Y = 0,
                            Z = 0
                        },
                        Radius = 1
                    }
                },
                CorePointId = 0,
                DelayToCanSpawnSec = 4,
                Infiltration = GetClosestInfil((double)point.X!, (double)point.Y!, (double)point.Z!),
                Position = point,
                Rotation = Random360(),
                Sides = ["Pmc"]
            }
        );
        
        return playerOnlySpawns.Concat(playerSpawns);

        string GetClosestInfil(double x, double y, double z)
        {
            var selectedInfil = "";
            var closest = double.PositiveInfinity;

            foreach (var spawn in playerOnlySpawns)
            {
                if (string.IsNullOrEmpty(spawn.Infiltration) || spawn.Position == null)
                    continue;

                var pos = spawn.Position;
                var dist = GetDistance(x, y, z, (double)pos.X!, (double)pos.Y!, (double)pos.Z!);

                if (!(dist < closest)) continue;
                
                selectedInfil = spawn.Infiltration;
                closest = dist;
            }

            return selectedInfil;
        }
    }
    
}