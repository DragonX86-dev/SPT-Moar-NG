using System.Text.Json;

namespace Moar.NG.Server.Stuff.Common;

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
    
    public static string Capitalize(this string input)
    {
        return string.IsNullOrWhiteSpace(input) ? input : string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));
    }
}