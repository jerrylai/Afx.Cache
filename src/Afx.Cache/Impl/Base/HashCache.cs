using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using StackExchange.Redis;
using Afx.Cache.Interfaces;
using System.Collections;

namespace Afx.Cache.Impl.Base
{
    /// <summary>
    /// hash
    /// </summary>
    /// <typeparam name="TKey">hash key数据类型</typeparam>
    /// <typeparam name="TValue">hash value数据类型</typeparam>
    public class HashCache<TKey, TValue> : RedisCache, IHashCache<TKey, TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">缓存key配置db节点</param>
        /// <param name="item">缓存key配置项</param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public HashCache(string node, string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
             : base(node, item, redis, cacheKey, prefix)
        {
            
        }

        /// <summary>
        /// hash value 原子自减，TValue 必须是 long、int类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long Decrement(TKey key, long value = 1, params object[] args)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var t = typeof(TValue);
            if (!(t == typeof(int) || t == typeof(long) || t == typeof(uint) || t == typeof(ulong)))
            {
                throw new ArgumentException($"TValue({t.Name}) is not int or long!", nameof(TValue));
            }
            string cachekey = this.GetCacheKey(args);
            RedisValue rv = this.ToBytes(key);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var v = database.HashDecrement(cachekey, rv, value);

            return v;
        }
        /// <summary>
        /// hash value 原子自增，TValue 必须是 long、int类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long Increment(TKey key, long value = 1, params object[] args)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var t = typeof(TValue);
            if (!(t == typeof(int) || t == typeof(long) || t == typeof(uint) || t == typeof(ulong)))
            {
                throw new ArgumentException($"TValue({t.Name}) is not int or long!", nameof(TValue));
            }
            string cachekey = this.GetCacheKey(args);
            RedisValue rv = this.ToBytes(key);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var v = database.HashIncrement(cachekey, rv, value);

            return v;
        }
        /// <summary>
        /// 是否存在hash key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool Exists(TKey key, params object[] args)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            string cachekey = this.GetCacheKey(args);
            RedisValue rv = this.ToBytes(key);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var v = database.HashExists(cachekey, rv);

            return v;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual Dictionary<TKey, TValue> Get(params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = database.HashGetAll(cachekey);
            Dictionary<TKey, TValue> dic = rs?.Select(q=> new KeyValuePair<TKey, TValue>(this.FromBytes<TKey>(q.Name), this.FromBytes<TValue>(q.Value))).ToDictionary(q => q.Key, q => q.Value);

            return dic;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual TValue GetValue(TKey key, params object[] args)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            string cachekey = this.GetCacheKey(args);
            RedisValue rv = this.ToBytes(key);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var v = database.HashGet(cachekey, rv);
            TValue m = this.FromBytes<TValue>(v);

            return m;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual List<TValue> GetValue(List<TKey> keys, params object[] args)
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
            var rs = database.HashGet(cachekey, rvs);
            List<TValue> list = rs?.Select(q => this.FromBytes<TValue>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 获取hash key 数量
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long GetCount(params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = database.HashLength(cachekey);

            return rs;
        }
        /// <summary>
        /// 获取hash key
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual List<TKey> GetKeys(params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = database.HashKeys(cachekey);
            List<TKey> list = rs?.Select(q => this.FromBytes<TKey>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 获取hash value
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual List<TValue> GetValues(params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = database.HashValues(cachekey);
            List<TValue> list = rs?.Select(q => this.FromBytes<TValue>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 移除hash key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool Delete(TKey key, params object[] args)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            string cachekey = this.GetCacheKey(args);
            RedisValue rv = this.ToBytes(key);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = database.HashDelete(cachekey, rv);

            return rs;
        }
        /// <summary>
        /// 移除hash key
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long Delete(List<TKey> keys, params object[] args)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            if (keys.Count == 0) return 0;
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
            var rs = database.HashDelete(cachekey, rvs);

            return rs;
        }
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="when"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool Set(TKey key, TValue value, OpWhen when = OpWhen.Always, params object[] args)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            string cachekey = this.GetCacheKey(args);
            RedisValue rf = this.ToBytes(key);
            RedisValue rv = this.ToBytes(value);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = database.HashSet(cachekey, rf, rv, (When)(int)when);

            return rs;
        }
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="args"></param>
        public virtual void AddOrUpdate(Dictionary<TKey, TValue> dic, params object[] args)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            if (dic.Count == 0) return;
            HashEntry[] hs = new HashEntry[dic.Count];
            int i = 0;
            foreach (KeyValuePair<TKey, TValue> kv in dic)
            {
                if (kv.Key == null) throw new ArgumentException($"{nameof(dic)} Key is null!", nameof(dic));
                hs[i++] = new HashEntry(this.ToBytes(kv.Key), this.ToBytes(kv.Value));
            }
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            database.HashSet(cachekey, hs);
        }

        /// <summary>
        /// 游标方式读取数据
        /// </summary>
        /// <param name="pattern">搜索表达式</param>
        /// <param name="start">开始位置</param>
        /// <param name="pageSize">游标页大小</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IEnumerable<KeyValuePair<TKey, TValue>> Scan(TKey pattern, int start, int pageSize, params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var el = database.HashScan(cachekey, this.ToBytes(pattern), pageSize, 0, start);

            return new Enumerable(this, el);
        }

        class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private HashCache<TKey, TValue> hashCache;
            private IEnumerator<HashEntry> enumerator;
            
            public Enumerator(HashCache<TKey, TValue> hashCache, IEnumerator<HashEntry> enumerator)
            {
                this.hashCache = hashCache;
                this.enumerator = enumerator;
            }

            public KeyValuePair<TKey, TValue> Current => new KeyValuePair<TKey, TValue>(this.hashCache.FromBytes<TKey>(enumerator.Current.Name), this.hashCache.FromBytes<TValue>(enumerator.Current.Value));

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
                if (this.enumerator != null) this.enumerator.Dispose();
                this.hashCache = null;
                this.enumerator = null;
            }

            public bool MoveNext()
            {
               return this.enumerator.MoveNext();
            }

            public void Reset()
            {
                this.enumerator.Reset();
            }
        }

        class Enumerable: IEnumerable<KeyValuePair<TKey, TValue>>
        {
            private HashCache<TKey, TValue> hashCache;
            private IEnumerable<HashEntry> hashes;

            public Enumerable(HashCache<TKey, TValue> hashCache, IEnumerable<HashEntry> hashes)
            {
                this.hashCache = hashCache;
                this.hashes = hashes;
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return new Enumerator(this.hashCache, this.hashes.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
