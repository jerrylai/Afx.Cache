using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache
{
    /// <summary>
    /// 执行类型
    /// </summary>
    public enum ExcludeType
    {
        /// <summary>
        /// Both start and stop are inclusive
        /// </summary>
        None = 0,
        /// <summary>
        /// Start is exclusive, stop is inclusive
        /// </summary>
        Start = 1,
        /// <summary>
        /// Start is inclusive, stop is exclusive
        /// </summary>
        Stop = 2,
        /// <summary>
        /// Both start and stop are exclusive
        /// </summary>
        Both = 3
    }
}
