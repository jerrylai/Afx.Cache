using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using Afx.Cache.Impl.Base;

namespace Afx.Cache.Impl.Db
{
    /// <summary>
    /// 哈希db
    /// </summary>
    /// <typeparam name="TField"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class HashDbCache<TField, TValue> : HashCache<TField, TValue>, IHashDbCache<TField, TValue>
    {
        /// <summary>
        /// 哈希db
        /// </summary>
        /// <param name="item"></param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public HashDbCache(string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
            : base("HashDb", item, redis, cacheKey, prefix) { }
    }
}
