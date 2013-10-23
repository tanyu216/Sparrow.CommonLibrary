using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Cache
{
    /// <summary>
    /// 缓存接口
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 验证缓存<paramref name="key"/>是否存在。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(string key);

        #region key/value
        /// <summary>
        /// 获取指定<paramref name="key"/>的缓存对象。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);
        /// <summary>
        /// 获取指定<paramref name="key"/>的缓存对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);
        /// <summary>
        /// 获取一组指定<paramref name="keys"/>的缓存对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        IDictionary<string, object> Get(string[] keys);
        /// <summary>
        /// 获取一组指定<paramref name="keys"/>的缓存对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        IDictionary<string, object> Get(IEnumerable<string> keys);
        /// <summary>
        /// 设置一个缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Set<T>(string key, T value);
        /// <summary>
        /// 设置一个缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireAt">对象绝对过期时间</param>
        void Set<T>(string key, T value, DateTime expireAt);

        #endregion

        #region dictionary

        #endregion
    }
}
