using Moar.NG.Server.Extensions;
using Moar.NG.Server.Globals;
using Moar.NG.Server.Routes.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Utils;

namespace Moar.NG.Server.Routes;

[Injectable]
public class StaticRouter : SPTarkov.Server.Core.DI.StaticRouter
{
    private static WavesExtension _wavesExtension = null!;

    public StaticRouter(JsonUtil jsonUtil, WavesExtension wavesExtension): base(jsonUtil, GetCustomRoutes())
    {
        _wavesExtension = wavesExtension;
    }
    
    private static List<RouteAction> GetCustomRoutes()
    {
        if (!GlobalValues.BaseConfig.EnableBotSpawning)
        {
            return [];
        }
        
        return
        [
            // Make buildwaves run on game end
            new RouteAction(
                "/client/match/local/end", 
                (url, info, sessionId, output) =>
                {
                    _wavesExtension.BuildWaves();
                    return ValueTask.FromResult<object>(output!);
                }
            )
            
        ];
    }
}