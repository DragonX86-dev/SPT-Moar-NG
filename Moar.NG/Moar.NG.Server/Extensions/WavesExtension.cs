using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using Moar.NG.Server.Globals;
using static Moar.NG.Server.Stuff.Utils;

namespace Moar.NG.Server.Extensions;

[Injectable(TypePriority = OnLoadOrder.PostSptModLoader)]
public class WavesExtension(
    BotConfig botConfig, 
    PmcConfig pmcConfig, 
    LocationConfig locationConfig,
    DatabaseServer databaseServer,
    ISptLogger<WavesExtension> logger): IOnLoad
{
    public Task OnLoad()
    {
        if (!GlobalValues.BaseConfig.EnableBotSpawning) return Task.CompletedTask;
        
        logger.Info("[MOAR-NG]: Starting up, may the bots ever be in your favour!");
        BuildWaves();

        return Task.CompletedTask;
    }
    
    public void BuildWaves()
    {
        // Move to ALP
        locationConfig.FitLootIntoContainerAttempts = 1; 
        
        locationConfig.RogueLighthouseSpawnTimeSettings.WaitTimeSeconds = 60;
        locationConfig.EnableBotTypeLimits = false;
        locationConfig.AddCustomBotWavesToMaps = false;
        
        locationConfig.CustomWaves = new CustomWaves();
        
        pmcConfig.RemoveExistingPmcWaves = true;

        var locations = databaseServer.GetTables().Locations;
        var bots  = databaseServer.GetTables().Bots;
        
        var config = DeepCopy(GlobalValues.BaseConfig);
        
        GlobalValues.LoadOrResetLocationsBase(databaseServer);
        
        pmcConfig.RemoveExistingPmcWaves = true;
        pmcConfig.CustomPmcWaves.Keys.ToList().ForEach(key =>
            {
                pmcConfig.CustomPmcWaves[key] = [];
            }
        );
        
        if (config.StartingPmcs && (!config.RandomSpawns || config.SpawnSmoothing)) {
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

    

    
}