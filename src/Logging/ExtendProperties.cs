using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using Sparrow.CommonLibrary.Utility.Extenssions;

namespace Sparrow.CommonLibrary.Logging
{
    /// <summary>
    /// 日志属性扩展
    /// </summary>
    public class ExtendProperties
    {
        private readonly bool _clientIp;
        private readonly bool _rawUrl;
        private readonly bool _urlRefer;
        private readonly bool _userAgent;
        private readonly string[] _cookies;

        private readonly object _obj;
        private readonly IDictionary<string, object> _properties;

        public ExtendProperties()
            : this(new Dictionary<string, object>())
        {
        }

        public ExtendProperties(IDictionary<string, object> properties)
        {
            _properties = properties ?? new Dictionary<string, object>();
        }

        public ExtendProperties(object properties)
            : this()
        {
            _obj = properties;
        }

        public ExtendProperties(IDictionary<string, object> properties, bool clientIp, bool rawUrl, bool urlRefer, bool userAgent, string[] cookies)
            : this(properties)
        {
            _clientIp = clientIp;
            _rawUrl = rawUrl;
            _urlRefer = urlRefer;
            _userAgent = userAgent;
            _cookies = cookies;
        }

        public ExtendProperties(object properties, bool clientIp, bool rawUrl, bool urlRefer, bool userAgent, string[] cookies)
            : this(properties)
        {
            _clientIp = clientIp;
            _rawUrl = rawUrl;
            _urlRefer = urlRefer;
            _userAgent = userAgent;
            _cookies = cookies;
        }

        public ExtendProperties(bool clientIp, bool rawUrl, bool urlRefer, bool userAgent, string[] cookies)
            : base()
        {
            _clientIp = clientIp;
            _rawUrl = rawUrl;
            _urlRefer = urlRefer;
            _userAgent = userAgent;
            _cookies = cookies;
        }

        private bool IsSingleVariant(object value)
        {
            if (value is string || value is char)
                return true;
            if (value is Int16 || value is Int32 || value is Int64 || value is UInt16 || value is UInt32 || value is UInt64)
                return true;
            if (value is decimal || value is double || value is float)
                return true;
            if (value is DateTime)
                return true;
            if (value is byte)
                return true;
            if (value is bool)
                return true;
            if (value is Array)
                return true;
            //
            return false;
        }

        public virtual IDictionary<string, object> Complete()
        {
            // arguments
            if (_obj != null)
            {
                if (IsSingleVariant(_obj))
                {
                    _properties["__arguments"] = _obj;
                }
                else
                {
                    var properties = _obj.GetType().GetProperties(BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
                    foreach (var propertyInfo in properties)
                    {
                        _properties[propertyInfo.Name] = propertyInfo.GetValue(_obj, null);
                    }
                    //
                    var fileds = _obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Static);
                    foreach (var fieldInfo in fileds)
                    {
                        _properties[fieldInfo.Name] = fieldInfo.GetValue(_obj);
                    }
                }
            }
            // web
            if (_clientIp)
            {
                _properties["__WebClientIp"] = HttpContext.Current.Request.GetClientIp();
            }
            if (_rawUrl)
            {
                _properties["__WebRawUrl"] = HttpContext.Current.Request.RawUrl;
            }
            if (_urlRefer)
            {
                _properties["__WebReferUrl"] = HttpContext.Current.Request.UrlReferrer.ToString();
            }
            if (_userAgent)
            {
                _properties["__WebUserAgent"] = HttpContext.Current.Request.UserAgent;
            }
            if (_cookies != null)
            {
                var cookies = new List<string>(_cookies.Length);
                foreach (var name in _cookies)
                {
                    var cookie = HttpContext.Current.Request.Cookies[name];
                    if (cookie != null)
                    {
                        var item = new StringBuilder();
                        item.AppendFormat("name={0},value={1}", cookie.Name, cookie.Value);
                        if (string.IsNullOrEmpty(cookie.Domain) == false)
                            item.AppendFormat(",domain={0}", cookie.Domain);
                        if (string.IsNullOrEmpty(cookie.Path) == false)
                            item.AppendFormat(",path={0}", cookie.Path);
                        if (cookie.HttpOnly)
                            item.Append(",httponly=true");
                        if (cookie.Secure)
                            item.Append(",secure=true");
                        if (cookie.Expires != default(DateTime))
                            item.AppendFormat(",expires={0:yy-MM-dd HH:mm:ss}", cookie.Expires);
                        cookies.Add(item.Append(";").ToString());
                    }
                }
                _properties["__WebCookie"] = cookies;
            }
            //
            return _properties;
        }
    }
}
