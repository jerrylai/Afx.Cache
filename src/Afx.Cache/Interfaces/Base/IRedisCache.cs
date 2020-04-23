using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache.Interfaces
{
    /// <summary>
    /// redis 缓存
    /// </summary>
    public interface IRedisCache : IBaseCache
    {
        /// <summary>
        /// 缓存key配置
        /// </summary>
        CacheKeyModel KeyConfig { get; }

        /// <summary>
        /// 获取缓存key
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        string GetCacheKey(params object[] args);

        /// <summary>
        /// 获取key所在db
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int GetCacheDb(string key);

        /// <summary>
        /// 缓存key是否存在
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        bool Contains(params object[] args);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        bool Remove(params object[] args);

        /// <summary>
        /// 设置缓存有效时间
        /// </summary>
        /// <param name="expireIn">缓存有效时间</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        bool Expire(TimeSpan? expireIn, params object[] args);

        /// <summary>
        /// 根据系统配置设置缓存有效时间
        /// </summary>
        /// <param name="args"></param>
        /// <returns>缓存key参数</returns>
        bool Expire(params object[] args);
    }
}
