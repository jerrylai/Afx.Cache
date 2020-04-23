using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OpWhen
    {
        /// <summary>
        /// The operation should occur whether or not there is an existing value
        /// </summary>
        Always = 0,
        /// <summary>
        /// The operation should only occur when there is an existing value
        /// </summary>
        Exists = 1,
        /// <summary>
        /// The operation should only occur when there is not an existing value
        /// </summary>
        NotExists = 2
    }
}
