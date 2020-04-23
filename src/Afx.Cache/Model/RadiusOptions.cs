using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache
{
    /// <summary>
    /// 搜索最近坐标返回数据选项
    /// </summary>
    public enum RadiusOptions
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// 返回结果会带上匹配位置的经纬度
        /// </summary>
        WithCoordinates = 1,
        /// <summary>
        /// 返回结果会带上匹配位置与给定地理位置的距离
        /// </summary>
        WithDistance = 2,
        /// <summary>
        /// Default
        /// </summary>
        Default = 3,
        /// <summary>
        /// 则返回结果会带上匹配位置的hash值
        /// </summary>
        WithGeoHash = 4
    }
}
