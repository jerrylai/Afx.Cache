using System;

namespace Afx.Cache
{
    /// <summary>
    /// json Serialize
    /// </summary>
    public interface IJsonSerialize
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        string Serialize<T>(T m);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        T Deserialize<T>(string json);
    }
}
