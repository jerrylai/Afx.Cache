using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StackExchange.Redis;
using Afx.Cache.Interfaces;
using System.Threading.Tasks;

#if NETCOREAPP || NETSTANDARD
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
#else
using Newtonsoft.Json;
#endif

namespace Afx.Cache.Impl.Base
{
    /// <summary>
    /// redis 缓存
    /// </summary>
    public abstract class RedisCache : BaseCache, IRedisCache
    {
#if NETCOREAPP || NETSTANDARD
        private static readonly JsonSerializerOptions defaultOptions = new JsonSerializerOptions()
        {
            IgnoreNullValues = true,
            WriteIndented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = null,
            DictionaryKeyPolicy = null
        };

        public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
        static RedisCache()
        {
            //  options.Converters.Add(new DateTimeJsonConverter(DATE_FORMAT));
            defaultOptions.Converters.Add(new StringJsonConverter());
            defaultOptions.Converters.Add(new BoolJsonConverter());
            defaultOptions.Converters.Add(new IntJsonConverter());
            defaultOptions.Converters.Add(new LongJsonConverter());
            defaultOptions.Converters.Add(new FloatJsonConverter());
            defaultOptions.Converters.Add(new DoubleJsonConverter());
            defaultOptions.Converters.Add(new DecimalJsonConverter());
        }
#else
        private static readonly JsonSerializerSettings defaultOptions = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore
        };
#endif

#if NETCOREAPP || NETSTANDARD
        private JsonSerializerOptions options = defaultOptions;
#else
        private JsonSerializerSettings options = defaultOptions;
#endif
        /// <summary>
        /// ICacheKey
        /// </summary>
        protected ICacheKey cacheKey { get; private set; }

        /// <summary>
        /// IConnectionMultiplexer
        /// </summary>
        protected IConnectionMultiplexer redis { get; private set; }
        
        /// <summary>
        /// 缓存key配置
        /// </summary>
        public CacheKeyConfig KeyConfig { get; private set; }

