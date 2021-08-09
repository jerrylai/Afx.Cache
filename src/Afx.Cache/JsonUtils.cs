using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETCOREAPP || NETSTANDARD
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
#else
using Newtonsoft.Json;
#endif

namespace Afx.Cache.Json
{
    /// <summary>
    /// json 序列化相关 Utils
    /// </summary>
    internal static class JsonUtils
    {
#if NETCOREAPP || NETSTANDARD
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            IgnoreNullValues = true,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = null,
            DictionaryKeyPolicy = null
        };

        public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        static JsonUtils()
        {
          //  options.Converters.Add(new DateTimeJsonConverter(DATE_FORMAT));
            options.Converters.Add(new StringJsonConverter());
            options.Converters.Add(new BoolJsonConverter());
            options.Converters.Add(new IntJsonConverter());
            options.Converters.Add(new LongJsonConverter());
            options.Converters.Add(new FloatJsonConverter());
            options.Converters.Add(new DoubleJsonConverter());
            options.Converters.Add(new DecimalJsonConverter());
        }
#else
        private static readonly JsonSerializerSettings options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore
        };
#endif

        /// <summary>
        /// 序列化json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Serialize<T>(T value)
        {
            if (value == null) return null;
#if NETCOREAPP || NETSTANDARD
            return JsonSerializer.Serialize(value, options);
#else
            return JsonConvert.SerializeObject(value, options);
#endif
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            T m = default(T);
            if (string.IsNullOrEmpty(json)) return m;
#if NETCOREAPP || NETSTANDARD
            m = JsonSerializer.Deserialize<T>(json, options);
#else
            m =  JsonConvert.DeserializeObject<T>(json, options);
#endif
            return m;
        }
    }

#if NETCOREAPP || NETSTANDARD
    /// <summary>
    /// 日期格式
    /// </summary>
    internal class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// 格式
        /// </summary>
        public string Format { get; }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="format"></param>
        public DateTimeJsonConverter(string format)
        {
            this.Format = format;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var t = DateTime.Parse(reader.GetString());
            if (t.Kind == DateTimeKind.Unspecified)
            {
                t = new DateTime(t.Ticks, DateTimeKind.Local);
            }

            return t;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(this.Format));
        }
    }
    /// <summary>
    /// 字符串
    /// </summary>
    internal class StringJsonConverter : JsonConverter<string>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string v = null;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    v = reader.GetString();
                    break;
                case JsonTokenType.Number:
                    if (reader.TryGetInt32(out var num))
                        v = num.ToString();
                    else if (reader.TryGetDecimal(out var dm))
                        v = dm.ToString();
                    break;
                case JsonTokenType.True:
                case JsonTokenType.False:
                    v = reader.GetBoolean().ToString().ToLower();
                    break;
                case JsonTokenType.Null:
                    break;
                default:
                    throw new InvalidOperationException($"{reader.TokenType} not convert to {typeToConvert.FullName}.");
            }

            return v;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class BoolJsonConverter : JsonConverter<bool>
    {

        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            bool v = false;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var s = reader.GetString()?.ToLower();
                    if (int.TryParse(s, out var k)) v = k != 0;
                    else v = (s == "true" || s == "on");
                    break;
                case JsonTokenType.Number:
                    if (reader.TryGetInt32(out var j)) v = j != 0;
                    break;
                case JsonTokenType.True:
                    v = true;
                    break;
                case JsonTokenType.False:
                    v = false;
                    break;
                case JsonTokenType.Null:
                    break;
                default:
                    throw new InvalidOperationException($"{reader.TokenType} not convert to {typeToConvert.FullName}.");
            }

            return v;
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class IntJsonConverter : JsonConverter<int>
    {

        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int v = 0;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    int.TryParse(reader.GetString(), out v);
                    break;
                case JsonTokenType.Number:
                    reader.TryGetInt32(out v);
                    break;
                case JsonTokenType.True:
                    v = 1;
                    break;
                case JsonTokenType.False:
                    v = 0;
                    break;
                case JsonTokenType.Null:
                    break;
                default:
                    throw new InvalidOperationException($"{reader.TokenType} not convert to {typeToConvert.FullName}.");
            }

            return v;
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class LongJsonConverter : JsonConverter<long>
    {

        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            long v = 0;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    long.TryParse(reader.GetString(), out v);
                    break;
                case JsonTokenType.Number:
                    reader.TryGetInt64(out v);
                    break;
                case JsonTokenType.True:
                    v = 1;
                    break;
                case JsonTokenType.False:
                    v = 0;
                    break;
                case JsonTokenType.Null:
                    break;
                default:
                    throw new InvalidOperationException($"{reader.TokenType} not convert to {typeToConvert.FullName}.");
            }

            return v;
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class FloatJsonConverter : JsonConverter<float>
    {
        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            float v = 0;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    float.TryParse(reader.GetString(), out v);
                    break;
                case JsonTokenType.Number:
                    reader.TryGetSingle(out v);
                    break;
                case JsonTokenType.True:
                    v = 1;
                    break;
                case JsonTokenType.False:
                    v = 0;
                    break;
                case JsonTokenType.Null:
                    break;
                default:
                    throw new InvalidOperationException($"{reader.TokenType} not convert to {typeToConvert.FullName}.");
            }

            return v;
        }

        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class DoubleJsonConverter : JsonConverter<double>
    {

        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            double v = 0;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    double.TryParse(reader.GetString(), out v);
                    break;
                case JsonTokenType.Number:
                    reader.TryGetDouble(out v);
                    break;
                case JsonTokenType.True:
                    v = 1;
                    break;
                case JsonTokenType.False:
                    v = 0;
                    break;
                case JsonTokenType.Null:
                    break;
                default:
                    throw new InvalidOperationException($"{reader.TokenType} not convert to {typeToConvert.FullName}.");
            }

            return v;
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class DecimalJsonConverter : JsonConverter<decimal>
    {

        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            decimal v = 0;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    decimal.TryParse(reader.GetString(), out v);
                    break;
                case JsonTokenType.Number:
                    reader.TryGetDecimal(out v);
                    break;
                case JsonTokenType.True:
                    v = 1;
                    break;
                case JsonTokenType.False:
                    v = 0;
                    break;
                case JsonTokenType.Null:
                    break;
                default:
                    throw new InvalidOperationException($"{reader.TokenType} not convert to {typeToConvert.FullName}.");
            }

            return v;
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
#endif
}
