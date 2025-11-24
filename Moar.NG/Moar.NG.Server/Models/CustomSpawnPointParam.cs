using Moar.NG.Server.Models.Enums;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace Moar.NG.Server.Models;

public record MoarSpawnPoint
{
    public required SpawnPointParam SpawnPointParam { get; init; }
    public required SpawnType Type { get; init; }
}