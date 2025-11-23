using System.Numerics;
using System.Text.Json;
using Moar.NG.Server.Globals;
using Moar.NG.Server.Models;

namespace Moar.NG.Server.Stuff;

public static class Utils
{
    public static T DeepCopy<T>(T obj)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new MongoIdConverter() }
        };
        
        var json = JsonSerializer.Serialize(obj, options);
        return JsonSerializer.Deserialize<T>(json, options)!;
    }

    public static BaseConfig GetRandomOrSelectedPreset()
    {
        if (GlobalValues.ForcedPreset.Equals("custom"))
        {
            return new BaseConfig(); 
        }

        var allPresets = new List<string>();
        var itemKeys = GlobalValues.PresetWeightings.Keys;

        foreach (var itemKey in itemKeys)
        {
            Enumerable.Range(0, GlobalValues.PresetWeightings[itemKey]).ToList().ForEach(_ => allPresets.Add(itemKey));
        }
        
        var random = new Random();
        var selectedPreset = allPresets[random.Next(allPresets.Count)];
        GlobalValues.CurrentPreset = selectedPreset;

        return GlobalValues.Presets[selectedPreset]; 
    }
    
    public static List<T> Shuffle<T>(List<T> list)
    {
        var random = new Random();
        return list.OrderBy(x => random.Next()).ToList();
    }
    
    public static double GetDistance(double x, double y, double z, double mX, double mY, double mZ)
    {
        var point1 = new Vector3((float)x, (float)y, (float)z);
        var point2 = new Vector3((float)mX, (float)mY, (float)mZ);
    
        return Vector3.Distance(point1, point2);
    }
    
    public static double Random360()
    {
        var random = new Random();
        return random.NextDouble() * 360;
    }
}