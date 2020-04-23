using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache.Interfaces
{
    /// <summary>
    /// gps位置接口
    /// </summary>
    public interface IGeoCache: IRedisCache
    {
        /// <summary>
        /// 添加位置
        /// </summary>
        /// <param name="name">位置名称</param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool AddOrUpdate(string name, double longitude, double latitude, params object[] args);

        /// <summary>
        /// 添加位置
        /// </summary>
        /// <param name="name">位置名称</param>
        /// <param name="pos"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool AddOrUpdate(string name, GeoPos pos, params object[] args);
        /// <summary>
        /// 添加位置
        /// </summary>
        /// <param name="m"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool AddOrUpdate(GeoInfo m, params object[] args);
        /// <summary>
        /// 添加位置
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        long AddOrUpdate(List<GeoInfo> list, params object[] args);
        /// <summary>
        /// 获取坐标
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        GeoPos Get(string name, params object[] args);
        /// <summary>
        /// 获取坐标
        /// </summary>
        /// <param name="names"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        List<GeoPos> Get(List<string> names, params object[] args);
        /// <summary>
        /// 计算距离
        /// </summary>
        /// <param name="firstName">坐标点名称</param>
        /// <param name="secondName">坐标点名称</param>
        /// <param name="unit">返回距离单位</param>
        /// <param name="args"></param>
        /// <returns></returns>
        double? GetDist(string firstName, string secondName, DistUnit unit = DistUnit.m, params object[] args);
        /// <summary>
        /// 获取GeoHash
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        string GetGeoHash(string name, params object[] args);
        /// <summary>
        /// 获取GeoHash
        /// </summary>
        /// <param name="names"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        List<string> GetGeoHash(List<string> names, params object[] args);
        /// <summary>
        /// 移除位置点
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        bool Delete(string name, params object[] args);
        /// <summary>
        /// 查询指定位置半径内的位置
        /// </summary>
        /// <param name="name">位置名称</param>
        /// <param name="radius">半径</param>
        /// <param name="unit">半径单位</param>
        /// <param name="count">返回数量</param>
        /// <param name="sort">排序，asc.由近到远</param>
        /// <param name="option">返回数据选项</param>
        /// <param name="args"></param>
        /// <returns></returns>
        List<GeoRadius> GetRadius(string name, double radius, DistUnit unit = DistUnit.m, int count = -1,
            Sort sort = Sort.Asc, RadiusOptions option = RadiusOptions.Default, params object[] args);

        /// <summary>
        /// 查询指定坐标半径内的位置
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="radius">半径</param>
        /// <param name="unit">半径单位</param>
        /// <param name="count">返回数量</param>
        /// <param name="sort">排序，asc.由近到远</param>
        /// <param name="option">返回数据选项</param>
        /// <param name="args"></param>
        /// <returns></returns>
        List<GeoRadius> GetRadius(double longitude, double latitude, double radius, DistUnit unit = DistUnit.m, int count = -1,
            Sort sort = Sort.Asc, RadiusOptions option = RadiusOptions.Default, params object[] args);
        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        long GetCount(params object[] args);
    }
}
