using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog.Collect
{
    public static class CollecterTypeContainer
    {
        private static ConcurrentDictionary<string, Type> collects;

        static CollecterTypeContainer()
        {
            collects = new ConcurrentDictionary<string, Type>();
            LoadFromAssembly();
            LoadFromConfig();
        }

        private static void LoadFromAssembly()
        {
            // 加载当前程序集中的采集器
            var assembly = System.Reflection.Assembly.GetCallingAssembly();
            var collectType = typeof(ICollecter);
            var @namespace = collectType.Namespace;
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && type.Namespace == @namespace)
                {
                    if (!IsICollectType(type))
                        continue;
                    Register(type);
                }
            }
        }

        private static void LoadFromConfig()
        {
            // 加载配置文件中的采集器
            var configuration = Configuration.WeblogConfigurationSection.GetSection();
            if (configuration == null)
                return;
            foreach (Configuration.CustomCollectElement custom in configuration.Collect.Customs)
            {
                if (IsICollectType(custom.Type))
                    SetCollect(custom.Name, custom.Type);
            }
        }

        /// <summary>
        /// 检测类型是否实现接口<see cref="ICollecter"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsICollectType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            var collectType = typeof(ICollecter);
            return type.GetInterfaces().Any(x => x == collectType);
        }

        /// <summary>
        /// 检测类型是否实现接口<see cref="ICollecterWithContext"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsICollect2Type(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            var collectType = typeof(ICollecterWithContext);
            return type.GetInterfaces().Any(x => x == collectType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public static void Register<T>() where T : ICollecter
        {
            Register(typeof(T), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public static void Register<T>(string name) where T : ICollecter
        {
            Register(typeof(T), name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type"></param>
        public static void Register(Type type)
        {
            Register(type, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public static void Register(Type type, string name)
        {
            if (!IsICollectType(type))
                throw new ArgumentException("参数type未实现接口:" + typeof(ICollecter).FullName);
            SetCollect(name, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        private static void SetCollect(string name, Type type)
        {
            if (name != null)
            {
                collects[name] = type;
            }
            else
            {
                var instance = (ICollecter)Activator.CreateInstance(type);
                collects[instance.Name] = type;
                if (instance is IDisposable)
                    ((IDisposable)instance).Dispose();
            }
        }

        /// <summary>
        /// 检测是否包含指定的<see cref="ICollecter"/>
        /// </summary>
        /// <returns></returns>
        public static bool Contains(string name)
        {
            return collects.ContainsKey(name);
        }

        /// <summary>
        /// 获取<see cref="ICollecter"/>的<see cref="System.Type"/>描述
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type GetCollectType(string name)
        {
            Type type;
            if (collects.TryGetValue(name, out type))
                return type;
            return null;
        }
    }
}
