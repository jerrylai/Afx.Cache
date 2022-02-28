using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using StackExchange.Redis;
using Afx.Cache.Interfaces;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

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
        /// <param name="value">value</param>
        /// <param name="score">排序分</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<double> Decrement(T value, double score, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rv = this.ToBytes(value);
            var r = await database.SortedSetDecrementAsync(key, rv, score);

            return r;
        }
        /// <summary>
        /// 增加 score
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="score">排序分</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<double> Increment(T value, double score, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rv = this.ToBytes(value);
            var r = await database.SortedSetIncrementAsync(key, rv, score);

            return r;
        }
        /// <summary>
        /// 获取集合数量
        /// </summary>
        /// <param name="minScore">最小排序分</param>
        /// <param name="maxScore">最大排序分</param>
        /// <param name="excType">条件类型</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<long> GetCount(double minScore = double.NegativeInfinity, double maxScore = double.PositiveInfinity, ExcludeType excType = ExcludeType.None, params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = await database.SortedSetLengthAsync(key, minScore, maxScore, (Exclude)(int)excType);

            return r;
        }
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="score">排序分</param>
        /// <param name="when">操作类型</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<bool> AddOrUpdate(T value, double score, OpWhen when = OpWhen.Always, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rv = this.ToBytes(value);
            var r = await database.SortedSetAddAsync(key, rv, score, (When)(int)when);

            return r;
        }
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="m">SortSetModel</param>
        /// <param name="when">操作类型</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<bool> AddOrUpdate(SortSetModel<T> m, OpWhen when = OpWhen.Always, params object[] args)
        {
            if (m == null) throw new ArgumentNullException(nameof(m));
            if (m.Value == null) throw new ArgumentNullException(nameof(m.Value));

            return await this.AddOrUpdate(m.Value, m.Score, when, args);
        }
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="m">SortSetModel</param>
        /// <param name="when">操作类型</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<long> AddOrUpdate(List<SortSetModel<T>> list, OpWhen when = OpWhen.Always, params object[] args)
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
            var r = await database.SortedSetAddAsync(key, rv, (When)(int)when);

            return r;
        }
        /// <summary>
        /// 返回并集合
        /// </summary>
        /// <param name="sort">排序</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<SortSetModel<T>> Pop(Sort sort = Sort.Asc, params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = await database.SortedSetPopAsync(key, (Order)(int)sort);
            var m = r.HasValue ? new SortSetModel<T>() { Value = this.FromBytes<T>(r.Value.Element), Score = r.Value.Score } : null;

            return m;
        }
        /// <summary>
        /// 返回并集合
        /// </summary>
        /// <param name="count">返回数量</param>
        /// <param name="sort">排序</</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<List<SortSetModel<T>>> Pop(long count, Sort sort = Sort.Asc, params object[] args)
        {
            if (count <= 0) throw new ArgumentException(nameof(count));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = await database.SortedSetPopAsync(key, count, (Order)(int)sort);
            var list = rs?.Select(q => new SortSetModel<T> { Value = this.FromBytes<T>(q.Element), Score = q.Score }).ToList();

            return list;
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="start">开始位置</param>
        /// <param name="stop">结束位置</param>
        /// <param name="sort">排序</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<List<T>> Get(long start = 0, long stop = -1, Sort sort = Sort.Asc, params object[] args)
        {
            if (start < 0) throw new ArgumentException(nameof(start));
            if (stop < -1 || stop != -1 && stop < start) throw new ArgumentException(nameof(stop));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = await database.SortedSetRangeByRankAsync(key, start, stop, (Order)(int)sort);
            var list = rs?.Select(q => this.FromBytes<T>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="start">开始位置</param>
        /// <param name="stop">结束位置</param>
        /// <param name="sort">排序</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<List<SortSetModel<T>>> GetWithScores(long start = 0, long stop = -1, Sort sort = Sort.Asc, params object[] args)
        {
            if (start < 0) throw new ArgumentException(nameof(start));
            if (stop < -1 || stop != -1 && stop < start) throw new ArgumentException(nameof(stop));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = await database.SortedSetRangeByRankWithScoresAsync(key, start, stop, (Order)(int)sort);
            var list = rs?.Select(q => new SortSetModel<T> { Value = this.FromBytes<T>(q.Element), Score = q.Score }).ToList();

            return list;
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="startScore">开始位置排序分</param>
        /// <param name="stopScore">结束位置排序分</param>
        /// <param name="excType">条件类型</param>
        /// <param name="sort">排序</param>
        /// <param name="skip">跳过多少个</param>
        /// <param name="take">返回多少个</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetByScore(double startScore = double.NegativeInfinity, double stopScore = double.PositiveInfinity,
            ExcludeType excType = ExcludeType.None, Sort sort = Sort.Asc, long skip = 0, long take = -1, params object[] args)
        {
            if (skip < 0) throw new ArgumentException(nameof(skip));
            if (take < -1) throw new ArgumentException(nameof(take));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = await database.SortedSetRangeByScoreAsync(key, startScore, stopScore, (Exclude)(int)excType, (Order)(int)sort, skip, take);
            var list = rs?.Select(q => this.FromBytes<T>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="startScore">开始位置排序分</param>
        /// <param name="stopScore">结束位置排序分</param>
        /// <param name="excType">条件类型</param>
        /// <param name="sort">排序</param>
        /// <param name="skip">跳过多少个</param>
        /// <param name="take">返回多少个</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<List<SortSetModel<T>>> GetByScoreWithScores(double startScore = double.NegativeInfinity,
            double stopScore = double.PositiveInfinity, ExcludeType excType = ExcludeType.None, Sort sort = Sort.Asc,
            long skip = 0, long take = -1, params object[] args)
        {
            if (skip < 0) throw new ArgumentException(nameof(skip));
            if (take < -1) throw new ArgumentException(nameof(take));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = await database.SortedSetRangeByScoreWithScoresAsync(key, startScore, stopScore, (Exclude)(int)excType, (Order)(int)sort, skip, take);
            var list = rs?.Select(q => new SortSetModel<T> { Value = this.FromBytes<T>(q.Element), Score = q.Score }).ToList();

            return list;
        }
        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<bool> Delete(T value, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rv = this.ToBytes(value);
            var r = await database.SortedSetRemoveAsync(key, rv);

            return r;
        }
        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="list">value List</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<long> Delete(List<T> list, params object[] args)
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
            var r = await database.SortedSetRemoveAsync(key, rv);

            return r;
        }
        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="start">开始位置</param>
        /// <param name="stop">结束位置</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<long> Delete(long start, long stop, params object[] args)
        {
            if (start < 0) throw new ArgumentException(nameof(start));
            if (stop < start) throw new ArgumentException(nameof(stop));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = await database.SortedSetRemoveRangeByRankAsync(key, start, stop);

            return rs;
        }
        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="startScore">开始位置排序分</param>
        /// <param name="stopScore">结束位置排序分</param>
        /// <param name="excType">条件类型</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual async Task<long> DeleteByScore(double startScore, double stopScore, ExcludeType excType = ExcludeType.None, params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = await database.SortedSetRemoveRangeByScoreAsync(key, startScore, stopScore, (Exclude)(int)excType);

            return rs;
        }

        /// <summary>
        /// 游标方式读取数据
        /// </summary>
        /// <param name="pattern">搜索表达式</param>
        /// <param name="start">开始位置</param>
        /// <param name="pageSize">游标页大小</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<T> Scan(string pattern, int start, int pageSize, params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var el = database.SetScanAsync(cachekey, this.ToBytes(pattern), pageSize, 0, start);

            return new AsyncEnumerable(this, el);
        }

        class AsyncEnumerator : IAsyncEnumerator<T>
        {
            private SortSetCache<T> setCache;
            private IAsyncEnumerator<RedisValue> enumerator;

            public AsyncEnumerator(SortSetCache<T> setCache, IAsyncEnumerator<RedisValue> enumerator)
            {
                this.setCache = setCache;
                this.enumerator = enumerator;
            }

            public T Current => this.setCache.FromBytes<T>(enumerator.Current);

            public virtual async ValueTask DisposeAsync()
            {
                if (this.enumerator != null) await this.enumerator.DisposeAsync();
                this.setCache = null;
                this.enumerator = null;
            }

            public virtual async ValueTask<bool> MoveNextAsync()
            {
                return await this.enumerator.MoveNextAsync();
            }
        }

        class AsyncEnumerable : IAsyncEnumerable<T>
        {
            private SortSetCache<T> setCache;
            private IAsyncEnumerable<RedisValue> values;

            public AsyncEnumerable(SortSetCache<T> setCache, IAsyncEnumerable<RedisValue> values)
            {
                this.setCache = setCache;
                this.values = values;
            }

            public virtual IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new AsyncEnumerator(this.setCache, this.values.GetAsyncEnumerator(cancellationToken));
            }
        }
    }
}
