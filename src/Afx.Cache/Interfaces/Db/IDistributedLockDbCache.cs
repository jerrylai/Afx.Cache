using System;
using System.Collections.Generic;
using System.Text;
using Afx.Cache.Interfaces;

namespace Afx.Cache
{
    /// <summary>
    /// 分布式锁db接口
    /// </summary>
    /// <typeparam name="T">锁定类型</typeparam>
    public interface IDistributedLockDbCache<T> : IStringCache<T>
    {

    }
}
