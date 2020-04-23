using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache
{
    /// <summary>
    /// 位置半径信息
    /// </summary>
    public class GeoRadius
    {
        /// <summary>
        /// 位置点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public double? Distance { get; set; }
        /// <summary>
        /// GeoHash
        /// </summary>
        public long? Hash { get; set; }
        /// <summary>
        /// gps坐标
        /// </summary>
        public GeoPos Position { get; set; }
    }
}
