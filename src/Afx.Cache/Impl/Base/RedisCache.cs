using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETCOREAPP || NETSTANDARD
using System.Text.Json;
#else
using Newtonsoft.Json;
#endif
using StackExchange.Redis;
using Afx.Cache.Interfaces;

namespace Afx.Cache.Impl.Base
{
    /// <summary>
    /// redis 缓存
    /// </summary>
    public abstract class RedisCache : BaseCache, IRedisCache
    {
#if NETCOREAPP || NETSTANDARD
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            IgnoreNullValues = true,
            WriteIndented = true
        };
#else
        private static readonly JsonSerializerSettings jsonSerializerOptions = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
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
        /// 序列化json
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual string Serialize<T>(T value)
        {
            if (value == null) return null;
#if NETCOREAPP || NETSTANDARD
            return JsonSerializer.Serialize(value, jsonSerializerOptions);
#else
            return JsonConvert.SerializeObject(value, jsonSerializerOptions);
#endif
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        protected virtual T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default(T);

#if NETCOREAPP || NETSTANDARD
            return JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);
#else
            return JsonConvert.DeserializeObject<T>(json, jsonSerializerOptions);
#endif
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
        /// 缓存key配置
        /// </summary>
        public virtual CacheKeyModel KeyConfig { get; private set; }

        /// <summary>
        /// 缓存前缀
        /// </summary>
        public virtual string Prefix { get; private set; }

        /// <summary>
        /// NodeName
        /// </summary>
        protected virtual string NodeName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">缓存key配置db节点</param>
        /// <param name="item">缓存key配置项</param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
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

        /// <summary>
        /// 获取缓存key
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
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual int GetCacheDb(string key)
        {
            var list = this.KeyConfig.Db ?? new List<int>(0);
            if (list.Count < 2) return list.FirstOrDefault();
            var hash = key.GetHashCode();
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
        /// 缓存key是否存在
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual bool Contains(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);

            return database.KeyExists(key);
        }

        /// <summary>
        /// 设置缓存有效时间
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual bool Expire(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);

            return database.KeyExpire(key, this.KeyConfig.Expire);
        }

        /// <summary>
        /// 根据系统配置设置缓存有效时间
        /// </summary>
        /// <param name="expireIn">缓存有效时间</param>
        /// <param name="args"></param>
        /// <returns>缓存key参数</returns>
        public virtual bool Expire(TimeSpan? expireIn, params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);

            return database.KeyExpire(key, expireIn);
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
}
