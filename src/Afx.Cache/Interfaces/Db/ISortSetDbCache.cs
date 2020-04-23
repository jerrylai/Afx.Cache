using System;
using System.Collections.Generic;
using System.Text;

using Afx.Cache.Interfaces;

namespace Afx.Cache
{
    /// <summary>
    /// 有序集合db接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISortSetDbCache<T> : ISortSetCache<T>
    {
    }
}
