using System;
using System.Collections.Generic;
using System.Text;

using Afx.Cache.Interfaces;

namespace Afx.Cache
{
    /// <summary>
    /// 常规带参数数据db接口
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public interface IParamDbCache<T> : IStringCache<T>
    {
    }
}
