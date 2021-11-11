using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client.Models
#else
namespace MyAuth.OAuthPoint.Models
#endif
{
    /// <summary>
    /// Represent claim value
    /// </summary>
    [JsonConverter(typeof(JsonConverter))]
    public class ClaimValue
    {
        /// <summary>
        /// String value
        /// </summary>
        public string String { get; }

        /// <summary>
        /// Long value
        /// </summary>
        public long? Long { get; }

        /// <summary>
        /// Double value
        /// </summary>
        public double? Double { get; }

        /// <summary>
        /// Json object value
        /// </summary>
        public JObject Object { get; }

        /// <summary>
        /// Json array object value
        /// </summary>
        public JArray Array { get; }

        /// <summary>
        /// Bool object value
        /// </summary>
        public bool? Bool { get; }

        /// <summary>
        /// DateTime object value
        /// </summary>
        public DateTime? DateTime { get; }

        public bool IsNull { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimValue"/>
        /// </summary>
        public ClaimValue(DateTime value)
        {
            DateTime = value;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimValue"/>
        /// </summary>
        public ClaimValue(bool value)
        {
            Bool = value;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimValue"/>
        /// </summary>
        public ClaimValue(double value)
        {
            Double = value;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimValue"/>
        /// </summary>
        public ClaimValue(long value)
        {
            Long = value;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimValue"/>
        /// </summary>
        public ClaimValue(string value)
        {
            String = value;
            IsNull = value == null;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimValue"/>
        /// </summary>
        public ClaimValue(object value)
        {
            if (value != null)
            {
                if (value is string strVal)
                {
                    String = strVal;
                }
                else if (value is JObject jObj)
                {
                    Object = jObj;
                }
                else if (value is JArray jArr)
                {
                    Array = jArr;
                }
                else
                {
                    Object = JObject.FromObject(value);
                }
            }
            else
            {
                IsNull = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimValue"/>
        /// </summary>
        public ClaimValue(JObject value)
        {
            if (value != null)
            {
                Object = JObject.FromObject(value);
            }
            else
            {
                IsNull = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimValue"/>
        /// </summary>
        public ClaimValue(JArray array)
        {
            if (array != null)
            {
                Array = array;
            }
            else
            {
                IsNull = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimValue"/>
        /// </summary>
        public ClaimValue(Array array)
        {
            if (array != null)
            {
                Array = JArray.FromObject(array);
            }
            else
            {
                IsNull = true;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (IsNull)
                return null;

            if (String != null)
                return String;

            if (Object != null)
                return Object.ToString();

            if (Array != null)
                return Array.ToString();

            if (Long.HasValue) return Long.ToString();
            if (Double.HasValue) return Double.ToString();
            if (Bool.HasValue) return Bool.ToString();
            if (DateTime.HasValue) return DateTime.ToString();

            return base.ToString();
        }

        public static ClaimValue Parse(string stringValue)
        {
            if (stringValue == null) return null;

            if (stringValue.StartsWith("{"))
            {
                try
                {
                    return new ClaimValue(JObject.Parse(stringValue));
                }
                catch (JsonException)
                {
                    return new ClaimValue(stringValue);
                }
            }

            if (stringValue.StartsWith("["))
            {
                return new ClaimValue(JArray.Parse(stringValue));
            }

            return new ClaimValue(stringValue);
        }

        protected bool Equals(ClaimValue other)
        {
            return String == other.String && Long == other.Long && Nullable.Equals(Double, other.Double) && JToken.DeepEquals(Array, other.Array) && JToken.DeepEquals(Object, other.Object) && Bool == other.Bool && Nullable.Equals(DateTime, other.DateTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClaimValue)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(String, Long, Double, Object, Array, Bool, DateTime);
        }

        class JsonConverter : JsonConverter<ClaimValue>
        {
            public override void WriteJson(JsonWriter writer, ClaimValue value, JsonSerializer serializer)
            {
                if (value == null || value.IsNull)
                {
                    writer.WriteNull();
                }
                else
                {
                    if (value.String != null)
                        writer.WriteValue(value.String);
                    else if (value.Long.HasValue)
                        writer.WriteValue(value.Long.Value);
                    else if (value.Double.HasValue)
                        writer.WriteValue(value.Double.Value);
                    else if (value.Bool.HasValue)
                        writer.WriteValue(value.Bool.Value);
                    else if (value.DateTime.HasValue)
                        writer.WriteValue(value.DateTime.Value);
                    else if (value.Array != null)
                    {
                        writer.WriteStartArray();
                        foreach (var arrItem in value.Array)
                        {
                            arrItem.WriteTo(writer);
                        }
                        writer.WriteEndArray();
                    }
                    else 
                        value.Object.WriteTo(writer);
                }
            }

            public override ClaimValue ReadJson(JsonReader reader, Type objectType, ClaimValue existingValue, bool hasExistingValue,
                JsonSerializer serializer)
            {
                serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        return new ClaimValue(JObject.ReadFrom(reader));

                    case JsonToken.StartArray:
                        return new ClaimValue(JArray.ReadFrom(reader));

                    case JsonToken.Integer:
                    case JsonToken.Bytes:
                        return new ClaimValue((long)new JValue(reader.Value));

                    case JsonToken.Float:
                        return new ClaimValue((double)new JValue(reader.Value));

                    case JsonToken.String:
                        return new ClaimValue((string)new JValue(reader.Value));

                    case JsonToken.Boolean:
                        return new ClaimValue((bool)new JValue(reader.Value));

                    case JsonToken.Null:
                        return null;

                    case JsonToken.Date:
                        return new ClaimValue((DateTime)new JValue(reader.Value));

                    default:
                        throw new JsonSerializationException($"Unsupported token {reader.TokenType}");
                }
            }
        }
    }
}
