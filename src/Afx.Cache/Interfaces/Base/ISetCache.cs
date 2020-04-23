using System;
using System.Collections.Generic;
using System.Text;

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
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool Add(T value, params object[] args);
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long Add(List<T> list, params object[] args);
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        List<T> Get(params object[] args);
        /// <summary>
        /// 两个集合运算，返回运算结果
        /// </summary>
        /// <param name="firstArgs">集合1参数</param>
        /// <param name="secondArgs">集合2参数</param>
        /// <param name="op"></param>
        /// <returns></returns>
        List<T> Join(object[] firstArgs, object[] secondArgs, SetOp op);
        /// <summary>
        /// 两个集合运算，并将运算结果存储到新集合
        /// </summary>
        /// <param name="addArgs">新集合</param>
        /// <param name="firstArgs">集合1参数</param>
        /// <param name="secondArgs">集合2参数</param>
        /// <param name="op"></param>
        /// <returns></returns>
        long JoinAndAdd(object[] addArgs, object[] firstArgs, object[] secondArgs, SetOp op);
        /// <summary>
        /// 是否存在集合
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool Exist(T value, params object[] args);
        /// <summary>
        /// 集合数量
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        long GetCount(params object[] args);
        /// <summary>
        /// 移动一个已存在对象到新集合
        /// </summary>
        /// <param name="sourceArgs">原来集合参数</param>
        /// <param name="desArgs">需要移到新集合参数</param>
        /// <param name="value">移动对象</param>
        /// <returns></returns>
        bool Move(object[] sourceArgs, object[] desArgs, T value);
        /// <summary>
        /// 返回并移除一个集合对象
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        T Pop(params object[] args);
        /// <summary>
        /// 返回并移除集合对象
        /// </summary>
        /// <param name="count"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        List<T> Pop(int count, params object[] args);
        /// <summary>
        /// 随机返回一个对象
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        T GetRandomValue(params object[] args);
        /// <summary>
        /// 随机返回对象
        /// </summary>
        /// <param name="count"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        List<T> GetRandomValue(int count, params object[] args);
        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool Delete(T value, params object[] args);
        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long Delete(List<T> list, params object[] args);

        /// <summary>
        /// 游标方式读取数据
        /// </summary>
        /// <param name="pattern">搜索表达式</param>
        /// <param name="start">开始位置</param>
        /// <param name="pageSize">游标页大小</param>
        /// <param name="args"></param>
        /// <returns></returns>
        IEnumerable<T> Scan(T pattern, int start, int pageSize, params object[] args);
    }
}
