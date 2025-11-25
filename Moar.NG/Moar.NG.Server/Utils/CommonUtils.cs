using System.Text.Json;
using Moar.NG.Server.Globals;

namespace Moar.NG.Server.Utils;

public static class CommonUtils
{
    public static Dictionary<string, dynamic> GetRandomOrSelectedPreset()
    {
        if (GlobalConstants.ForcedPreset.Equals("custom"))
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

    public static IList<T> LooselyShuffle<T>(this IList<T> list, int shuffleStep = 3)
    {
        if (list.Count == 0)
            return list;

        var n = list.Count;
        var halfN = n / 2;
        var random = new Random();

        for (var i = shuffleStep - 1; i < halfN; i += shuffleStep)
        {
            var randomIndex = halfN + random.Next(n - halfN);
            
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }

        return list;
    }

    
}