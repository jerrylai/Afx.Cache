using System;
using System.Collections.Generic;

namespace Afx.Cache
{
    /// <summary>
    /// 缓存key配置接口
    /// </summary>
    public interface ICacheKey
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        CacheKeyConfig Get(string node, string item);

        /// <summary>
        /// 获取key
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="item">名称</param>
        /// <returns></returns>
        string GetKey(string node, string item);

        /// <summary>
        /// 获取过期时间
        /// </summary>
        /// <param name="node"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        TimeSpan? GetExpire(string node, string item);

        /// <summary>
        /// 获取db
        /// </summary>
        /// <param name="node"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        List<int> GetDb(string node, string item);
    }
}
