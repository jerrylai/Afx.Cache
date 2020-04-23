using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using Afx.Cache.Impl.Base;

namespace Afx.Cache.Impl.Db
{
    /// <summary>
    /// gps位置信息db
    /// </summary>
    public class GeoDbCache : GeoCache, IGeoDbCache
    {
        /// <summary>
        /// gps位置信息db
        /// </summary>
        /// <param name="item"></param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public GeoDbCache(string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
            : base("GeoDb", item, redis, cacheKey, prefix) { }
    }
}
