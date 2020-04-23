using System;
using System.Collections.Generic;
using System.Text;
using Afx.Cache.Interfaces;

namespace Afx.Cache
{
    /// <summary>
    /// session数据db接口
    /// </summary>
    /// <typeparam name="T">session数据类型</typeparam>
    public interface ISessionDbCache<T>: IStringCache<T>
    {
    }
}
