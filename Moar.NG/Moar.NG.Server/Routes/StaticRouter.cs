using Moar.NG.Server.Controllers;
using Moar.NG.Server.Globals;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Utils;

namespace Moar.NG.Server.Routes;

[Injectable(TypePriority = OnLoadOrder.PostSptModLoader + 1)]
public class MoarStaticRouter(WavesController wavesController, JsonUtil jsonUtil) 
    : StaticRouter(jsonUtil, GetCustomRoutes(wavesController))
{
    // _wavesExtension = wavesExtension;

    private static List<RouteAction> GetCustomRoutes(WavesController wavesController)
    {
        return
        [
            // Make buildwaves run on game end
            new RouteAction(
                "/client/match/local/end", 
                (url, info, sessionId, output) =>
                {
                    wavesController.BuildWaves();
                    return ValueTask.FromResult<object>(output!);
                }
            )
            
        ];
    }
}