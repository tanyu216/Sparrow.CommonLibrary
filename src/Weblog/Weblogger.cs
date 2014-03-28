using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sparrow.CommonLibrary.Common;
using Sparrow.CommonLibrary.Weblog.Collect;
using Sparrow.CommonLibrary.Weblog.Writer;

namespace Sparrow.CommonLibrary.Weblog
{
    public class Weblogger : IDisposable
    {
        /// <summary>
        /// 版本号标识
        /// </summary>
        private readonly string Version;
        /// <summary>
        /// 采集器名称
        /// </summary>
        private readonly string[] CollecterNames;
        /// <summary>
        /// 采集器
        /// </summary>
        private readonly ICollecter[] Collecters;
        /// <summary>
        /// 单独列出Collecters中实现接口<see cref="ICollecterWithContext"/>的对象
        /// </summary>
        private readonly ICollecterWithContext[] Collecter2s;
        /// <summary>
        /// 缓冲区
        /// </summary>
        private readonly Buffered<WeblogEntry> buffer;
        /// <summary>
        /// 写入指定的存储介质
        /// </summary>
        private readonly IWeblogWriter Writer;

        public Weblogger(string version, IEnumerable<ICollecter> collecters, IWeblogWriter writer)
        {
            if (string.IsNullOrEmpty(version))
                throw new ArgumentNullException("version");
            Version = version;

            if (writer == null)
                throw new ArgumentNullException("writer");
            Writer = writer;

            // 初始化采集器
            CollecterNames = collecters.Select(x => x.Name).ToArray();
            Collecters = collecters.ToArray();
            Collecter2s = collecters.Where(x => x is ICollecterWithContext).Cast<ICollecterWithContext>().ToArray();

            //初始化缓冲区
            buffer = new Buffered<WeblogEntry>();
            buffer.OnFlush += new EventHandler<BufferedFlushEventArgs<WeblogEntry>>(buffer_OnFlush);
        }

        public void Begin(HttpApplication context)
        {
            //开始上下文监听
            foreach (ICollecterWithContext collect in Collecter2s)
            {
                try
                {
                    if (collect != null)
                        collect.Begin(context);
                }
                catch 
                {
                }
            }
        }

        public void End(HttpApplication context)
        {
            //结束上下文监听
            foreach (ICollecterWithContext collect in Collecter2s)
            {
                try
                {
                    if (collect != null)
                        collect.End(context);
                }
                catch 
                {
                }
            }
        }

        public void Complete(HttpApplication context)
        {
            if (disposed)
                throw new ObjectDisposedException(this.GetType().Name);
            var data = new string[Collecters.Length];
            for (var i = data.Length - 1; i > -1; i--)
            {
                try
                {
                    if (Collecters[i] != null)
                    {
                        data[i] = Collecters[i].GetValue(context);
                    }
                }
                catch
                {
                }
            }
            //
            try
            {
                buffer.Write(new WeblogEntry(data));
            }
            catch
            {
            }
        }

        private void buffer_OnFlush(object sender, BufferedFlushEventArgs<WeblogEntry> e)
        {
            try
            {
                var weblogs = new WeblogEntryCollection(Version, CollecterNames, e.List);
                Writer.Write(weblogs);
            }
            catch
            {
            }
        }

        #region Implements IDisposable
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(disposed);
        }

        public void Dispose(bool disposed)
        {
            if (disposed)
            {
                buffer.Dispose();
                disposed = true;
            }
        }
        #endregion
    }
}
