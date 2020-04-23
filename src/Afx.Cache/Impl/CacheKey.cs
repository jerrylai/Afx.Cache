using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Afx.Cache.Impl
{
    /// <summary>
    /// 缓存key配置
    /// </summary>
    public class CacheKey : ICacheKey
    {
        private List<CacheKeyModel> list;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="xmlFile">配置文件</param>
        public CacheKey(string xmlFile)
        {
            this.Load(xmlFile);
        }

        private void Load(string xmlFile)
        {
            if (string.IsNullOrEmpty(xmlFile)) throw new ArgumentNullException(xmlFile);
            if (!System.IO.File.Exists(xmlFile)) throw new FileNotFoundException(xmlFile + " not found!", xmlFile);
            var xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            using (var fs = System.IO.File.Open(xmlFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings() { IgnoreComments = true, XmlResolver = null };
                using (var rd = XmlReader.Create(fs, xmlReaderSettings))
                {
                    xmlDoc.Load(rd);

                    var rootElement = xmlDoc.DocumentElement;
                    if (rootElement == null) throw new ArgumentException(xmlFile + " is error!");
                    this.list = new List<CacheKeyModel>();
                    foreach (XmlNode n in rootElement.ChildNodes)
                    {
                        if (n is XmlElement)
                        {
                            var node = n as XmlElement;
                            TimeSpan? node_expire = null;
                            List<int> node_db = this.GetDbList(node.GetAttribute("db"));
                            TimeSpan ts;
                            var s = node.GetAttribute("expire");
                            if (!string.IsNullOrEmpty(s))
                            {
                                int count = s.Count(q => q == ':');
                                while(count < 3)
                                {
                                    s = $"0:{s}";
                                    count++;
                                }
                                if (TimeSpan.TryParse(s, out ts) && ts.TotalSeconds > 0)
                                    node_expire = ts;
                            }
                            foreach (XmlNode cn in node.ChildNodes)
                            {
                                if (cn is XmlElement)
                                {
                                    var el = cn as XmlElement;
                                    var key = el.GetAttribute("key");
                                    var db = this.GetDbList(el.GetAttribute("db")) ?? node_db ?? new List<int>(0);
                                    TimeSpan? expire = node_expire;
                                    s = el.GetAttribute("expire");
                                    if (!string.IsNullOrEmpty(s))
                                    {
                                        int count = s.Count(q => q == ':');
                                        while (count < 3)
                                        {
                                            s = $"0:{s}";
                                            count++;
                                        }
                                        if (TimeSpan.TryParse(s, out ts) && ts.TotalSeconds > 0)
                                            expire = ts;
                                    }
                                    
                                    this.list.Add(new CacheKeyModel(node.Name, el.Name, key, expire, db));
                                }
                            }
                        }
                    }
                    this.list.TrimExcess();
                }
            }
        }

        private List<int> GetDbList(string val)
        {
            List<int> list = null;
            if (!string.IsNullOrEmpty(val))
            {
                list = new List<int>();
                string[] arr = val.Split(',');
                foreach (var ts in arr)
                {
                    var ss = ts.Trim();
                    if (!string.IsNullOrEmpty(ss))
                    {
                        if (ss.Contains("-"))
                        {
                            var ssarr = ss.Split('-');
                            if (ssarr.Length == 2)
                            {
                                var bs = ssarr[0].Trim();
                                var es = ssarr[1].Trim();
                                int bv = 0;
                                int ev = 0;
                                if (int.TryParse(bs, out bv) && int.TryParse(es, out ev) && bv <= ev)
                                {
                                    while (bv < ev)
                                    {
                                        list.Add(bv++);
                                    }
                                    list.Add(ev);
                                }
                            }
                        }
                        else
                        {
                            int v = 0;
                            if (int.TryParse(ss, out v)) list.Add(v);
                        }
                    }
                }
                list.TrimExcess();
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public CacheKeyModel Get(string node, string item)
        {
            var m = this.list.Find(q => q.Node == node && q.Item == item);

            return m;
        }

        /// <summary>
        /// 获取key
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="item">名称</param>
        /// <returns></returns>
        public string GetKey(string node, string item)
        {
            var m = this.Get(node, item);

            return m?.Key;
        }

        /// <summary>
        /// 获取过期时间
        /// </summary>
        /// <param name="node"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public TimeSpan? GetExpire(string node, string item)
        {
            var m = this.Get(node, item);

            return m?.Expire;
        }

        /// <summary>
        /// 获取db
        /// </summary>
        /// <param name="node"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<int> GetDb(string node, string item)
        {
            var m = this.Get(node, item);

            return m?.Db;
        }
    }
}