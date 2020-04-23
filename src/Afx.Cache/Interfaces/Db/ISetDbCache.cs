using System;
using System.Collections.Generic;
using System.Text;

using Afx.Cache.Interfaces;

namespace Afx.Cache
{
    /// <summary>
    /// 集合db接口
    /// </summary>
    /// <typeparam name="T">集合数据类型</typeparam>
    public interface ISetDbCache<T>: ISetCache<T>
    {
    }
}
