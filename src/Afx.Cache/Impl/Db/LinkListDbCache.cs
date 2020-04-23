using System;
using System.Collections.Generic;
using System.Text;
using Afx.Cache.Impl.Base;

namespace Afx.Cache.Impl.Db
{
    /// <summary>
    /// 链接db
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkListDbCache<T> : LinkListCache<T>, ILinkListDbCache<T>
    {
        /// <summary>
        /// 链接db
        /// </summary>
        /// <param name="item"></param>
        /// <param name="redis"></param>
        /// <param name="cacheKey"></param>
        /// <param name="prefix"></param>
        public LinkListDbCache(string item, StackExchange.Redis.IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
            : base("LinkListDb", item, redis, cacheKey, prefix) { }
    }
}
