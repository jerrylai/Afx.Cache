using System;
using System.Collections.Generic;
using System.Linq;

using StackExchange.Redis;
using Afx.Cache.Interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Afx.Cache.Impl.Base
{
    /// <summary>
    /// hash
    /// </summary>
    /// <typeparam name="TField">hash key数据类型</typeparam>
    /// <typeparam name="TValue">hash value数据类型</typeparam>
    public class HashCache<TField, TValue> : RedisCache, IHashCache<TField, TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">缓存key配置db节点</param>
        /// <param name="item">缓存key配置项</param>
        /// <param name="redis">redis</param>
        /// <param name="cacheKey">ICacheKey</param>
        /// <param name="prefix">缓存前缀</param>
        public HashCache(string node, string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
             : base(node, item, redis, cacheKey, prefix)
        {
            
        }

        /// <summary>
        /// hash value 原子自减，TValue 必须是 long、int类型
        /// </summary>
        /// <param name="field">hash key</param>
        /// <param name="value">hash value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<long> Decrement(TField field, long value = 1, params object[] args)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            var t = typeof(TValue);
            if (!(t == typeof(int) || t == typeof(long) || t == typeof(uint) || t == typeof(ulong)))
            {
                throw new ArgumentException($"TValue({t.Name}) is not int or long!", nameof(TValue));
            }
            string cachekey = this.GetCacheKey(args);
            RedisValue rv = this.ToBytes(field);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var v = await database.HashDecrementAsync(cachekey, rv, value);

            return v;
        }
        /// <summary>
        /// hash value 原子自增，TValue 必须是 long、int类型
        /// </summary>
        /// <param name="field">hash key</param>
        /// <param name="value">hash value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<long> Increment(TField field, long value = 1, params object[] args)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            var t = typeof(TValue);
            if (!(t == typeof(int) || t == typeof(long) || t == typeof(uint) || t == typeof(ulong)))
            {
                throw new ArgumentException($"TValue({t.Name}) is not int or long!", nameof(TValue));
            }
            string cachekey = this.GetCacheKey(args);
            RedisValue rv = this.ToBytes(field);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var v = await database.HashIncrementAsync(cachekey, rv, value);

            return v;
        }
        /// <summary>
        /// 是否存在hash key
        /// </summary>
        /// <param name="field">hash key</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<bool> Exists(TField field, params object[] args)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            string cachekey = this.GetCacheKey(args);
            RedisValue rv = this.ToBytes(field);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var v = await database.HashExistsAsync(cachekey, rv);

            return v;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<TField, TValue>> Get(params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = await database.HashGetAllAsync(cachekey);
            Dictionary<TField, TValue> dic = rs?.Select(q=> new KeyValuePair<TField, TValue>(this.FromBytes<TField>(q.Name), this.FromBytes<TValue>(q.Value))).ToDictionary(q => q.Key, q => q.Value);

            return dic;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="field">hash key</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<TValue> GetValue(TField field, params object[] args)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            string cachekey = this.GetCacheKey(args);
            RedisValue rv = this.ToBytes(field);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var v = await database.HashGetAsync(cachekey, rv);
            TValue m = this.FromBytes<TValue>(v);

            return m;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keys">hash key</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<List<TValue>> GetValue(List<TField> keys, params object[] args)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            if (keys.Count == 0) return new List<TValue>(0);
            RedisValue[] rvs = new RedisValue[keys.Count];
            int i = 0;
            foreach (var m in keys)
            {
                if (m == null) throw new ArgumentException($"{nameof(keys)} Item is null!", nameof(keys));
                rvs[i++] = this.ToBytes(m);
            }
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = await database.HashGetAsync(cachekey, rvs);
            List<TValue> list = rs?.Select(q => this.FromBytes<TValue>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 获取hash key 数量
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<long> GetCount(params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = await database.HashLengthAsync(cachekey);

            return rs;
        }
        /// <summary>
        /// 获取hash key
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<List<TField>> GeTFields(params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = await database.HashKeysAsync(cachekey);
            List<TField> list = rs?.Select(q => this.FromBytes<TField>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 获取hash value
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<List<TValue>> GetValues(params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = await database.HashValuesAsync(cachekey);
            List<TValue> list = rs?.Select(q => this.FromBytes<TValue>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 移除hash key
        /// </summary>
        /// <param name="field">hash key</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<bool> Delete(TField field, params object[] args)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            string cachekey = this.GetCacheKey(args);
            RedisValue rv = this.ToBytes(field);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = await database.HashDeleteAsync(cachekey, rv);

            return rs;
        }
        /// <summary>
        /// 移除hash key
        /// </summary>
        /// <param name="fields">hash key</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<long> Delete(List<TField> fields, params object[] args)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            if (fields.Count == 0) return 0;
            RedisValue[] rvs = new RedisValue[fields.Count];
            int i = 0;
            foreach (var m in fields)
            {
                if (m == null) throw new ArgumentException($"{nameof(fields)} Item is null!", nameof(fields));
                rvs[i++] = this.ToBytes(m);
            }
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = await database.HashDeleteAsync(cachekey, rvs);

            return rs;
        }
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="field">hash key</param>
        /// <param name="value">hash value</param>
        /// <param name="when">操作</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<bool> Set(TField field, TValue value, OpWhen when = OpWhen.Always, params object[] args)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            string cachekey = this.GetCacheKey(args);
            RedisValue rf = this.ToBytes(field);
            RedisValue rv = this.ToBytes(value);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = await database.HashSetAsync(cachekey, rf, rv, (When)(int)when);

            return rs;
        }
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="args">缓存key参数</param>
        public virtual async Task AddOrUpdate(Dictionary<TField, TValue> dic, params object[] args)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            if (dic.Count == 0) return;
            HashEntry[] hs = new HashEntry[dic.Count];
            int i = 0;
            foreach (KeyValuePair<TField, TValue> kv in dic)
            {
                if (kv.Key == null) throw new ArgumentException($"{nameof(dic)} Key is null!", nameof(dic));
                hs[i++] = new HashEntry(this.ToBytes(kv.Key), this.ToBytes(kv.Value));
            }
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            await database.HashSetAsync(cachekey, hs);
        }

        /// <summary>
        /// 游标方式读取数据
        /// </summary>
        /// <param name="pattern">搜索表达式</param>
        /// <param name="start">开始位置</param>
        /// <param name="pageSize">游标页大小</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<KeyValuePair<TField, TValue>> Scan(string pattern, int start, int pageSize, params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var el = database.HashScanAsync(cachekey, this.ToBytes(pattern), pageSize, 0, start);

            return new AsyncEnumerable(this, el);
        }

        class AsyncEnumerator : IAsyncEnumerator<KeyValuePair<TField, TValue>>
        {
            private HashCache<TField, TValue> hashCache;
            private IAsyncEnumerator<HashEntry> enumerator;
            
            public AsyncEnumerator(HashCache<TField, TValue> hashCache, IAsyncEnumerator<HashEntry> enumerator)
            {
                this.hashCache = hashCache;
                this.enumerator = enumerator;
            }

            public KeyValuePair<TField, TValue> Current
            {
                get
                {
                    var name = enumerator.Current.Name;
                    var v = enumerator.Current.Value;

                    return new KeyValuePair<TField, TValue>(this.hashCache.FromBytes<TField>(name), this.hashCache.FromBytes<TValue>(v));
                }
            }
            
            public virtual async ValueTask<bool> MoveNextAsync()
            {
               return await enumerator.MoveNextAsync();
            }

            public virtual async ValueTask DisposeAsync()
            {
                if (this.enumerator != null) await this.enumerator.DisposeAsync();
                this.hashCache = null;
                this.enumerator = null;
            }

        }

        class AsyncEnumerable : IAsyncEnumerable<KeyValuePair<TField, TValue>>
        {
            private HashCache<TField, TValue> hashCache;
            private IAsyncEnumerable<HashEntry> hashes;

            public AsyncEnumerable(HashCache<TField, TValue> hashCache, IAsyncEnumerable<HashEntry> hashes)
            {
                this.hashCache = hashCache;
                this.hashes = hashes;
            }

            public virtual IAsyncEnumerator<KeyValuePair<TField, TValue>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new AsyncEnumerator(this.hashCache, this.hashes.GetAsyncEnumerator(cancellationToken));
            }
        }
    }
}
