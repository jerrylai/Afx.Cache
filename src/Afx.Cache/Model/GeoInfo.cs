using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache
{
    /// <summary>
    /// 位置点坐标
    /// </summary>
    public class GeoInfo
    {
        /// <summary>
        /// 位置点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// gps坐标
        /// </summary>
        public GeoPos Position { get; set; }
    }
}
