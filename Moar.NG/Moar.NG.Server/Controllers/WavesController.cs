using System.Text.Json;
using Moar.NG.Server.Globals;
using Moar.NG.Server.Models;
using Moar.NG.Server.Stuff;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Bots;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using static Moar.NG.Server.Stuff.Utils.CommonUtils;

namespace Moar.NG.Server.Controllers;

[Injectable(TypePriority = OnLoadOrder.PostSptModLoader)]
public class WavesController(
    ConfigServer configServer,
    DatabaseServer databaseServer,
    ISptLogger<WavesController> logger) : IOnLoad
{
    public Task OnLoad()
    {
        if (!GlobalValues.MoarConfig.EnableBotSpawning) return Task.CompletedTask;
        
        logger.LogWithColor("[MOAR]: Starting up, may the bots ever be in your favour!", LogTextColor.Cyan);
        BuildWaves();
        
        return Task.CompletedTask;
    }
    
    public void BuildWaves()
    {
        var pmcConfig = configServer.GetConfig<PmcConfig>();
        var botConfig = configServer.GetConfig<BotConfig>();
        var locationConfig = configServer.GetConfig<LocationConfig>();
        
        locationConfig.RogueLighthouseSpawnTimeSettings.WaitTimeSeconds = 60;
        locationConfig.EnableBotTypeLimits = false;
        // Move to ALP
        locationConfig.FitLootIntoContainerAttempts = 1; 
        locationConfig.AddCustomBotWavesToMaps = false;
        locationConfig.CustomWaves = new CustomWaves();
        
        pmcConfig.RemoveExistingPmcWaves = true;

        var bots  = databaseServer.GetTables().Bots;
        
        var config = DeepCopy(GlobalValues.MoarConfig);
        var preset = GetRandomOrSelectedPreset();

        preset.Keys.ToList().ForEach(key =>
        {
            var field = config.GetType().GetProperty(key.Capitalize());
            if (field != null)
            {
                field.SetValue(config, preset[key].ValueKind switch
                {
                    JsonValueKind.Number => ((JsonElement) preset[key]).GetNumberValue(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => preset[key].ToString() 
                });
            }
        });
        
        var locationsDict = databaseServer.GetTables().Locations.GetDictionary();
        var locationList = GlobalValues.MapList
            .Select(mapName => locationsDict[mapName] with { Base = DeepCopy(locationsDict[mapName].Base) })
            .ToArray();
        
        pmcConfig.RemoveExistingPmcWaves = true;
        pmcConfig.CustomPmcWaves.Keys.ToList().ForEach(key =>
            {
                pmcConfig.CustomPmcWaves[key] = [];
            }
        );
        
        if (config.StartingPmcs  && (config.RandomSpawns || config.SpawnSmoothing)) {
            logger.Warning("[MOAR] Starting pmcs turned on, turning off cascade system and smoothing.\n");
            config.SpawnSmoothing = false;
            config.RandomSpawns = true;
        }
        
        if (GlobalValues.AdvancedConfig.MarksmanDifficultyChanges) {
            MakeMarksmanChanges(bots);
        }
            
    }

    private static void MakeMarksmanChanges(Bots bots)
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

    public void UpdateSpawnLocations(Location[] locations, MoarConfig config)
    {
        throw new NotImplementedException();
    }
}