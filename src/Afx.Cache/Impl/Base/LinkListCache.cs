using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using StackExchange.Redis;
using Afx.Cache.Interfaces;

namespace Afx.Cache.Impl.Base
{
    /// <summary>
    /// 链表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkListCache<T> : RedisCache, ILinkListCache<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">缓存key配置db节点</param>
        /// <param name="item">缓存key配置项</param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public LinkListCache(string node, string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
             : base(node, item, redis, cacheKey, prefix)
        {

        }
        /// <summary>
        /// 添加到左边第一个
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long PushLeft(T value, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            var v = this.ToBytes(value);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.ListLeftPush(key, v);

            return r;
        }
        /// <summary>
        /// 添加到左边第一个
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long PushLeft(List<T> list, params object[] args)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            foreach(var m in list)
            {
                if(m == null) throw new ArgumentException($"{nameof(list)} Item is null!", nameof(list));
            }
            string key = this.GetCacheKey(args);
            var v = list.Select<T, RedisValue>(q => this.ToBytes(q)).ToArray();
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.ListLeftPush(key, v);

            return r;
        }
        /// <summary>
        /// 添加到右边第一个
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long PushRight(T value, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            var v = this.ToBytes(value);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.ListRightPush(key, v);

            return r;
        }
        /// <summary>
        /// 添加到右边第一个
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long PushRight(List<T> list, params object[] args)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            foreach (var m in list)
            {
                if (m == null) throw new ArgumentException($"{nameof(list)} Item is null!", nameof(list));
            }
            string key = this.GetCacheKey(args);
            var v = list.Select<T, RedisValue>(q => this.ToBytes(q)).ToArray();
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.ListRightPush(key, v);

            return r;
        }
        /// <summary>
        /// 获取指定索引位置数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual T Get(long index, params object[] args)
        {
            if (index < 0) throw new ArgumentException(nameof(index));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.ListGetByIndex(key, index);
            T m = this.FromBytes<T>(r);

            return m;
        }
        /// <summary>
        /// 获取一个范围数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual List<T> GetRange(long start = 0, long stop = -1, params object[] args)
        {
            if (start < 0) throw new ArgumentException(nameof(start));
            if (stop < -1 || stop != -1 && stop < start) throw new ArgumentException(nameof(stop));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.ListRange(key, start, stop);
            var list = r?.Select(q => this.FromBytes<T>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long InsertAfter(T pivot, T value, params object[] args)
        {
            if (pivot == null) throw new ArgumentNullException(nameof(pivot));
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            var p = this.ToBytes(pivot);
            var v = this.ToBytes(value);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.ListInsertAfter(key, p, v);

            return r;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long InsertBefore(T pivot, T value, params object[] args)
        {
            if (pivot == null) throw new ArgumentNullException(nameof(pivot));
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            var p = this.ToBytes(pivot);
            var v = this.ToBytes(value);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.ListInsertBefore(key, p, v);

            return r;
        }
        /// <summary>
        /// 返回并移除左边第一个
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual T PopLeft(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.ListLeftPop(key);
            var m = this.FromBytes<T>(r);

            return m;
        }
        /// <summary>
        /// 返回并移除右边第一个
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual T PopRight(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.ListRightPop(key);
            var m = this.FromBytes<T>(r);

            return m;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="index">位置</param>
        /// <param name="value">更新后value</param>
        /// <param name="args">缓存key参数</param>
        public virtual bool Update(long index, T value, params object[] args)
        {
            if (index < 0) throw new ArgumentNullException(nameof(index));
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            var v = this.ToBytes(value);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            try { database.ListSetByIndex(key, index, v); }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="value">要删除的value</param>
        /// <param name="count">匹配数据个数，0.匹配所有</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual long Delete(T value, long count = 0, params object[] args)
        {
            if (count < 0) throw new ArgumentNullException(nameof(count));
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            var v = this.ToBytes(value);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.ListRemove(key, v, count);

            return r;
        }

        /// <summary>
        /// 移除指定区域之外的所有数据
        /// </summary>
        /// <param name="start">开始位置</param>
        /// <param name="stop">结束位置</param>
        /// <param name="args">缓存key参数</param>
        public virtual void Trim(long start, long stop, params object[] args)
        {
            if (start < 0) throw new ArgumentException(nameof(start));
            if (stop < start) throw new ArgumentException(nameof(stop));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            database.ListTrim(key, start, stop);
        }
        /// <summary>
        /// 获取链表长度
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        public virtual long GetCount(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.ListLength(key);

            return r;
        }
    }
}
