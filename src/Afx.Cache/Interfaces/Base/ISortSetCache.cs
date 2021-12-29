using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache.Interfaces
{
    /// <summary>
    /// 有序集合接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISortSetCache<T> : IRedisCache
    {
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="score">排序分</param>
        /// <param name="when">操作类型</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        bool AddOrUpdate(T value, double score, OpWhen when = OpWhen.Always, params object[] args);
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="m">SortSetModel</param>
        /// <param name="when">操作类型</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        bool AddOrUpdate(SortSetModel<T> m, OpWhen when = OpWhen.Always, params object[] args);
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="list">SortSetModel List</param>
        /// <param name="when">操作类型</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        long AddOrUpdate(List<SortSetModel<T>> list, OpWhen when = OpWhen.Always, params object[] args);

        /// <summary>
        /// 减少 score
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="score">排序分</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        double Decrement(T value, double score, params object[] args);
        /// <summary>
        /// 增加 score
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="score">排序分</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        double Increment(T value, double score, params object[] args);

        /// <summary>
        /// 获取集合数量
        /// </summary>
        /// <param name="minScore">最小排序分</param>
        /// <param name="maxScore">最大排序分</param>
        /// <param name="excType">条件类型</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        long GetCount(double minScore = double.NegativeInfinity, double maxScore = double.PositiveInfinity, ExcludeType excType= ExcludeType.None, params object[] args);
        /// <summary>
        /// 返回并集合
        /// </summary>
        /// <param name="sort">排序</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        SortSetModel<T> Pop(Sort sort = Sort.Asc, params object[] args);
        /// <summary>
        /// 返回并集合
        /// </summary>
        /// <param name="count">返回数量</param>
        /// <param name="sort">排序</</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        List<SortSetModel<T>> Pop(long count, Sort sort = Sort.Asc, params object[] args);
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="start">开始位置</param>
        /// <param name="stop">结束位置</param>
        /// <param name="sort">排序</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        List<T> Get(long start = 0, long stop = -1, Sort sort = Sort.Asc, params object[] args);
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="start">开始位置</param>
        /// <param name="stop">结束位置</param>
        /// <param name="sort">排序</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        List<SortSetModel<T>> GetWithScores(long start = 0, long stop = -1, Sort sort = Sort.Asc, params object[] args);
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="startScore">开始位置排序分</param>
        /// <param name="stopScore">结束位置排序分</param>
        /// <param name="excType">条件类型</param>
        /// <param name="sort">排序</param>
        /// <param name="skip">跳过多少个</param>
        /// <param name="take">返回多少个</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        List<T> GetByScore(double startScore = double.NegativeInfinity, double stopScore = double.PositiveInfinity,
            ExcludeType excType = ExcludeType.None, Sort sort = Sort.Asc, long skip = 0, long take = -1, params object[] args);
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <param name="startScore">开始位置排序分</param>
        /// <param name="stopScore">结束位置排序分</param>
        /// <param name="excType">条件类型</param>
        /// <param name="sort">排序</param>
        /// <param name="skip">跳过多少个</param>
        /// <param name="take">返回多少个</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        List<SortSetModel<T>> GetByScoreWithScores(double startScore = double.NegativeInfinity, double stopScore = double.PositiveInfinity,
            ExcludeType excType = ExcludeType.None, Sort sort = Sort.Asc, long skip = 0, long take = -1, params object[] args);
        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        bool Delete(T value, params object[] args);
        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="list">value List</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        long Delete(List<T> list, params object[] args);
        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="start">开始位置</param>
        /// <param name="stop">结束位置</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        long Delete(long start, long stop, params object[] args);
        /// <summary>
        /// 移除集合
        /// </summary>
        /// <param name="startScore">开始位置排序分</param>
        /// <param name="stopScore">结束位置排序分</param>
        /// <param name="excType">条件类型</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        long DeleteByScore(double startScore, double stopScore, ExcludeType excType = ExcludeType.None, params object[] args);

        /// <summary>
        /// 游标方式读取数据
        /// </summary>
        /// <param name="pattern">搜索表达式</param>
        /// <param name="start">开始位置</param>
        /// <param name="pageSize">游标页大小</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        IEnumerable<T> Scan(string pattern, int start, int pageSize, params object[] args);
    }
}
