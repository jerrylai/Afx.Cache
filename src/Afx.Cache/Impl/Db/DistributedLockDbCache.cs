using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StackExchange.Redis;
using Afx.Cache.Impl.Base;

namespace Afx.Cache.Impl.Db
{
    /// <summary>
    /// 分布式锁db
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DistributedLockDbCache<T> : StringCache<T>, IDistributedLockDbCache<T>
    {
        /// <summary>
        /// 分布式锁db
        /// </summary>
        /// <param name="item"></param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public DistributedLockDbCache(string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
            : base("DistributedLockDb", item, redis, cacheKey, prefix) { }
    }
}
