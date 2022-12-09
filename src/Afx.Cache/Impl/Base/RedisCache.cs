using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StackExchange.Redis;
using Afx.Cache.Interfaces;
using System.Threading.Tasks;


namespace Afx.Cache.Impl.Base
{
    /// <summary>
    /// redis 缓存
    /// </summary>
    public abstract class RedisCache : BaseCache, IRedisCache
    {
        /// <summary>
        /// 默认json序列化
        /// </summary>
        public static IJsonSerialize DefaultSerialize;
        private IJsonSerialize options;

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
            if (string.IsNullOrEmpty(node)) throw new ArgumentNullException("node");
            if (string.IsNullOrEmpty(item))throw new ArgumentNullException("item");
            if (redis == null)throw new ArgumentNullException("redis");
            if (cacheKey == null)throw new ArgumentNullException("cacheKey");
            if (DefaultSerialize == null) throw new ArgumentNullException("RedisCache.DefaultSerialize");
            this.KeyConfig = cacheKey.Get(node, item);
            if(this.KeyConfig == null) throw new ArgumentException($"{node}/{item} 未配置！");
            this.redis = redis;
            this.cacheKey = cacheKey;
            this.Prefix = prefix ?? string.Empty;
            this.options = DefaultSerialize;
            
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
        /// 
        /// </summary>
        /// <param name="jsonSerialize"></param>
        public virtual void SetJsonSerialize(IJsonSerialize jsonSerialize)
        {
            this.options = jsonSerialize ?? DefaultSerialize;
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
                    string json = this.options.Serialize(value);
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
                        m = this.options.Deserialize<T>(s);
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
        public virtual bool SyncRemove(params object[] args)
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
        public virtual async Task<bool> Remove(params object[] args)
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
                this.options = null;
            }
            base.Dispose(disposing);
        }

    }
}
