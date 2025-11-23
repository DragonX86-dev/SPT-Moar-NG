using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using Moar.NG.Server.Globals;
using Moar.NG.Server.Stuff;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Logging;
using static Moar.NG.Server.Stuff.Utils;

namespace Moar.NG.Server.Extensions;

[Injectable(TypePriority = OnLoadOrder.PostSptModLoader)]
public class WavesExtension(
    ConfigServer configServer,
    DatabaseServer databaseServer,
    ISptLogger<WavesExtension> logger) : IOnLoad
{
    public Task OnLoad()
    {
        if (GlobalValues.BaseConfig.EnableBotSpawning != true) return Task.CompletedTask;
        
        logger.LogWithColor("[MOAR]: Starting up, may the bots ever be in your favour!", LogTextColor.Cyan);
        BuildWaves();

        var preset = GetRandomOrSelectedPreset();
        logger.LogWithColor($"[MOAR]: Выбран пресет {preset}", LogTextColor.Cyan);

        return Task.CompletedTask;
    }
    
    public void BuildWaves()
    {
        var pmcConfig = configServer.GetConfig<PmcConfig>();
        var botConfig = configServer.GetConfig<BotConfig>();
        var locationConfig = configServer.GetConfig<LocationConfig>();
        
        // Move to ALP
        locationConfig.FitLootIntoContainerAttempts = 1; 
        
        locationConfig.RogueLighthouseSpawnTimeSettings.WaitTimeSeconds = 60;
        locationConfig.EnableBotTypeLimits = false;
        locationConfig.AddCustomBotWavesToMaps = false;
        
        locationConfig.CustomWaves = new CustomWaves();
        
        pmcConfig.RemoveExistingPmcWaves = true;

        var bots  = databaseServer.GetTables().Bots;
        
        var config = DeepCopy(GlobalValues.BaseConfig);
        
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
        
        if (config.StartingPmcs == true && (config.RandomSpawns != true || config.SpawnSmoothing == true)) {
            logger.Warning("[MOAR-NG] Starting pmcs turned on, turning off cascade system and smoothing.\n");
            config.SpawnSmoothing = false;
            config.RandomSpawns = true;
        }
        
        /*
           if (advancedConfig.MarksmanDifficultyChanges) {
            marksmanChanges(bots);
          }
         */

    }

    public void UpdateSpawnLocations(Location[] locations, BaseConfig config)
    {
        throw new NotImplementedException();
    }
}