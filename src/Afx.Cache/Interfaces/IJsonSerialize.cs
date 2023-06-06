using System;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Afx.Cache
{
    /// <summary>
    /// json Serialize
    /// </summary>
    public interface IJsonSerialize
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        string Serialize<T>(T m);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        T Deserialize<T>(string json);
    }

    /// <summary>
    /// 
    /// </summary>
    internal class RedisJson: IJsonSerialize
    {
        /// <summary>
        /// 日期序列化格式
        /// </summary>
        public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

        private static readonly JsonSerializerOptions options;

        static RedisJson()
        {
            options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = false,
                PropertyNamingPolicy = null,
                DictionaryKeyPolicy = null
            };

            options.Converters.Add(new StringJsonConverter());
            options.Converters.Add(new BoolJsonConverter());
            options.Converters.Add(new DateTimeJsonConverter(DATE_FORMAT));
            options.Converters.Add(new IntJsonConverter());
            options.Converters.Add(new LongJsonConverter());
            options.Converters.Add(new FloatJsonConverter());
            options.Converters.Add(new DoubleJsonConverter());
            options.Converters.Add(new DecimalJsonConverter());
        }

        /// <summary>
        /// 序列化json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Serialize<T>(T value)
        {
            if (value == null) return null;

            return JsonSerializer.Serialize(value, options);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T Deserialize<T>(string json)
        {
            T m = default(T);
            if (string.IsNullOrEmpty(json)) return m;
            m = JsonSerializer.Deserialize<T>(json, options);

            return m;
        }
    }

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int v = 0;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    int.TryParse(reader.GetString(), out v);
                    break;
                case JsonTokenType.Number:
                    double d;
                    if (reader.TryGetDouble(out d))
                    {
                        v = Convert.ToInt32(d);
                    }
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            long v = 0;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    long.TryParse(reader.GetString(), out v);
                    break;
                case JsonTokenType.Number:
                    double d;
                    if (reader.TryGetDouble(out d))
                    {
                        v = Convert.ToInt64(d);
                    }
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
