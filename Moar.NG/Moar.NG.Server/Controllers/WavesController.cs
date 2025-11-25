using System.Text.Json;
using Moar.NG.Server.Globals;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils.Cloners;
using static Moar.NG.Server.Utils.CommonUtils;
using static Moar.NG.Server.Utils.WaveUtils;

namespace Moar.NG.Server.Controllers;

[Injectable(TypePriority = OnLoadOrder.PostSptModLoader)]
public class WavesController(
    ICloner cloner,
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
        
        var config = cloner.Clone(GlobalValues.MoarConfig)!;
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
        var locationList = GlobalConstants.MapList
            .Select(mapName => cloner.Clone(locationsDict[mapName])!)
            .ToList();
        
        pmcConfig.RemoveExistingPmcWaves = true;
        pmcConfig.CustomPmcWaves.Keys.ToList().ForEach(key =>
            {
                pmcConfig.CustomPmcWaves[key] = [];
            }
        );
        
        if (config.StartingPmcs  && (!config.RandomSpawns || config.SpawnSmoothing)) {
            logger.Warning("[MOAR] Starting pmcs turned on, turning off cascade system and smoothing.\n");
            config.SpawnSmoothing = false;
            config.RandomSpawns = true;
        }
        
        var bots  = databaseServer.GetTables().Bots;
        
        if (GlobalValues.AdvancedConfig.MarksmanDifficultyChanges) {
            MakeMarksmanChanges(bots);
        }
        
        UpdateSpawnLocations(locationList, config);

        BuildBossWaves(locationList, config);
        
        //Zombies
        if (GlobalValues.MoarConfig.ZombiesEnabled) {
            BuildZombieWaves(locationList, config);
        }

        BuildPmcWaves(locationList, config);
        


    }
}