        /// <summary>
        /// 缓存前缀
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// NodeName
        /// </summary>
        protected string NodeName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">缓存key配置db节点</param>
        /// <param name="item">缓存key配置项</param>
        /// <param name="redis">redis</param>
        /// <param name="cacheKey">ICacheKey</param>
        /// <param name="prefix">缓存前缀</param>
        public RedisCache(string node, string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
        {
            if (string.IsNullOrEmpty(node))
            {
                throw new ArgumentNullException("node");
            }
            if (string.IsNullOrEmpty(item))
            {
                throw new ArgumentNullException("item");
            }
            if (redis == null)
            {
                throw new ArgumentNullException("redis");
            }
            if (cacheKey == null)
            {
                throw new ArgumentNullException("cacheKey");
            }
            this.KeyConfig = cacheKey.Get(node, item);
            if(this.KeyConfig == null) throw new ArgumentException($"{node}/{item} 未配置！");
            this.redis = redis;
            this.cacheKey = cacheKey;
            this.Prefix = prefix ?? string.Empty;
            
            StringBuilder stringBuilder = new StringBuilder();
            foreach(var c in this.KeyConfig.Node)
            {
                if('A' <= c && c <= 'Z')
                {
                    if (stringBuilder.Length > 0) stringBuilder.Append($"_");
                    stringBuilder.Append((char)(c + 32));
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }
            stringBuilder.Append(":");
            this.NodeName = stringBuilder.ToString();
        }

#if NETCOREAPP || NETSTANDARD
        public virtual void SetJsonOptions(JsonSerializerOptions options)
        {
            if (options == null) return;
            this.options = options;
        }
#else
        public virtual void SetJsonOptions(JsonSerializerSettings options)
        {
            if (options == null) return;
            this.options = options;
        }
#endif
        /// <summary>
        /// 序列化json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual string Serialize<T>(T value)
        {
            if (value == null) return null;
#if NETCOREAPP || NETSTANDARD
            if (value is JsonElement)
            {
                object o = value;
                var el = (JsonElement)o;
                return el.GetRawText();
            }

            return JsonSerializer.Serialize(value, defaultOptions);
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
        protected virtual T Deserialize<T>(string json)
        {
            T m = default(T);
            if (string.IsNullOrEmpty(json)) return m;
#if NETCOREAPP || NETSTANDARD
            m = JsonSerializer.Deserialize<T>(json, defaultOptions);
#else
            m =  JsonConvert.DeserializeObject<T>(json, options);
#endif
            return m;
        }

        /// <summary>
        /// ToBytes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual byte[] ToBytes<T>(T value)
        {
            byte[] buffer = null;
            if (value != null)
            {
                if (value is byte[])
                {
                    buffer = value as byte[];
                }
                else if (value is string)
                {
                    string json = value as string;
                    buffer = Encoding.UTF8.GetBytes(json);
                }
                else
                {
                    string json = this.Serialize(value);
                    buffer = Encoding.UTF8.GetBytes(json);
                }
            }

            return buffer;
        }
        /// <summary>
        /// FromBytes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected virtual T FromBytes<T>(byte[] buffer)
        {
            T m = default(T);
            if (buffer != null)
            {
                if (typeof(T) == typeof(byte[]))
                {
                    m = (T)((object)buffer);
                }
                else if (buffer.Length > 0)
                {
                    string s = Encoding.UTF8.GetString(buffer);
                    if (typeof(T) == typeof(string))
                    {
                        m = (T)((object)s);
                    }
                    else if (!string.IsNullOrEmpty(s))
                    {
                        m = this.Deserialize<T>(s);
                    }
                }
            }

            return m;
        }

        /// <summary>
        /// 获取完整缓存key
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual string GetCacheKey(params object[] args)
        {
            if (string.IsNullOrEmpty(this.KeyConfig.Key)) throw new ArgumentNullException($"cache key(Node={this.KeyConfig.Node}, Item={this.KeyConfig.Item}) is null!", "key");
            var key = this.KeyConfig.Key;
            if (args != null && args.Length > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < args.Length; i++)
                {
                    var o = args[i];
                    if (o is Enum) o = (int)o;
                    stringBuilder.AppendFormat(":{0}", o?.ToString().ToLower() ?? "null");
                }
                key = $"{key}{stringBuilder.ToString()}";
            }

            return $"{this.Prefix}{this.NodeName}{key}";
        }

        /// <summary>
        /// 获取key所在db
        /// </summary>
        /// <param name="key">完整缓存key</param>
        /// <returns></returns>
        public virtual int GetCacheDb(string key)
        {
            var list = this.KeyConfig.Db ?? new List<int>(0);
            if (list.Count < 2) return list.FirstOrDefault();
            int hash = 0;
            foreach(var c in key)
            {
                hash += c;
                if (hash > 255) hash = hash % 255;
            }
            var db = list[hash % list.Count];

            return db;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual bool Remove(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);

            return database.KeyDelete(key);
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<bool> RemoveAsync(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);

            return await database.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 缓存key是否存在
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<bool> Contains(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);

            return await database.KeyExistsAsync(key);
        }

        /// <summary>
        /// 设置缓存有效时间
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<bool> Expire(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);

            return await database.KeyExpireAsync(key, this.KeyConfig.Expire);
        }

        /// <summary>
        /// 根据系统配置设置缓存有效时间
        /// </summary>
        /// <param name="expireIn">缓存有效时间</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<bool> Expire(TimeSpan? expireIn, params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);

            return await database.KeyExpireAsync(key, expireIn);
        }

        /// <summary>
        /// ping
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<TimeSpan>> Ping()
        {
            var eps = this.redis.GetEndPoints();
            List<TimeSpan> list = new List<TimeSpan>(eps.Count());
            foreach(var ep in eps)
            {
                var server = this.redis.GetServer(ep);
                var ts = await server.PingAsync();
                list.Add(ts);
            }
            return list;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.KeyConfig = null;
                this.NodeName = null;
                this.redis = null;
                this.cacheKey = null;
                this.Prefix = null;
            }
            base.Dispose(disposing);
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
