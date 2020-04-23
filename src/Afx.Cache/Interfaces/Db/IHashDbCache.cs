using System;
using System.Collections.Generic;
using System.Text;

using Afx.Cache.Interfaces;

namespace Afx.Cache
{
    /// <summary>
    /// 哈希db接口
    /// </summary>
    /// <typeparam name="TField">哈希列类型</typeparam>
    /// <typeparam name="TValue">哈希值类型</typeparam>
    public interface IHashDbCache<TField, TValue>: IHashCache<TField, TValue>
    {
    }
}
