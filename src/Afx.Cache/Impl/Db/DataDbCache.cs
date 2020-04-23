using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StackExchange.Redis;
using Afx.Cache.Impl.Base;

namespace Afx.Cache.Impl.Db
{
    /// <summary>
    /// 常规缓存数据db
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataDbCache<T> : StringCache<T>, IDataDbCache<T>
    {
        /// <summary>
        /// 常规缓存数据db
        /// </summary>
        /// <param name="item"></param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public DataDbCache(string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
            : base("DataDb", item, redis, cacheKey, prefix) { }
    }
}
