using System.Text.Json;
using Moar.NG.Server.Globals;

namespace Moar.NG.Server.Stuff.Common;

public static class CommonUtils
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

    public static Dictionary<string, dynamic> GetRandomOrSelectedPreset()
    {
        if (GlobalValues.ForcedPreset.Equals("custom"))
        {
            return new Dictionary<string, dynamic>(); 
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
    
    public static IEnumerable<T> Shuffle<T>(IEnumerable<T> list)
    {
        var random = new Random();
        return list.OrderBy(x => random.Next());
    }
}