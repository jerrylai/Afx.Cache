using System;
using System.Collections.Generic;
using System.Text;
using Afx.Cache.Interfaces;

namespace Afx.Cache
{
    /// <summary>
    /// 常规缓存数据db接口
    /// </summary>
    /// <typeparam name="T">缓存数据类型</typeparam>
    public interface IDataDbCache<T> : IStringCache<T>
    {
    }
}
