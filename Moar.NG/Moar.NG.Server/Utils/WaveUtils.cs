using Moar.NG.Server.Globals;
using Moar.NG.Server.Models;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Spt.Bots;
using static Moar.NG.Server.Utils.SpawnUtils;

namespace Moar.NG.Server.Utils;

public static class WaveUtils
{
    public static void MakeMarksmanChanges(Bots bots)
    {
        foreach (var difficulty in bots.Types["marksman"]!.BotDifficulty.Keys
                     .Select(difficultyKey => bots.Types["marksman"]!.BotDifficulty[difficultyKey]))
        {
            difficulty.Core = difficulty.Core with
            {
                VisibleAngle = 300,
                VisibleDistance = 245,
                ScatteringPerMeter = 0.1f,
                HearingSense = 2.85f
            };
            
            difficulty.Mind = difficulty.Mind with
            {
                BulletFeelDist = 360,
                ChanceFuckYouOnContact100 = 10
            };
            
            difficulty.Hearing = difficulty.Hearing with
            {
                ChanceToHearSimpleSound01 = 0.7f,
                DispersionCoef = 3.6f,
                CloseDist = 10,
                FarDist = 30
            };
        }
    }
    
    public static void UpdateSpawnLocations(List<Location> locations, MoarConfig config)
    {
        foreach (var (location, mapName) in locations.Zip(GlobalConstants.MapList))
        {
            var playerSpawns = GlobalSpawnData.PlayerMapSpawns[mapName];
            var currentPlayerSpawn = playerSpawns.GetRandomSpawnPoint();
            GlobalSpawnData.PlayerSpawn = currentPlayerSpawn;
            
            var possibleSpawnList = new List<SpawnPointParam>();
            var sortedPlayerSpawns = GetSortedSpawnPointList(playerSpawns, currentPlayerSpawn.Position!);
            
            foreach (var spawnPoint in sortedPlayerSpawns)
            {
                if (possibleSpawnList.Count >= GlobalValues.AdvancedConfig.SpawnPointAreaTarget) continue;
                
                spawnPoint.ColliderParams!.Properties!.Radius = 1;
                possibleSpawnList.Add(spawnPoint);
            }
 
            location.Base.SpawnPointParams = possibleSpawnList
                .Concat(GlobalSpawnData.PmcMapSpawns[mapName])
                .Concat(GlobalSpawnData.ScavMapSpawns[mapName])
                .Concat(GlobalSpawnData.SniperMapSpawns[mapName])
                .Concat(GlobalSpawnData.BossMapSpawns[mapName])
                .ToArray();
        }
    }

    public static void BuildBossWaves(List<Location> locations, MoarConfig config)
    {
        throw new NotImplementedException();
    }

    public static void BuildPmcWaves(List<Location> locations, MoarConfig config)
    {
        foreach (var (location, mapName) in locations.Zip(GlobalConstants.MapList))
        {
            location.Base.BotLocationModifier.AdditionalHostilitySettings = GlobalConstants.DefaultHostility;

            var pmcHotZones = GlobalValues.MapsConfig[mapName].PmcHotZones ?? [];
            var pmcWaveCount = GlobalValues.MapsConfig[mapName].PmcWaveCount;
            var initialSpawnDelay = GlobalValues.MapsConfig[mapName].InitialSpawnDelay;
            
            var playerPosition = GlobalSpawnData.PlayerSpawn.Position ?? new XYZ
            {
                X = 0,
                Y = 0,
                Z = 0
            };

            var pmcZones = GetSortedSpawnPointList(
                 GlobalSpawnData.PlayerMapSpawns[mapName], playerPosition, 0.05f)
                .Select(point => point.BotZoneName);

        }
    }

    // public static void SetEscapeTimeOverrides(List<Location> locations, MoarConfig config)
    // {
    //     
    //     {
    //         
    //         var EscapeTimeLimitOverride = GlobalValues.MapsConfig[mapName].
    //         
    //         
    //     }
    // }
}