using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache
{
    /// <summary>
    /// 有序集合model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortSetModel<T>
    {
        /// <summary>
        /// 集合数据
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// 排序Score
        /// </summary>
        public double Score { get; set; }
    }
}
