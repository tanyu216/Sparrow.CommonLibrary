using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Sparrow.CommonLibrary.Utility
{
    /// <summary>
    /// app.config/web.config配置节点：appSettings，当配置文件中不包含需要的配置是，使用默认设置的配置。
    /// </summary>
    public static class AppSettings
    {
        private static readonly ConcurrentDictionary<string, object> keyValues;

        static AppSettings()
        {
            keyValues = new ConcurrentDictionary<string, object>();
        }

        /// <summary>
        /// 设置<paramref name="name"/>的默认值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetDefualtValue(string name, object value)
        {
            keyValues[name] = value;
        }

        /// <summary>
        /// 获取配置appSettings[name="<paramref name="name"/>"]的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetValue<T>(string name)
        {
            return GetValue<T>(name);
        }

        /// <summary>
        /// 获取配置appSettings[name="<paramref name="name"/>"]的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static T GetValue<T>(string name, T defValue)
        {
            //从配置文件中获取
            var val = ConfigurationManager.AppSettings[name];
            if (val != null)
            {
                return (T)Convert.ChangeType(val, typeof(T));
            }
            //从默认只列表中获取
            object output;
            if (keyValues.TryGetValue(name, out output))
            {
                if (output is T)
                    return (T)output;
                return (T)Convert.ChangeType(output, typeof(T));
            }
            return defValue;
        }
    }
}
