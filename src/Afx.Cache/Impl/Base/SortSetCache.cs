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
    /// 有序集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortSetCache<T> : RedisCache, ISortSetCache<T>
    {
        /// <summary>
        /// 有序集合
        /// </summary>
        /// <param name="node">缓存key配置db节点</param>
        /// <param name="item">缓存key配置项</param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public SortSetCache(string node, string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
             : base(node, item, redis, cacheKey, prefix)
        {

        }
        /// <summary>
        /// 减少 score
        /// </summary>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual double Decrement(T value, double score, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rv = this.ToBytes(value);
            var r = database.SortedSetDecrement(key, rv, score);

            return r;
        }
        /// <summary>
        /// 增加 score
        /// </summary>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual double Increment(T value, double score, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rv = this.ToBytes(value);
            var r = database.SortedSetIncrement(key, rv, score);

            return r;
        }
        /// <summary>
        /// 获取集合数量
        /// </summary>
        /// <param name="minScore"></param>
        /// <param name="maxScore"></param>
        /// <param name="excType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long GetCount(double minScore = double.NegativeInfinity, double maxScore = double.PositiveInfinity, ExcludeType excType = ExcludeType.None, params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.SortedSetLength(key, minScore, maxScore, (Exclude)(int)excType);

            return r;
        }
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="score"></param>
        /// <param name="when"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool AddOrUpdate(T value, double score, OpWhen when = OpWhen.Always, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rv = this.ToBytes(value);
            var r = database.SortedSetAdd(key, rv, score, (When)(int)when);

            return r;
        }
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="m"></param>
        /// <param name="when"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool AddOrUpdate(SortSetModel<T> m, OpWhen when = OpWhen.Always, params object[] args)
        {
            if (m == null) throw new ArgumentNullException(nameof(m));
            if (m.Value == null) throw new ArgumentNullException(nameof(m.Value));

            return this.AddOrUpdate(m.Value, m.Score, when, args);
        }
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="when"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long AddOrUpdate(List<SortSetModel<T>> list, OpWhen when = OpWhen.Always, params object[] args)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            foreach(var kv in list)
            {
                if(kv.Value == null) throw new ArgumentException($"{nameof(list)} Item.Value is null!", nameof(list));
            }
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rv = list.Select(q => new SortedSetEntry(this.ToBytes(q.Value), q.Score)).ToArray();
            var r = database.SortedSetAdd(key, rv, (When)(int)when);

            return r;
        }
        /// <summary>
        /// 返回并集合
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual SortSetModel<T> Pop(Sort sort = Sort.Asc, params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.SortedSetPop(key, (Order)(int)sort);
            var m = r.HasValue ? new SortSetModel<T>() { Value = this.FromBytes<T>(r.Value.Element), Score = r.Value.Score } : null;

            return m;
        }
        /// <summary>
        /// 返回并集合
        /// </summary>
        /// <param name="count"></param>
        /// <param name="sort"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual List<SortSetModel<T>> Pop(long count, Sort sort = Sort.Asc, params object[] args)
        {
            if (count <= 0) throw new ArgumentException(nameof(count));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = database.SortedSetPop(key, count, (Order)(int)sort);
            var list = rs?.Select(q => new SortSetModel<T> { Value = this.FromBytes<T>(q.Element), Score = q.Score }).ToList();

            return list;
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="sort"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual List<T> Get(long start = 0, long stop = -1, Sort sort = Sort.Asc, params object[] args)
        {
            if (start < 0) throw new ArgumentException(nameof(start));
            if (stop < -1 || stop != -1 && stop < start) throw new ArgumentException(nameof(stop));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = database.SortedSetRangeByRank(key, start, stop, (Order)(int)sort);
            var list = rs?.Select(q => this.FromBytes<T>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="sort"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual List<SortSetModel<T>> GetWithScores(long start = 0, long stop = -1, Sort sort = Sort.Asc, params object[] args)
        {
            if (start < 0) throw new ArgumentException(nameof(start));
            if (stop < -1 || stop != -1 && stop < start) throw new ArgumentException(nameof(stop));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = database.SortedSetRangeByRankWithScores(key, start, stop, (Order)(int)sort);
            var list = rs?.Select(q => new SortSetModel<T> { Value = this.FromBytes<T>(q.Element), Score = q.Score }).ToList();

            return list;
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="startScore"></param>
        /// <param name="stopScore"></param>
        /// <param name="excType"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual List<T> GetByScore(double startScore = double.NegativeInfinity, double stopScore = double.PositiveInfinity,
            ExcludeType excType = ExcludeType.None, Sort sort = Sort.Asc, long skip = 0, long take = -1, params object[] args)
        {
            if (skip < 0) throw new ArgumentException(nameof(skip));
            if (take < -1) throw new ArgumentException(nameof(take));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = database.SortedSetRangeByScore(key, startScore, stopScore, (Exclude)(int)excType, (Order)(int)sort, skip, take);
            var list = rs?.Select(q => this.FromBytes<T>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="startScore"></param>
        /// <param name="stopScore"></param>
        /// <param name="excType"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual List<SortSetModel<T>> GetByScoreWithScores(double startScore = double.NegativeInfinity,
            double stopScore = double.PositiveInfinity, ExcludeType excType = ExcludeType.None, Sort sort = Sort.Asc,
            long skip = 0, long take = -1, params object[] args)
        {
            if (skip < 0) throw new ArgumentException(nameof(skip));
            if (take < -1) throw new ArgumentException(nameof(take));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = database.SortedSetRangeByScoreWithScores(key, startScore, stopScore, (Exclude)(int)excType, (Order)(int)sort, skip, take);
            var list = rs?.Select(q => new SortSetModel<T> { Value = this.FromBytes<T>(q.Element), Score = q.Score }).ToList();

            return list;
        }
        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool Delete(T value, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rv = this.ToBytes(value);
            var r = database.SortedSetRemove(key, rv);

            return r;
        }
        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long Delete(List<T> list, params object[] args)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            foreach (var m in list)
            {
                if (m == null) throw new ArgumentException($"{nameof(list)} Item is null!", nameof(list));
            }
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rv = list.Select<T, RedisValue>(q => this.ToBytes(q)).ToArray();
            var r = database.SortedSetRemove(key, rv);

            return r;
        }
        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long Delete(long start, long stop, params object[] args)
        {
            if (start < 0) throw new ArgumentException(nameof(start));
            if (stop < start) throw new ArgumentException(nameof(stop));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = database.SortedSetRemoveRangeByRank(key, start, stop);

            return rs;
        }
        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="startScore"></param>
        /// <param name="stopScore"></param>
        /// <param name="excType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long DeleteByScore(double startScore, double stopScore, ExcludeType excType = ExcludeType.None, params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = database.SortedSetRemoveRangeByScore(key, startScore, stopScore, (Exclude)(int)excType);

            return rs;
        }

        /// <summary>
        /// 游标方式读取数据
        /// </summary>
        /// <param name="pattern">搜索表达式</param>
        /// <param name="start">开始位置</param>
        /// <param name="pageSize">游标页大小</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Scan(T pattern, int start, int pageSize, params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var el = database.SetScan(cachekey, this.ToBytes(pattern), pageSize, 0, start);

            return new Enumerable(this, el);
        }

        class Enumerator : IEnumerator<T>
        {
            private SortSetCache<T> setCache;
            private IEnumerator<RedisValue> enumerator;

            public Enumerator(SortSetCache<T> setCache, IEnumerator<RedisValue> enumerator)
            {
                this.setCache = setCache;
                this.enumerator = enumerator;
            }

            public T Current => this.setCache.FromBytes<T>(enumerator.Current);

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
                if (this.enumerator != null) this.enumerator.Dispose();
                this.setCache = null;
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

        class Enumerable : IEnumerable<T>
        {
            private SortSetCache<T> setCache;
            private IEnumerable<RedisValue> values;

            public Enumerable(SortSetCache<T> setCache, IEnumerable<RedisValue> values)
            {
                this.setCache = setCache;
                this.values = values;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new Enumerator(this.setCache, this.values.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
