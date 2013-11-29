using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sparrow.CommonLibrary.Logging;
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

        private Logging.Log Log { get { return Logging.Log.GetLog(Configuration.WeblogSettings.Settings.LogCategory); } }

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
                catch (Exception ex)
                {
                    Log.Warning("带有上下文的采集器错误，Begin", ex, new { Name = collect.Name, Type = collect.GetType().FullName });
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
                catch (Exception ex)
                {
                    Log.Warning("带有上下文的采集器错误，End", ex, new { Name = collect.Name, Type = collect.GetType().FullName });
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
                catch (Exception ex)
                {
                    Log.Warning("采集器错误，GetValue", ex, new { Name = Collecters[i].Name, Type = Collecters[i].GetType().FullName });
                }
            }
            //
            try
            {
                buffer.Write(new WeblogEntry(data));
            }
            catch (Exception ex)
            {
                Log.Error("采集器采集的数据输出至缓冲区异常。", ex);
            }
        }

        private void buffer_OnFlush(object sender, BufferedFlushEventArgs<WeblogEntry> e)
        {
            try
            {
                var weblogs = new WeblogEntryCollection(Version, CollecterNames, e.List);
                Writer.Write(weblogs);
            }
            catch (Exception ex)
            {
                Log.Error("weblog写日志错误。", ex);
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
