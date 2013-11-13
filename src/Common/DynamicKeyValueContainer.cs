using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sparrow.CommonLibrary.Common
{
    /// <summary>
    /// key/value容器对象，value不是一个静态值，而是由key设置的lambda表达式决定。
    /// </summary>
    public class DynamicKeyValueContainer<TKey, TValue>
    {
        private IDictionary<TKey, Func<object, TValue>> keyValues;

        public DynamicKeyValueContainer()
            : this(true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSync">指示是否支持同步，true表示支持即保证多线程安全。</param>
        public DynamicKeyValueContainer(bool isSync)
        {
            if (isSync)
                keyValues = new Dictionary<TKey, Func<object, TValue>>();
            else
                keyValues = new ConcurrentDictionary<TKey, Func<object, TValue>>();
            //

        }

        public ICollection<TKey> Keys { get { return keyValues.Keys; } }

        public Func<object, TValue> this[TKey key]
        {
            get { return keyValues[key]; }
            set { keyValues[key] = value; }
        }

        public void Add(TKey key, Func<object, TValue> func)
        {
            keyValues.Add(key, func);
        }

        public TValue GetValue(TKey key, object state)
        {
            Func<object, TValue> func;
            if (keyValues.TryGetValue(key, out func))
                return func(state);
            return default(TValue);
        }

        public void Remove(TKey key)
        {
            keyValues.Remove(key);
        }

        public bool ContainsKey(TKey key)
        {
            return keyValues.ContainsKey(key);
        }
    }
}
