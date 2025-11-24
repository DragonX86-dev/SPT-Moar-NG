using System.Numerics;
using Moar.NG.Server.Globals;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace Moar.NG.Server.Stuff.Spawn;

public static class SpawnZoneUtils
{
    private static double Random360()
    {
        var random = new Random();
        return random.NextDouble() * 360;
    }

    private static float GetDistance(XYZ first, XYZ second)
    {
        var point1 = new Vector3((float)first.X!, (float)first.Y!, (float)first.Z!);
        var point2 = new Vector3((float)second.X!, (float)second.Y!, (float)second.Z!);
    
        return Vector3.Distance(point1, point2);
    }

    public static SpawnPointParam[] GetSortedSpawnPointList(SpawnPointParam[] spawnPointParams, XYZ coords, 
        float cull = 0.0f)
    {
        var sorted = spawnPointParams
            .OrderBy(sp => GetDistance(sp.Position!, coords))
            .Where((_, index) =>
            {
                if (cull == 0.0f) return true;
                return index > spawnPointParams.Length * cull;
            })
            .ToList();
        
        return sorted.ToArray();
    }
    
    public static SpawnPointParam[] CleanClosest(SpawnPointParam[] spawnPointParams, int mapCullingNearPointValue)
    {
        var okayList = new HashSet<string>();
        var filteredParams = spawnPointParams.Select(point =>
        {
            var hasClosePoint = spawnPointParams.Any(other =>
            {
                var dist = GetDistance(point.Position!, other.Position!);
                return mapCullingNearPointValue > dist && dist != 0 && !okayList.Contains(point.Id!);
            });

            if (!hasClosePoint) return point;

            okayList.Add(point.Id!);
            return point with
            {
                DelayToCanSpawnSec = 9999999,
                CorePointId = 99999,
                Categories = [],
                Sides = []
            };
        });

        return filteredParams.Where(point => point.Categories!.Any()).ToArray();
    }

    public static SpawnPointParam[] AddCustomPmcSpawnPoints(SpawnPointParam[] spawnPointParams, string mapName)
    {
        if (GlobalValues.PmcSpawns[mapName].Length == 0)
        {
            return spawnPointParams;
        }

        var playerSpawns = GlobalValues.PmcSpawns[mapName].Select((coords, index) => 
            new SpawnPointParam {
                Id = Guid.NewGuid().ToString(),
                BotZoneName = GetClosestZone(spawnPointParams, coords),
                Categories = ["Coop", new Random().NextDouble() < 0.5 ? "Group" : "Opposite"],
                Sides = ["Pmc"],
                CorePointId = 0,
                ColliderParams = new ColliderParams
                {
                    Parent = "SpawnSphereParams",
                    Properties = new ColliderProperties
                    {
                        Center = new XYZ
                        {
                            X = 0,
                            Y = 0,
                            Z = 0,
                        },
                        Radius = 20
                    }
                },
                DelayToCanSpawnSec = 4,
                Infiltration = "",
                Position = coords,
                Rotation = Random360()
            }
        );
        
        return spawnPointParams.Concat(playerSpawns).ToArray();
    }
    
    public static SpawnPointParam[] AddCustomBotSpawnPoints(SpawnPointParam[] spawnPointParams, string mapName)
    {
        if (GlobalValues.ScavSpawns[mapName].Length == 0)
        {
            return spawnPointParams;
        }

        var scavSpawns = GlobalValues.ScavSpawns[mapName].Select(coords => 
            new SpawnPointParam {
                Id = Guid.NewGuid().ToString(),
                BotZoneName = GetClosestZone(spawnPointParams, coords),
                Categories = ["Bot"],
                Sides = ["Savage"],
                CorePointId = 1,
                ColliderParams = new ColliderParams
                {
                    Parent = "SpawnSphereParams",
                    Properties = new ColliderProperties
                    {
                        Center = new XYZ
                        {
                            X = 0,
                            Y = 0,
                            Z = 0,
                        },
                        Radius = 20
                    }
                },
                DelayToCanSpawnSec = 4,
                Infiltration = "",
                Position = coords,
                Rotation = Random360()
            }
        );
        
        return spawnPointParams.Concat(scavSpawns).ToArray();
    }

