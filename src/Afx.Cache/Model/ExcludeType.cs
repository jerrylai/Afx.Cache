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
        /// Both start and stop are inclusive, start &lt;= value &lt;= stop
        /// </summary>
        None = 0,
        /// <summary>
        /// Start is exclusive, stop is inclusive, start &lt; value &lt;= stop
        /// </summary>
        Start = 1,
        /// <summary>
        /// Start is inclusive, stop is exclusive, start &lt;= value &lt; stop
        /// </summary>
        Stop = 2,
        /// <summary>
        /// Both start and stop are exclusive, start &lt; value &lt; stop
        /// </summary>
        Both = 3
    }
}
