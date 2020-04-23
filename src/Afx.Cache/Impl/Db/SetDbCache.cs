using System;
using System.Collections.Generic;
using System.Text;

using StackExchange.Redis;
using Afx.Cache.Impl.Base;

namespace Afx.Cache.Impl.Db
{
    /// <summary>
    /// set集合db
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SetDbCache<T>: SetCache<T>, ISetDbCache<T>
    {
        /// <summary>
        /// set集合db
        /// </summary>
        /// <param name="item"></param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public SetDbCache(string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
            : base("SetDb", item, redis, cacheKey, prefix) { }
    }
}
