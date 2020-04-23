using System;
using System.Collections.Generic;
using System.Text;

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
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long PushLeft(T value, params object[] args);
        /// <summary>
        /// 添加到左边第一个
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long PushLeft(List<T> list, params object[] args);
        /// <summary>
        /// 添加到右边第一个
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long PushRight(T value, params object[] args);
        /// <summary>
        /// 添加到右边第一个
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long PushRight(List<T> list, params object[] args);
        /// <summary>
        /// 获取指定索引位置数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        T Get(long index, params object[] args);
        /// <summary>
        /// 获取一个范围数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        List<T> GetRange(long start = 0, long stop = -1, params object[] args);
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long InsertAfter(T pivot, T value, params object[] args);
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long InsertBefore(T pivot, T value, params object[] args);
        /// <summary>
        /// 返回并移除左边第一个
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        T PopLeft(params object[] args);
        /// <summary>
        /// 返回并移除右边第一个
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        T PopRight(params object[] args);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        bool Update(long index, T value, params object[] args);

        /// <summary>
        /// 移除数据，count=0：移除所有匹配数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long Delete(T value, long count = 0, params object[] args);

        /// <summary>
        /// 移除指定区域之外的所有数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="args"></param>
        void Trim(long start, long stop, params object[] args);
        /// <summary>
        /// 获取链表长度
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        long GetCount(params object[] args);
    }
}
