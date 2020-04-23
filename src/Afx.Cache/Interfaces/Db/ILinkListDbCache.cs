using System;
using System.Collections.Generic;
using System.Text;
using Afx.Cache.Interfaces;

namespace Afx.Cache
{
    /// <summary>
    /// 链接db接口
    /// </summary>
    /// <typeparam name="T">链接表数据类型</typeparam>
    public interface ILinkListDbCache<T> : ILinkListCache<T>
    {
    }
}
