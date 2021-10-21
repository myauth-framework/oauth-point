using System;
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
        /// Int value
        /// </summary>
        public int? Int { get; }

        /// <summary>
        /// Double value
        /// </summary>
        public double? Double { get; }

        /// <summary>
        /// Json object value
        /// </summary>
        public JObject Object { get; }

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
        public ClaimValue(int value)
        {
            Int = value;
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
                    String = strVal;
                else
                {
                    Object = JObject.FromObject(value);
                }
            }
            else
            {
                IsNull = false;
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

        public override string ToString()
        {
            if (IsNull)
                return null;

            if (String != null)
                return String;

            if (Object != null)
                return Object.ToString();

            if (Int.HasValue) return Int.ToString();
            if (Double.HasValue) return Int.ToString();
            if (Bool.HasValue) return Int.ToString();
            if (DateTime.HasValue) return Int.ToString();

            return base.ToString();
        }

        public static ClaimValue Parse(string stringValue)
        {
            if (stringValue == null) return null;

            if (stringValue.StartsWith("{") || stringValue.StartsWith("["))
            {
                try
                {
                    return new ClaimValue(JObject.Parse(stringValue));
                }
                catch (JsonException )
                {
                    return new ClaimValue(stringValue);
                }
            }

            return new ClaimValue(stringValue);
        }

        protected bool Equals(ClaimValue other)
        {
            return String == other.String && Int == other.Int && Nullable.Equals(Double, other.Double) && JToken.DeepEquals(Object, other.Object) && Bool == other.Bool && Nullable.Equals(DateTime, other.DateTime);
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
            return HashCode.Combine(String, Int, Double, Object, Bool, DateTime);
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
                    else if (value.Int.HasValue)
                        writer.WriteValue(value.Int.Value);
                    else if (value.Double.HasValue)
                        writer.WriteValue(value.Double.Value);
                    else if (value.Bool.HasValue)
                        writer.WriteValue(value.Bool.Value);
                    else if (value.DateTime.HasValue)
                        writer.WriteValue(value.DateTime.Value);
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

                    case JsonToken.Integer:
                    case JsonToken.Bytes:
                        return new ClaimValue((int)new JValue(reader.Value));

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
