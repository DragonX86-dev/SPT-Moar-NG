using System.Text.Json;
using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;

namespace Moar.NG.Server.Stuff.Common;

public class MongoIdConverter : JsonConverter<MongoId>
{
    public override MongoId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new MongoId(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, MongoId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
    
    public override MongoId ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Read(ref reader, typeToConvert, options);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, MongoId value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.ToString()); 
    }
}