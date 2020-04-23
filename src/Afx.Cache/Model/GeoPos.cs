using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache
{
    /// <summary>
    /// gps坐标
    /// </summary>
    public class GeoPos
    {
        private double longitude;
        private double latitude;

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude
        {
            get { return this.longitude; }
            set
            {
                if (value > 180 || value < -180) throw new ArgumentException($"{nameof(value)}({value}) is error!", nameof(value));
                this.longitude = value;
            }
        }


        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude
        {
            get { return this.latitude; }
            set
            {
                if (value > 90 || value < -90) throw new ArgumentException($"{nameof(value)}({value}) is error!", nameof(value));
                this.latitude = value;
            }
        }
    }
}
