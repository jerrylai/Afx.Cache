using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Afx.Cache.Interfaces
{
    /// <summary>
    /// 链表接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILinkListCache<T> : IRedisCache
    {
        /// <summary>
        /// 添加到左边第一个
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> PushLeft(T value, params object[] args);
        /// <summary>
        /// 添加到左边第一个
        /// </summary>
        /// <param name="list">value List</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> PushLeft(List<T> list, params object[] args);
        /// <summary>
        /// 添加到右边第一个
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> PushRight(T value, params object[] args);
        /// <summary>
        /// 添加到右边第一个
        /// </summary>
        /// <param name="list">value List</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> PushRight(List<T> list, params object[] args);
        /// <summary>
        /// 获取指定索引位置数据
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<T> Get(long index, params object[] args);
        /// <summary>
        /// 获取一个范围数据
        /// </summary>
        /// <param name="start">开始位置</param>
        /// <param name="stop">结束位置，-1.全部</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<List<T>> GetRange(long start = 0, long stop = -1, params object[] args);
        /// <summary>
        /// 那个value后面
        /// </summary>
        /// <param name="pivot">要插入到那个value后面</param>
        /// <param name="value">插入value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> InsertAfter(T pivot, T value, params object[] args);
        /// <summary>
        /// 插入到那个value前面
        /// </summary>
        /// <param name="pivot">要插入到那个value前面</param>
        /// <param name="value">插入value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> InsertBefore(T pivot, T value, params object[] args);
        /// <summary>
        /// 返回并移除左边第一个
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<T> PopLeft(params object[] args);
        /// <summary>
        /// 返回并移除右边第一个
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<T> PopRight(params object[] args);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="index">位置</param>
        /// <param name="value">更新后value</param>
        /// <param name="args">缓存key参数</param>
        Task<bool> Update(long index, T value, params object[] args);

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="value">要删除的value</param>
        /// <param name="count">匹配数据个数，0.匹配所有</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> Delete(T value, long count = 0, params object[] args);

        /// <summary>
        /// 移除指定区域之外的所有数据
        /// </summary>
        /// <param name="start">开始位置</param>
        /// <param name="stop">结束位置</param>
        /// <param name="args">缓存key参数</param>
        Task Trim(long start, long stop, params object[] args);
        /// <summary>
        /// 获取链表长度
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> GetCount(params object[] args);
    }
}
