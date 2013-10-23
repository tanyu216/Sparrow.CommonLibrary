using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Sparrow.CommonLibrary.Utility
{
    /// <summary>
    /// Json序列、反序列化
    /// </summary>
    public static class JsonSerialize
    {
        private static IJsonSerializer Serializer;

        static JsonSerialize()
        {
            Serializer = new DefaultJsonSerializer();
        }

        /// <summary>
        /// 重置序列化器
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static void ResetSerializer(IJsonSerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");
            JsonSerialize.Serializer = serializer;
        }

        /// <summary>
        /// 序列化对象<paramref name="data"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Serialize(object data)
        {
            return Serializer.Serialize(data);
        }

        /// <summary>
        /// 序列化对象<paramref name="data"/>，输出至<paramref name="output"/>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="output"></param>
        public static void Serialize(object data, Stream output)
        {
            Serializer.Serialize(data, output);
        }

        /// <summary>
        /// 序列化对象<paramref name="data"/>，输出至<paramref name="output"/>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="output"></param>
        public static void Serialize(object data, StringBuilder output)
        {
            Serializer.Serialize(data, output);
        }

        /// <summary>
        /// 将数据<paramref name="data"/>反序化为指定类型<typeparamref name="T"/>的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(string data)
        {
            return Serializer.DeSerialize<T>(data);
        }

        /// <summary>
        /// 将数据<paramref name="data"/>反序化为指定类型<typeparamref name="T"/>的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(string data, Encoding encoding)
        {
            return Serializer.DeSerialize<T>(data, encoding);
        }

        /// <summary>
        /// 将数据<paramref name="data"/>反序化为指定类型<typeparamref name="T"/>的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(Stream data)
        {
            return Serializer.DeSerialize<T>(data);
        }

        /// <summary>
        /// 默认实现的Json序列化（封装的.NET框架自带序列化对象<see cref="DataContractJsonSerializer"/>。
        /// </summary>
        private class DefaultJsonSerializer : IJsonSerializer
        {
            public Encoding Encoding { get { return Encoding.UTF8; } }

            public string Serialize(object data)
            {
                using (var ms = new MemoryStream())
                {
                    Serialize(data, ms);
                    string json = Encoding.GetString(ms.ToArray());
                    return json;
                }
            }

            public void Serialize(object data, Stream output)
            {
                if (output == null)
                    throw new ArgumentNullException("output");
                var ds = new DataContractJsonSerializer(data == null ? typeof(object) : data.GetType());
                ds.WriteObject(output, data);
            }

            public void Serialize(object data, StringBuilder output)
            {
                if (output == null)
                    throw new ArgumentNullException("output");
                output.Append(Serialize(data));
            }

            public T DeSerialize<T>(string data)
            {
                return DeSerialize<T>(data, Encoding);
            }

            public T DeSerialize<T>(string data, Encoding encoding)
            {
                using (MemoryStream ms = new MemoryStream(encoding.GetBytes(data)))
                {
                    return DeSerialize<T>(ms);
                }
            }

            public T DeSerialize<T>(Stream data)
            {
                var ds = new DataContractJsonSerializer(typeof(T));
                T obj = (T)ds.ReadObject(data);
                return obj;
            }
        }
    }

    public interface IJsonSerializer
    {
        /// <summary>
        /// 反序列化默认使用的字符编码
        /// </summary>
        Encoding Encoding { get; }

        /// <summary>
        /// 序列化对象<paramref name="data"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        string Serialize(object data);

        /// <summary>
        /// 序列化对象<paramref name="data"/>，输出至<paramref name="output"/>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="output"></param>
        void Serialize(object data, Stream output);

        /// <summary>
        /// 序列化对象<paramref name="data"/>，输出至<paramref name="output"/>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="output"></param>
        void Serialize(object data, StringBuilder output);

        /// <summary>
        /// 将数据<paramref name="data"/>反序化为指定类型<typeparamref name="T"/>的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        T DeSerialize<T>(string data);

        /// <summary>
        /// 将数据<paramref name="data"/>反序化为指定类型<typeparamref name="T"/>的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        T DeSerialize<T>(string data, Encoding encoding);

        /// <summary>
        /// 将数据<paramref name="data"/>反序化为指定类型<typeparamref name="T"/>的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        T DeSerialize<T>(Stream data);
    }
}
