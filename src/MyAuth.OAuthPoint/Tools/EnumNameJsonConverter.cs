using System;
using Newtonsoft.Json;

#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client.Tools
#else
namespace MyAuth.OAuthPoint.Tools
#endif
{
    class EnumNameJsonConverter : JsonConverter<Enum>
    {
        public override void WriteJson(JsonWriter writer, Enum value, JsonSerializer serializer)
        {
            writer.WriteValue(EnumNameTools.GetName(value));
        }

        public override Enum ReadJson(JsonReader reader, Type objectType, Enum existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return EnumNameTools.GetValue(objectType, reader.Value?.ToString() ?? throw new InvalidOperationException("Enum name is null"));
        }
    }
}