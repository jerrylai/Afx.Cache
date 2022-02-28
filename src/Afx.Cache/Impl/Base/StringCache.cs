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
    /// redis string 缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StringCache<T> : RedisCache, IStringCache<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">缓存key配置db节点</param>
        /// <param name="item">缓存key配置项</param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public StringCache(string node, string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
             : base(node, item, redis, cacheKey, prefix)
        {
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<T> Get(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var value = await database.StringGetAsync(key);
            T m = this.FromBytes<T>(value);

            return m;
        }
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="m"></param>
        /// <param name="when"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task<bool> Set(T m, OpWhen when = OpWhen.Always, params object[] args)
        {
            return await this.Set(m, this.KeyConfig.Expire, when, args);
        }
        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="m"></param>
        /// <param name="expireIn"></param>
        /// <param name="when"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task<bool> Set(T m, TimeSpan? expireIn, OpWhen when = OpWhen.Always, params object[] args)
        {
            bool result = false;
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            if (m == null) result = await database.KeyDeleteAsync(key);
            else result = await database.StringSetAsync(key, this.ToBytes(m), expireIn, (When)(int)when);

            return result;
        }
        /// <summary>
        /// 原子增 T 必须是 int、 long
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task<long> Increment(long value = 1, params object[] args)
        {
            var t = typeof(T);
            if(!(t == typeof(int) || t == typeof(long) || t == typeof(uint) || t == typeof(ulong)))
            {
                throw new ArgumentException($"T({t.Name}) is not int or long!", nameof(T));
            }
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var v = await database.StringIncrementAsync(key, value);

            return v;
        }
        /// <summary>
        /// 原子减 T 必须是 int、 long
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual async Task<long> Decrement(long value = 1, params object[] args)
        {
            var t = typeof(T);
            if (!(t == typeof(int) || t == typeof(long) || t == typeof(uint) || t == typeof(ulong)))
            {
                throw new ArgumentException($"T({t.Name}) is not int or long!", nameof(T));
            }
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var v = await database.StringDecrementAsync(key, value);

            return v;
        }
    }
}
