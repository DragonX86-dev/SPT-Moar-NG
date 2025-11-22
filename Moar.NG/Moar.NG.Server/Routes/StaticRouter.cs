using Moar.NG.Server.Extensions;
using Moar.NG.Server.Globals;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Utils;

namespace Moar.NG.Server.Routes;

[Injectable(TypePriority = OnLoadOrder.PostSptModLoader + 1)]
public class MoarStaticRouter(WavesExtension wavesExtension, JsonUtil jsonUtil) 
    : StaticRouter(jsonUtil, GetCustomRoutes(wavesExtension))
{
    // _wavesExtension = wavesExtension;

    private static List<RouteAction> GetCustomRoutes(WavesExtension wavesExtension)
    {
        return
        [
            // Make buildwaves run on game end
            new RouteAction(
                "/client/match/local/end", 
                (url, info, sessionId, output) =>
                {
                    wavesExtension.BuildWaves();
                    return ValueTask.FromResult<object>(output!);
                }
            )
            
        ];
    }
}