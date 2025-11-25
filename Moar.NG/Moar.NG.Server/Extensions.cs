using System.Text.Json;
using SPTarkov.Server.Core.Models.Eft.Common;

namespace Moar.NG.Server;

public static class Extensions
{
    public static object GetNumberValue(this JsonElement element)
    {
        if (element.TryGetInt32(out var intValue))
            return intValue;
        if (element.TryGetInt64(out var longValue))
            return longValue;
        if (element.TryGetDouble(out var doubleValue))
            return doubleValue;
        if (element.TryGetDecimal(out var decimalValue))
            return decimalValue;
    
        return element.GetDouble();
    }

    public static string CombinePaths(this string[] folders)
    {
        return folders.Aggregate(string.Empty, Path.Combine);
    } 

    public static SpawnPointParam GetRandomSpawnPoint(this SpawnPointParam[] spawnPoints)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            throw new ArgumentException("Array cannot be null or empty");
    
        var rand = new Random();
        var index = rand.Next(0, spawnPoints.Length);
        return spawnPoints[index];
    }
    
    public static string Capitalize(this string input)
    {
        return string.IsNullOrWhiteSpace(input) ? input : string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));
    }
}