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

    public static string GetRandomOrSelectedPreset()
    {
        if (GlobalValues.ForcedPreset.Equals("custom"))
        {
            return "custom"; 
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

        return selectedPreset; 
    }

    public static BaseConfig GetPresetData(string presetName)
    {
        return GlobalValues.Presets[presetName];
    } 
}