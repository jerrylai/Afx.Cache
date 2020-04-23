using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache.Interfaces
{
    /// <summary>
    /// hash 接口
    /// </summary>
    /// <typeparam name="TKey">hash key数据类型</typeparam>
    /// <typeparam name="TValue">hash value数据类型</typeparam>
    public interface IHashCache<TKey, TValue> : IRedisCache
    {
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="when"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool Set(TKey key, TValue value, OpWhen when = OpWhen.Always, params object[] args);
        /// <summary>
        /// 添加或更新数据
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="args"></param>
        void AddOrUpdate(Dictionary<TKey, TValue> dic, params object[] args);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Dictionary<TKey, TValue> Get(params object[] args);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        TValue GetValue(TKey key, params object[] args);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        List<TValue> GetValue(List<TKey> keys, params object[] args);
        /// <summary>
        /// 获取hash key
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        List<TKey> GetKeys(params object[] args);
        /// <summary>
        /// 获取hash value
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        List<TValue> GetValues(params object[] args);
        /// <summary>
        /// 获取hash key 数量
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        long GetCount(params object[] args);
        /// <summary>
        /// 是否存在hash key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool Exists(TKey key, params object[] args);
        /// <summary>
        /// 移除hash key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool Delete(TKey key, params object[] args);
        /// <summary>
        /// 移除hash key
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long Delete(List<TKey> keys, params object[] args);
        /// <summary>
        /// hash value 原子自增，TValue 必须是 long、int类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long Increment(TKey key, long value = 1, params object[] args);
        /// <summary>
        /// hash value 原子自减，TValue 必须是 long、int类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long Decrement(TKey key, long value = 1, params object[] args);


        /// <summary>
        /// 游标方式读取数据
        /// </summary>
        /// <param name="pattern">搜索表达式</param>
        /// <param name="start">开始位置</param>
        /// <param name="pageSize">游标页大小</param>
        /// <param name="args"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<TKey, TValue>> Scan(TKey pattern, int start, int pageSize, params object[] args);
    }
}
