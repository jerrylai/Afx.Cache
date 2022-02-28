using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Afx.Cache.Interfaces;
using System.Threading.Tasks;

namespace Afx.Cache.Impl.Base
{
    /// <summary>
    /// gps位置
    /// </summary>
    public class GeoCache : RedisCache, IGeoCache
    {
        /// <summary>
        /// gps位置
        /// </summary>
        /// <param name="node">缓存key配置db节点</param>
        /// <param name="item">缓存key配置项</param>
        /// <param name="redis"></param>
        /// <param name="cacheKey">ICacheKey</param>
        /// <param name="prefix">缓存前缀</param>
        public GeoCache(string node, string item, IConnectionMultiplexer redis, ICacheKey cacheKey, string prefix)
             : base(node, item, redis, cacheKey, prefix)
        {
        }
        /// <summary>
        /// 添加位置
        /// </summary>
        /// <param name="name">位置名称</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<bool> AddOrUpdate(string name, double longitude, double latitude, params object[] args)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (longitude > 180 || longitude < -180) throw new ArgumentException($"{nameof(longitude)}({longitude}) is error!", nameof(longitude));
            if (latitude > 90 || latitude < -90) throw new ArgumentException($"{nameof(latitude)}({latitude}) is error!", nameof(latitude));
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var r = await database.GeoAddAsync(cachekey, longitude, latitude, name);

            return r;
        }
        /// <summary>
        /// 添加位置
        /// </summary>
        /// <param name="name">位置名称</param>
        /// <param name="pos">位置</param>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<bool> AddOrUpdate(string name, GeoPos pos, params object[] args)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (pos == null) throw new ArgumentNullException(nameof(pos));

