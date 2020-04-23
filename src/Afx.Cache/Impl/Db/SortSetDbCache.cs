using System;
using System.Collections.Generic;
using System.Text;

using Afx.Cache.Impl.Base;

namespace Afx.Cache.Impl.Db
{
    /// <summary>
    /// 有序集合db
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortSetDbCache<T> : SortSetCache<T>, ISortSetDbCache<T>
    {
        /// <summary>
        /// 有序集合db
        /// </summary>
        /// <param name="item"></param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public SortSetDbCache(string item, StackExchange.Redis.IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
            : base("SortSetDb", item, redis, cacheKey, prefix) { }
    }
}
