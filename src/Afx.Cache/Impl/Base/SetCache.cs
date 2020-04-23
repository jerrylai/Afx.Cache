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
    /// set 集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SetCache<T> : RedisCache, ISetCache<T>
    {
        /// <summary>
        /// set 集合
        /// </summary>
        /// <param name="node">缓存key配置db节点</param>
        /// <param name="item">缓存key配置项</param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public SetCache(string node, string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
             : base(node, item, redis, cacheKey, prefix)
        {
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool Add(T value, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rv = this.ToBytes(value);
            var r = database.SetAdd(key, rv);

            return r;
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long Add(List<T> list, params object[] args)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count == 0) return 0;
            list = list.Distinct().ToList();
            foreach(var m in list)
            {
                if(m == null) throw new ArgumentException($"{nameof(list)} Item is null!", nameof(list));
            }
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rvs = list.Select<T, RedisValue>(q => this.ToBytes(q)).ToArray();
            var r = database.SetAdd(key, rvs);

            return r;
        }
        /// <summary>
        /// 是否存在集合
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool Exist(T value, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rv = this.ToBytes(value);
            var r = database.SetContains(key, rv);

            return r;
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual List<T> Get(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = database.SetMembers(key);
            List<T> list = rs?.Select(q => this.FromBytes<T>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 集合数量
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long GetCount(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.SetLength(key);

            return r;
        }
        /// <summary>
        /// 随机返回对象
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual T GetRandomValue(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.SetRandomMember(key);
            var v = this.FromBytes<T>(r);

            return v;
        }
        /// <summary>
        /// 随机返回对象
        /// </summary>
        /// <param name="count"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual List<T> GetRandomValue(int count, params object[] args)
        {
            if (count <= 0) throw new ArgumentException($"{nameof(count)}({count}) is error!", nameof(count));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = database.SetRandomMembers(key, count);
            List<T> list = rs?.Select(q => this.FromBytes<T>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 两个集合运算，返回运算结果
        /// </summary>
        /// <param name="firstArgs">集合1参数</param>
        /// <param name="secondArgs">集合2参数</param>
        /// <param name="op"></param>
        /// <returns></returns>
        public virtual List<T> Join(object[] firstArgs, object[] secondArgs, SetOp op)
        {
            if (firstArgs == secondArgs
                || firstArgs != null && secondArgs!= null
                && firstArgs.Length == 0 && secondArgs.Length == 0)
                throw new ArgumentException("firstArgs == secondArgs is error!");
            if(firstArgs != null && secondArgs != null
                && firstArgs.Length == secondArgs.Length)
            {
                bool isok = true;
                for(var i=0; i<firstArgs.Length; i++)
                {
                    if(firstArgs[i] != secondArgs[i])
                    {
                        isok = false;
                        break;
                    }
                }
                if(isok) throw new ArgumentException("firstArgs == secondArgs is error!");
            }
            string firstKey = this.GetCacheKey(firstArgs);
            string secondKey = this.GetCacheKey(secondArgs);
            int db = this.GetCacheDb(firstKey);
            var database = this.redis.GetDatabase(db);
            RedisKey[] redisKeys = new RedisKey[2] { firstKey, secondKey };
            var rs = database.SetCombine((SetOperation)((int)op), redisKeys);
            List<T> list = rs?.Select(q => this.FromBytes<T>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 两个集合运算，并将运算结果存储到新集合
        /// </summary>
        /// <param name="addArgs">新集合</param>
        /// <param name="firstArgs">集合1参数</param>
        /// <param name="secondArgs">集合2参数</param>
        /// <param name="op"></param>
        /// <returns></returns>
        public virtual long JoinAndAdd(object[] addArgs, object[] firstArgs, object[] secondArgs, SetOp op)
        {
            if (firstArgs == secondArgs
                || firstArgs != null && secondArgs != null
                && firstArgs.Length == 0 && secondArgs.Length == 0)
                throw new ArgumentException("firstArgs == secondArgs is error!");
            if (firstArgs != null && secondArgs != null
                && firstArgs.Length == secondArgs.Length)
            {
                bool isok = true;
                for (var i = 0; i < firstArgs.Length; i++)
                {
                    if (firstArgs[i] != secondArgs[i])
                    {
                        isok = false;
                        break;
                    }
                }
                if (isok) throw new ArgumentException("firstArgs == secondArgs is error!");
            }
            string addKey = this.GetCacheKey(addArgs);
            string firstKey = this.GetCacheKey(firstArgs);
            string secondKey = this.GetCacheKey(secondArgs);
            int db = this.GetCacheDb(firstKey);
            var database = this.redis.GetDatabase(db);
            var r = database.SetCombineAndStore((SetOperation)((int)op), addKey, firstKey, secondKey);

            return r;
        }
        /// <summary>
        /// 移动一个已存在对象到新集合
        /// </summary>
        /// <param name="sourceArgs">原来集合参数</param>
        /// <param name="desArgs">需要移到新集合参数</param>
        /// <param name="value">移动对象</param>
        /// <returns></returns>
        public virtual bool Move(object[] sourceArgs, object[] desArgs, T value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (sourceArgs == desArgs
                || sourceArgs != null && desArgs != null
                && sourceArgs.Length == 0 && desArgs.Length == 0)
                throw new ArgumentException("sourceArgs == desArgs is error!");
            if (sourceArgs != null && desArgs != null
                && sourceArgs.Length == desArgs.Length)
            {
                bool isok = true;
                for (var i = 0; i < sourceArgs.Length; i++)
                {
                    if (sourceArgs[i] != desArgs[i])
                    {
                        isok = false;
                        break;
                    }
                }
                if (isok) throw new ArgumentException("sourceArgs == desArgs is error!");
            }
            string sourceKey = this.GetCacheKey(sourceArgs);
            string desKey = this.GetCacheKey(desArgs);
            RedisValue v = this.ToBytes<T>(value);
            int db = this.GetCacheDb(sourceKey);
            var database = this.redis.GetDatabase(db);
            var r = database.SetMove(sourceKey, desKey, v);

            return r;
        }
        /// <summary>
        /// 返回并移除一个集合对象
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual T Pop(params object[] args)
        {
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.SetPop(key);
            var v = this.FromBytes<T>(r);

            return v;
        }
        /// <summary>
        /// 返回并移除集合对象
        /// </summary>
        /// <param name="count"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual List<T> Pop(int count, params object[] args)
        {
            if (count <= 0) throw new ArgumentException($"{nameof(count)}({count}) is error!", nameof(count));
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rs = database.SetPop(key, count);
            List<T> list = rs?.Select(q => this.FromBytes<T>(q)).ToList();

            return list;
        }
        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual bool Delete(T value, params object[] args)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            string key = this.GetCacheKey(args);
            RedisValue v = this.ToBytes<T>(value);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var r = database.SetRemove(key, v);

            return r;
        }
        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual long Delete(List<T> list, params object[] args)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count == 0) return 0;
            list = list.Distinct().ToList();
            foreach (var m in list)
            {
                if (m == null) throw new ArgumentException($"{nameof(list)} Item is null!", nameof(list));
            }
            string key = this.GetCacheKey(args);
            int db = this.GetCacheDb(key);
            var database = this.redis.GetDatabase(db);
            var rvs = list.Select<T, RedisValue>(q => this.ToBytes(q)).ToArray();
            var r = database.SetRemove(key, rvs);

            return r;
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
            private SetCache<T> setCache;
            private IEnumerator<RedisValue> enumerator;

            public Enumerator(SetCache<T> setCache, IEnumerator<RedisValue> enumerator)
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
            private SetCache<T> setCache;
            private IEnumerable<RedisValue> values;

            public Enumerable(SetCache<T> setCache, IEnumerable<RedisValue> values)
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