    public static SpawnPointParam[] AddCustomSniperSpawnPoints(SpawnPointParam[] spawnPointParams, string mapName)
    {
        if (GlobalValues.SniperSpawns[mapName].Length == 0)
        {
            return spawnPointParams;
        }

        var sniperSpawns = GlobalValues.SniperSpawns[mapName].Select((coords, index) =>
            new SpawnPointParam {
                Id = Guid.NewGuid().ToString(),
                BotZoneName = GetClosestZone(spawnPointParams, coords) + $"custom_snipe_{index}",
                Categories = ["Bot"],
                Sides = ["Savage"],
                CorePointId = 1,
                ColliderParams = new ColliderParams
                {
                    Parent = "SpawnSphereParams",
                    Properties = new ColliderProperties
                    {
                        Center = new XYZ
                        {
                            X = 0,
                            Y = 0,
                            Z = 0,
                        },
                        Radius = 20
                    }
                },
                DelayToCanSpawnSec = 4,
                Infiltration = "",
                Position = coords,
                Rotation = Random360()
            }
        );
        
        return spawnPointParams.Concat(sniperSpawns).ToArray();
    }

    public static SpawnPointParam[] BuildCustomPlayerSpawnPoints(string mapName, SpawnPointParam[] refSpawns)
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
                    Id = Guid.NewGuid().ToString(),
                    BotZoneName = "",
                    Sides = ["Pmc"]
                };
            })
            .ToArray();

        if (GlobalValues.PlayerSpawns[mapName].Length == 0)
        {
            return playerOnlySpawns;
        }

        var playerSpawns = GlobalValues.PlayerSpawns[mapName].Select(point =>
            new SpawnPointParam
            {
                Id = Guid.NewGuid().ToString(),
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
                Infiltration = GetClosestInfiltration((double)point.X!, (double)point.Y!, (double)point.Z!),
                Position = point,
                Rotation = Random360(),
                Sides = ["Pmc"]
            }
        );

        return playerOnlySpawns.Concat(playerSpawns).ToArray();

        string GetClosestInfiltration(double x, double y, double z)
        {
            var selectedInfiltration = "";
            var closest = double.PositiveInfinity;

            foreach (var spawn in playerOnlySpawns)
            {
                if (string.IsNullOrEmpty(spawn.Infiltration) || spawn.Position == null)
                    continue;

                var pos = spawn.Position;
                var dist = GetDistance(new XYZ
                {
                    X = x,
                    Y = y,
                    Z = z,
                }, pos);

                if (!(dist < closest)) continue;

                selectedInfiltration = spawn.Infiltration;
                closest = dist;
            }

            return selectedInfiltration;
        }
    }

    private static string GetClosestZone(SpawnPointParam[] spawnPointParams, XYZ coords)
    {
        var validPoints = spawnPointParams
            .Where(p => !string.IsNullOrEmpty(p.BotZoneName))
            .ToArray();

        if (validPoints.Length == 0)
            return "";

        var sorted = GetSortedSpawnPointList(validPoints, coords);
        return sorted.First().BotZoneName ?? "";
    } 
    
    public static XYZ[] RemoveClosestSpawnsFromCustomBots(Dictionary<string, XYZ[]> customBots,
        List<SpawnPointParam> spawnPointParams, string mapName)
    {
        if (customBots[mapName].Length == 0)
        {
            return [];
        }
        
        var coords = customBots[mapName];
        var mapConfig = GlobalValues.MapsConfig[mapName];

        var mapCullingNearPointValue = (mapConfig.MapCullingNearPointValuePlayer +
                                        mapConfig.MapCullingNearPointValuePmc +
                                        mapConfig.MapCullingNearPointValueScav) / 3;

        var okayList = new HashSet<string>();
        var filteredCoords = coords.ToList().Where(coord =>
        {
            var result = !coords.Any(other =>
            {
                var dist = GetDistance(coord, other);
                return mapCullingNearPointValue * 1.3f > dist &&
                       dist != 0 && !okayList.Contains($"{other.X}{other.Y}{other.Z}");
            });

            if (!result)
            {
                okayList.Add($"{coord.X}{coord.Y}{coord.Z}");
            }

            return result;
        }).ToArray();

        return filteredCoords;
    }
}