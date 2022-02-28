using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Afx.Cache.Interfaces
{
    /// <summary>
    /// string key value 接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStringCache<T>: IRedisCache
    {
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<T> Get(params object[] args);

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="value">缓存数据</param>
        /// <param name="when">when</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<bool> Set(T value, OpWhen when = OpWhen.Always, params object[] args);

        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <param name="m">缓存数据</param>
        /// <param name="expireIn">缓存有效时间</param>
        /// <param name="when">when</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<bool> Set(T m, TimeSpan? expireIn, OpWhen when = OpWhen.Always, params object[] args);
        /// <summary>
        /// 原子增 T 必须是 int、 long
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> Increment(long value = 1, params object[] args);
        /// <summary>
        /// 原子减 T 必须是 int、 long
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> Decrement(long value = 1, params object[] args);
    }
}
