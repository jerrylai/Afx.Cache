using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Afx.Cache.Interfaces
{
    /// <summary>
    /// set集合接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISetCache<T> : IRedisCache
    {
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<bool> Add(T value, params object[] args);
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="list">value list</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> Add(List<T> list, params object[] args);
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<List<T>> Get(params object[] args);
        /// <summary>
        /// 两个集合运算，返回运算结果
        /// </summary>
        /// <param name="firstArgs">第一个集合缓存key参数</param>
        /// <param name="secondArgs">第二集合缓存key参数</param>
        /// <param name="op">操作</param>
        /// <returns></returns>
        Task<List<T>> Join(object[] firstArgs, object[] secondArgs, SetOp op);
        /// <summary>
        /// 两个集合运算，并将运算结果存储到新集合
        /// </summary>
        /// <param name="addArgs">新集合缓存key参数</param>
        /// <param name="firstArgs">第一个集合缓存key参数</param>
        /// <param name="secondArgs">第二集合缓存key参数</param>
        /// <param name="op">操作</param>
        /// <returns></returns>
       Task<long> JoinAndAdd(object[] addArgs, object[] firstArgs, object[] secondArgs, SetOp op);
        /// <summary>
        /// 是否存在集合
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<bool> Exist(T value, params object[] args);
        /// <summary>
        /// 集合数量
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> GetCount(params object[] args);
        /// <summary>
        /// 移动一个已存在对象到新集合
        /// </summary>
        /// <param name="sourceArgs">源集合缓存key参数</param>
        /// <param name="desArgs">需要移到新集合缓存key参数</param>
        /// <param name="value">移动对象</param>
        /// <returns></returns>
        Task<bool> Move(object[] sourceArgs, object[] desArgs, T value);
        /// <summary>
        /// 返回并移除一个集合对象
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<T> Pop(params object[] args);
        /// <summary>
        /// 返回并移除集合对象
        /// </summary>
        /// <param name="count"></param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<List<T>> Pop(int count, params object[] args);
        /// <summary>
        /// 随机返回一个对象
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<T> GetRandomValue(params object[] args);
        /// <summary>
        /// 随机返回对象
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<List<T>> GetRandomValue(int count, params object[] args);
        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<bool> Delete(T value, params object[] args);
        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="list">value list</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Task<long> Delete(List<T> list, params object[] args);

        /// <summary>
        /// 游标方式读取数据
        /// </summary>
        /// <param name="pattern">搜索表达式</param>
        /// <param name="start">开始位置</param>
        /// <param name="pageSize">游标页大小</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        IAsyncEnumerable<T> Scan(string pattern, int start, int pageSize, params object[] args);
    }
}