            return await this.AddOrUpdate(name, pos.Longitude, pos.Latitude, args);
        }
        /// <summary>
        /// 添加位置
        /// </summary>
        /// <param name="m">GeoInfo</param>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<bool> AddOrUpdate(GeoInfo m, params object[] args)
        {
            if (m == null) throw new ArgumentNullException(nameof(m));
            if (string.IsNullOrEmpty(m.Name)) throw new ArgumentNullException(nameof(m.Name));
            if (m.Position == null) throw new ArgumentNullException(nameof(m.Position));

            return await this.AddOrUpdate(m.Name, m.Position.Longitude, m.Position.Latitude, args);
        }
        /// <summary>
        /// 添加位置
        /// </summary>
        /// <param name="list">List GeoInfo</param>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<long> AddOrUpdate(List<GeoInfo> list, params object[] args)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count == 0) return 0;
            GeoEntry[] geos = new GeoEntry[list.Count];
            int i = 0;
            foreach (var m in list)
            {
                if (m == null) throw new ArgumentException($"item is null!", nameof(list));
                if (string.IsNullOrEmpty(m.Name)) throw new ArgumentException($"item.Name is null!", nameof(list));
                if (m.Position == null) throw new ArgumentException($"item.Position is null!", nameof(list));
                geos[i++] = new GeoEntry(m.Position.Longitude, m.Position.Latitude, m.Name);
            }
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var r = await database.GeoAddAsync(cachekey, geos);

            return r;
        }
        /// <summary>
        /// 获取坐标
        /// </summary>
        /// <param name="name">位置名称</param>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<GeoPos> Get(string name, params object[] args)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var pos = await database.GeoPositionAsync(cachekey, name);
            GeoPos m = null;
            if(pos.HasValue)
            {
                m = new GeoPos()
                {
                    Latitude = pos.Value.Latitude,
                    Longitude = pos.Value.Longitude
                };
            }

            return m;
        }
        /// <summary>
        /// 获取坐标
        /// </summary>
        /// <param name="names">位置名称</param>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<List<GeoPos>> Get(List<string> names, params object[] args)
        {
            if (names == null) throw new ArgumentNullException(nameof(names));
            if (names.Count == 0) return new List<GeoPos>(0);
            RedisValue[] members = new RedisValue[names.Count];
            int i = 0;
            foreach (var n in names)
            {
                if (string.IsNullOrEmpty(n)) throw new ArgumentException($"{nameof(names)} item is null", nameof(names));
                members[i++] = n;
            }
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var pos = await database.GeoPositionAsync(cachekey, members);
            List<GeoPos> list = pos?.Select(q => !q.HasValue ? null : new GeoPos
            {
                Latitude = q.Value.Latitude,
                Longitude = q.Value.Longitude
            }).ToList();

            return list;
        }
        /// <summary>
        /// 计算距离
        /// </summary>
        /// <param name="firstName">坐标点名称</param>
        /// <param name="secondName">坐标点名称</param>
        /// <param name="unit">返回距离单位</param>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<double?> GetDist(string firstName, string secondName, DistUnit unit = DistUnit.m, params object[] args)
        {
            if (string.IsNullOrEmpty(firstName)) throw new ArgumentNullException(nameof(firstName));
            if (string.IsNullOrEmpty(secondName)) throw new ArgumentNullException(nameof(secondName));
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var dist = await database.GeoDistanceAsync(cachekey, firstName, secondName, (GeoUnit)((int)unit));

            return dist;
        }
        /// <summary>
        /// 获取GeoHash
        /// </summary>
        /// <param name="name">位置名称</param>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<string> GetGeoHash(string name, params object[] args)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var hash = await database.GeoHashAsync(cachekey, name);

            return hash;
        }

        /// <summary>
        /// 获取GeoHash
        /// </summary>
        /// <param name="names">位置名称</param>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<List<string>> GetGeoHash(List<string> names, params object[] args)
        {
            if (names == null) throw new ArgumentNullException(nameof(names));
            if (names.Count == 0) return new List<string>(0);
            RedisValue[] members = new RedisValue[names.Count];
            int i = 0;
            foreach (var n in names)
            {
                if (string.IsNullOrEmpty(n)) throw new ArgumentException($"{nameof(names)} item is null", nameof(names));
                members[i++] = n;
            }
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var hashs = await database.GeoHashAsync(cachekey, members);
            List<string> list = hashs?.ToList();

            return list;
        }
        /// <summary>
        /// 查询指定位置半径内的位置
        /// </summary>
        /// <param name="name">位置名称</param>
        /// <param name="radius">半径</param>
        /// <param name="unit">半径单位</param>
        /// <param name="count">返回数量</param>
        /// <param name="sort">排序，asc.由近到远</param>
        /// <param name="option">返回数据选项</param>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<List<GeoRadius>> GetRadius(string name, double radius, DistUnit unit = DistUnit.m, int count = -1,
            Sort sort = Sort.Asc, RadiusOptions option = RadiusOptions.Default, params object[] args)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if(radius < 0) throw new ArgumentException($"{nameof(radius)}({radius}) is error!", nameof(radius));
            if (count < -1) throw new ArgumentException($"{nameof(count)}({count}) is error!", nameof(count));
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = await database.GeoRadiusAsync(cachekey, name, radius, (GeoUnit)((int)unit), count, (Order)((int)sort), (GeoRadiusOptions)((int)option));
            List<GeoRadius> list = rs?.Select(q => new GeoRadius
            {
                Name = q.Member,
                Distance = q.Distance,
                Hash = q.Hash,
                Position = !q.Position.HasValue
                            ? null
                            : new GeoPos { Latitude = q.Position.Value.Latitude, Longitude = q.Position.Value.Longitude }
            }).ToList();

            return list;
        }
        /// <summary>
        /// 查询指定坐标半径内的位置
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">半径</param>
        /// <param name="unit">半径单位</param>
        /// <param name="count">返回数量</param>
        /// <param name="sort">排序，asc.由近到远</param>
        /// <param name="option">返回数据选项</param>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<List<GeoRadius>> GetRadius(double longitude, double latitude, double radius, DistUnit unit = DistUnit.m, int count = -1,
            Sort sort = Sort.Asc, RadiusOptions option = RadiusOptions.Default, params object[] args)
        {
            if (longitude > 180 || longitude < -180) throw new ArgumentException($"{nameof(longitude)}({longitude}) is error!", nameof(longitude));
            if (latitude > 90 || latitude < -90) throw new ArgumentException($"{nameof(latitude)}({latitude}) is error!", nameof(latitude));
            if (radius < 0) throw new ArgumentException($"{nameof(radius)}({radius}) is error!", nameof(radius));
            if (count < -1) throw new ArgumentException($"{nameof(count)}({count}) is error!", nameof(count));
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var rs = await database.GeoRadiusAsync(cachekey, longitude, latitude, radius, (GeoUnit)((int)unit), count, (Order)((int)sort), (GeoRadiusOptions)((int)option));
            List<GeoRadius> list = rs?.Select(q => new GeoRadius
            {
                Name = q.Member,
                Distance = q.Distance,
                Hash = q.Hash,
                Position = q.Position.HasValue
                            ? new GeoPos
                            {
                                Latitude = q.Position.Value.Latitude,
                                Longitude = q.Position.Value.Longitude
                            }
                            : null
            }).ToList();

            return list;
        }
        /// <summary>
        /// 移除位置点
        /// </summary>
        /// <param name="name">位置名称</param>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<bool> Delete(string name, params object[] args)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            var r = await database.GeoRemoveAsync(cachekey, name);

            return r;
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="args">key 参数</param>
        /// <returns></returns>
        public virtual async Task<long> GetCount(params object[] args)
        {
            string cachekey = this.GetCacheKey(args);
            int db = this.GetCacheDb(cachekey);
            var database = this.redis.GetDatabase(db);
            return await database.SortedSetLengthAsync(cachekey);
        }
    }
}
