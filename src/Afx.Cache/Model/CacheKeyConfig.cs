using System;
using System.Collections.Generic;
using System.Text;

namespace Afx.Cache
{
    /// <summary>
    /// 缓存key配置
    /// </summary>
    public class CacheKeyConfig
    {
        /// <summary>
        /// db节点名称
        /// </summary>
        public string Node { get; private set; }
        /// <summary>
        /// 配置Item名称
        /// </summary>
        public string Item { get; private set; }
        /// <summary>
        /// 配置key
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan? Expire { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public List<int> Db { get { return db?.FindAll(q => true); } }

        private List<int> db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="item"></param>
        /// <param name="key"></param>
        /// <param name="expire"></param>
        /// <param name="db"></param>
        public CacheKeyConfig(string node, string item, string key, TimeSpan? expire, List<int> db)
        {
            this.Node = node;
            this.Item = item;
            this.Key = key;
            this.Expire = expire;
            this.db = db?.FindAll(q => true) ?? new List<int>(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CacheKeyConfig Copy()
        {
            return new CacheKeyConfig(this.Node, this.Item, this.Key, this.Expire, this.Db);
        }
    }
}
