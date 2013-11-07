using System;
using System.Web;
using System.Linq;
using Sparrow.CommonLibrary.Utility;
using Sparrow.CommonLibrary.Weblog.Collect;
using System.Collections.Generic;
using Sparrow.CommonLibrary.Weblog.Writer;
using System.Text.RegularExpressions;
using Sparrow.CommonLibrary.Logging;

namespace Sparrow.CommonLibrary.Weblog
{
    public class WeblogHttpModule : IHttpModule
    {
        /// <summary>
        /// 跳过采集过程
        /// </summary>
        private readonly bool Skip = false;
        /// <summary>
        /// 忽略的请求
        /// </summary>
        private readonly Regex[] Ignores;
        /// <summary>
        /// 
        /// </summary>
        private readonly Weblogger weblogger;

        public WeblogHttpModule()
        {
            try
            {
                //ignores
                var ignoreList = new List<Regex>();
                foreach (var ignore in WeblogSettings.Ignores)
                {
                    try
                    {
                        ignoreList.Add(new Regex(ignore, RegexOptions.Compiled | RegexOptions.IgnoreCase));
                    }
                    catch (Exception ex)
                    {
                        Log.GetLog(LoggingSettings.SparrowCategory).Warning("weblog跟踪初始化Ignores", ex, new { Regex = ignore });
                    }
                }
                Ignores = ignoreList.ToArray();
                //weblogger
                weblogger = new Weblogger(WeblogSettings.Version, WeblogSettings.Items, WeblogSettings.Writer, WeblogSettings.WriterParameters);
            }
            catch (Exception ex)
            {
                Skip = true;
                Logging.Log.GetLog(LoggingSettings.SparrowCategory).Error("WeblogHttpModule初始化失败。", ex);
            }
        }

        #region implements

        public void Init(HttpApplication app)
        {
            //如果忽略数据采集直接跳过，不注册事件
            if (Skip)
                return;

            var rawUrl = app.Request.RawUrl;
            foreach (var ignore in Ignores)
            {
                if (ignore.IsMatch(rawUrl))
                    return;
            }

            //注册事件
            app.BeginRequest += new EventHandler(Application_BeginRequest);
            app.EndRequest += new EventHandler(Application_EndRequest);
        }

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(disposed);
        }

        public void Dispose(bool disposed)
        {
            if (disposed)
            {
                weblogger.Dispose();
                disposed = true;
            }
        }

        #endregion

        #region events

        private void Application_BeginRequest(Object source, EventArgs args)
        {
            var app = (HttpApplication)source;
            weblogger.Begin(app);
        }

        private void Application_EndRequest(Object source, EventArgs args)
        {
            var app = (HttpApplication)source;
            weblogger.End(app);
            //完成数据采集
            weblogger.Complete(app);
        }

        #endregion
    }
}
