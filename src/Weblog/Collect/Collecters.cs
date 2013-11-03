using Sparrow.CommonLibrary.Utility;
using Sparrow.CommonLibrary.Extenssions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog.Collect
{
    public class ReqUrlCollecter : ICollecter
    {
        public string Name
        {
            get { return "req_url"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            return app.Request.RawUrl;
        }
    }

    public class ReqProtocolCollecter : ICollecter
    {
        public string Name
        {
            get { return "protocol"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            return app.Request.ServerVariables["SERVER_PROTOCOL"].Split('/')[0].ToLower();
        }
    }

    public class ReqDomainCollecter : ICollecter
    {
        public string Name
        {
            get { return "domain"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            return app.Request.Url.Host;
        }
    }

    public class ReqAbsolutePathCollecter : ICollecter
    {
        public string Name
        {
            get { return "absolute_path"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            return app.Request.Url.AbsolutePath;
        }
    }

    public class ReqQueryStringCollecter : ICollecter
    {
        public string Name
        {
            get { return "query_string"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            return app.Request.Url.Query;
        }
    }

    public class ReqTypeCollecter : ICollecter
    {

        public string Name
        {
            get { return "req_type"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            return app.Request.RequestType;
        }
    }

    public class ReqUserAgentCollecter : ICollecter
    {

        public string Name
        {
            get { return "user_agent"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            var val = app.Request.UserAgent;
            if (val != null && val.Length > 500)
                return val.Substring(0, 500);
            return val;
        }
    }

    public class ReqUrlReferrerCollecter : ICollecter
    {

        public string Name
        {
            get { return "url_referrer"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            var val = app.Request.UrlReferrer.ToString();
            if (val != null && val.Length > 2038)
                return val.Substring(0, 2038);//ie对URI的长度限制
            return val;
        }
    }

    public class StatusCodeCollecter : ICollecter
    {
        public string Name
        {
            get { return "status_code"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            return app.Response.StatusCode.ToString(CultureInfo.InvariantCulture);
        }
    }

    public class ServerHostCollecter : ICollecter
    {
        public string Name
        {
            get { return "server_host"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            return app.Request.ServerVariables["LOCAL_ADDR"];
        }
    }

    public class ServerPortCollecter : ICollecter
    {
        public string Name
        {
            get { return "server_port"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            return app.Request.ServerVariables["SERVER_PORT"];
        }
    }

    public class UserHostCollecter : ICollecter
    {
        public string Name
        {
            get { return "user_host"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            return app.Request.GetClientIp();
        }
    }

    public class VisitTimeCollecter : ICollecter
    {

        public string Name
        {
            get { return "visit_time"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            return ((DateTime)Timestamp.Now).ToString();
        }
    }

    public class CookieSidCollecter : ICollecter
    {
        public string Name
        {
            get { return "sid"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            var cookie = app.Request.Cookies["sid"];
            if (cookie != null)
                return cookie.Value;
            return null;
        }
    }

    public class CookieVucaCollecter : ICollecter
    {
        public string Name
        {
            get { return "vuca"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            var cookie = app.Request.Cookies["vuca"];
            if (cookie != null)
                return cookie.Value;
            return null;
        }
    }

    public class RewriterUrlCollecter : ICollecter
    {
        protected static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> rewriterHeaders;

        static RewriterUrlCollecter()
        {
            rewriterHeaders = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
            rewriterHeaders.TryAdd("HTTP_X_REWRITE_URL", null);
            rewriterHeaders.TryAdd("X-REWRITE-URL", null);
            rewriterHeaders.TryAdd("HTTP_X_REWRITE_URL", null);
        }

        public string Name
        {
            get { return "rewriter_url"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            string RewriteUrl = app.Request.ServerVariables.AllKeys.FirstOrDefault(k => rewriterHeaders.ContainsKey(k));
            if (string.IsNullOrEmpty(RewriteUrl) == false)
            {
                string host_name = app.Request.Url.Host;
                string protocol = app.Request.ServerVariables["SERVER_PROTOCOL"].Split('/')[0].ToLower();
                return string.Format(@"{0}://{1}{2}", protocol, host_name, app.Request.ServerVariables[RewriteUrl]);
            }
            return null;
        }
    }

    public class LoadTimerCollecter : ICollecterWithContext
    {
        public LoadTimerCollecter()
        {
        }

        public void Begin(System.Web.HttpApplication app)
        {
            var watch = new Stopwatch();
            app.Application["__weblog:loadtime"] = watch;
            watch.Start();
        }

        public void End(System.Web.HttpApplication app)
        {
            var watch = app.Application["__weblog:loadtime"] as Stopwatch;
            if (watch != null && watch.IsRunning)
                ((Stopwatch)watch).Stop();
        }

        public string Name
        {
            get { return "load_time"; }
        }

        public string GetValue(System.Web.HttpApplication app)
        {
            var watch = app.Application["__weblog:loadtime"] as Stopwatch;
            if (watch != null)
                return watch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture);
            else
                return null;
        }
    }

}
