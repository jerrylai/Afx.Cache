using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache.Interfaces
{
    /// <summary>
    /// hash 接口
    /// </summary>
    /// <typeparam name="TField">hash key数据类型</typeparam>
    /// <typeparam name="TValue">hash value数据类型</typeparam>
    public interface IHashCache<TField, TValue> : IRedisCache
    {
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="field">hash key</param>
        /// <param name="value">hash value</param>
        /// <param name="when">操作类型</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        bool Set(TField field, TValue value, OpWhen when = OpWhen.Always, params object[] args);
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="args">缓存key参数</param>
        void AddOrUpdate(Dictionary<TField, TValue> dic, params object[] args);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        Dictionary<TField, TValue> Get(params object[] args);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="field">hash key</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        TValue GetValue(TField field, params object[] args);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keys">hash key</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        List<TValue> GetValue(List<TField> keys, params object[] args);
        /// <summary>
        /// 获取hash key
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        List<TField> GeTFields(params object[] args);
        /// <summary>
        /// 获取hash value
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        List<TValue> GetValues(params object[] args);
        /// <summary>
        /// 获取hash key 数量
        /// </summary>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        long GetCount(params object[] args);
        /// <summary>
        /// 是否存在hash key
        /// </summary>
        /// <param name="key">hash key</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        bool Exists(TField field, params object[] args);
        /// <summary>
        /// 移除hash key
        /// </summary>
        /// <param name="field">hash key</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        bool Delete(TField field, params object[] args);
        /// <summary>
        /// 移除hash key
        /// </summary>
        /// <param name="fields">hash key</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        long Delete(List<TField> fields, params object[] args);
        /// <summary>
        /// hash value 原子自增，TValue 必须是 long、int类型
        /// </summary>
        /// <param name="field">hash key</param>
        /// <param name="value">hash value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        long Increment(TField field, long value = 1, params object[] args);
        /// <summary>
        /// hash value 原子自减，TValue 必须是 long、int类型
        /// </summary>
        /// <param name="field">hash key</param>
        /// <param name="value">hash value</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        long Decrement(TField field, long value = 1, params object[] args);


        /// <summary>
        /// 游标方式读取数据
        /// </summary>
        /// <param name="pattern">搜索表达式</param>
        /// <param name="start">开始位置</param>
        /// <param name="pageSize">游标页大小</param>
        /// <param name="args">缓存key参数</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<TField, TValue>> Scan(string pattern, int start, int pageSize, params object[] args);
    }
}
