using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StackExchange.Redis;
using Afx.Cache.Impl.Base;

namespace Afx.Cache.Impl.Db
{
    /// <summary>
    /// session数据db
    /// </summary>
    /// <typeparam name="T"></typeparam>
   public class SessionDbCache<T> : StringCache<T>, ISessionDbCache<T>
    {
        /// <summary>
        /// session数据db
        /// </summary>
        /// <param name="item"></param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public SessionDbCache(string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
            : base("SessionDb", item, redis, cacheKey, prefix) { }

    }
}
