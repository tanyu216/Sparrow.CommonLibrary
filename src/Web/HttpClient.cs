using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Net.Security;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Sparrow.CommonLibrary.Logging;
using Sparrow.CommonLibrary.Retrying;

namespace Sparrow.CommonLibrary.Web
{
    public class HttpClient
    {
        /// <summary>
        /// 编码类型，默认为utf-8。
        /// </summary>
        public Encoding Encoding { get; set; }

        private NameValueCollection _queryString;
        /// <summary>
        /// 提交至目标服务器的url字符参数
        /// </summary>
        public NameValueCollection QueryString
        {
            get
            {
                if (_queryString == null)
                    _queryString = new NameValueCollection();
                return _queryString;
            }
            set { _queryString = value; }
        }

        private NameValueCollection _form;
        /// <summary>
        /// 提交至目标服务的表单数据
        /// </summary>
        public NameValueCollection Form
        {
            get
            {
                if (_form == null)
                    _form = new NameValueCollection();
                return _form;
            }
            set { _form = value; }
        }

        public NameValueCollection _headers;
        /// <summary>
        /// 自定义Http请求的头
        /// </summary>
        public NameValueCollection Headers
        {
            get
            {
                if (_headers == null)
                    _headers = new NameValueCollection();
                return _headers;
            }
            set { _headers = value; }
        }

        /// <summary>
        /// Http请求等待响应的最长时间（单位：毫秒）。
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// HttpClient默认的浏览器
        /// </summary>
        private static readonly string HTTPCLIENT_USERAGENT = "Mozilla/5.0 (SparrowHttpClient; ) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.60 Safari/537.1";
        private static string _userAgent;
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent ?? HTTPCLIENT_USERAGENT; }
            set { _userAgent = value; }
        }

        /// <summary>
        /// Cookies
        /// </summary>
        public System.Net.CookieContainer Cookies { get; set; }

        /// <summary>
        /// 支持GZip打包数据后再发送至服务
        /// </summary>
        public bool CompressData { get; set; }

        private RetryStrategy _retry;
        /// <summary>
        /// 重试策略，当http请求发生意外时，尝试重新请求。
        /// </summary>
        public RetryStrategy Retry
        {
            get { return _retry = _retry ?? new NonRetryInterval(); }
            set { _retry = value; }
        }

        protected readonly string Url;

        public HttpClient(string url)
        {
            Url = url;
            Timeout = 10 * 1000;
            Encoding = Encoding.UTF8;
            CompressData = true;
        }

        protected virtual string NameValueSerialize(NameValueCollection values)
        {
            if (values != null)
            {
                var builder = new StringBuilder();
                foreach (var key in values.AllKeys)
                {
                    builder.Append(HttpUtility.UrlEncode(key))
                        .Append('=')
                        .Append(HttpUtility.UrlEncode(values[key]))
                        .Append('&');
                }
                if (builder[builder.Length - 1] == '&')
                    builder.Remove(builder.Length - 1, 1);
                return builder.ToString();
            }
            return string.Empty;
        }

        private HttpWebRequest CreateRequest(string method)
        {
            string url;
            if (_queryString != null)
            {
                var queryString = NameValueSerialize(_queryString);
                var joinCharset = Url.IndexOf('?') > 0 ? "&" : "?";
                url = string.IsNullOrEmpty(queryString) == false ? string.Concat(Url, joinCharset, queryString) : Url;
            }
            else
            {
                url = Url;
            }

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            return request;
        }

        protected virtual void InitRequest(HttpWebRequest request)
        {
            request.Timeout = Timeout;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Accept-Charset", Encoding.WebName);
            request.Headers.Add("Accept-Encoding", "gzip,deflate");//始终接受压缩格式的数据

            if (request.RequestUri.AbsoluteUri.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => true);
                request.ProtocolVersion = HttpVersion.Version10;
            }
            if (_headers != null)
            {
                request.Headers.Add(_headers);
            }
            if (Cookies != null)
            {
                request.CookieContainer = Cookies;
            }
            request.UserAgent = UserAgent;
        }

        protected virtual void LoadFormData(HttpWebRequest request)
        {
            if (_form == null)
                return;

            var forms = NameValueSerialize(Form);
            if (string.IsNullOrEmpty(forms) == false)
            {
                using (var reqStream = request.GetRequestStream())
                {
                    if (CompressData)
                    {
                        request.Headers.Add("Content-Encoding", "gzip");
                        CompressByGZip(forms, reqStream);
                    }
                    else
                    {
                        using (var sw = new StreamWriter(reqStream, Encoding))
                        {
                            sw.Write(forms);
                        }
                    }
                }
            }
        }

        protected virtual ResponseResult SubmitRequest(string method)
        {
            var request = CreateRequest(method);
            InitRequest(request);
            LoadFormData(request);

            try
            {
                return Retry.DoExecute<ResponseResult>(() => CreateReponseResult((HttpWebResponse)request.GetResponse()));
            }
            catch (WebException ex)
            {
                return CreateReponseResult((HttpWebResponse)ex.Response);
            }
        }

        protected virtual ResponseResult CreateReponseResult(HttpWebResponse response)
        {
            return new ResponseResult(response);
        }

        /// <summary>
        /// 压缩数据，数据压缩格式为GZip
        /// </summary>
        /// <param name="text"></param>
        /// <param name="output"></param>
        protected virtual void CompressByGZip(string text, Stream output)
        {
            using (var gs = new GZipStream(output, CompressionMode.Compress))
            {
                using (var sw = new StreamWriter(gs, Encoding))
                {
                    sw.Write(text);
                }
            }
        }

        /// <summary>
        /// 以GET传值方式向目标服务器发出Http请求。
        /// </summary>
        /// <returns></returns>
        public ResponseResult Get()
        {
            return SubmitRequest("GET");
        }

        /// <summary>
        /// 以POST的方式向目标服务器发出Http请求。
        /// </summary>
        /// <returns></returns>
        public ResponseResult Post()
        {
            return SubmitRequest("POST");
        }

        /// <summary>
        /// 以PUT的方式向目标服务器发出Http请求。
        /// </summary>
        /// <returns></returns>
        public ResponseResult Put()
        {
            return SubmitRequest("PUT");
        }

        /// <summary>
        /// 以DELETE的方式向目标服务器发出Http请求。
        /// </summary>
        /// <returns></returns>
        public ResponseResult Delete()
        {
            return SubmitRequest("DELETE");
        }

        /// <summary>
        /// 转换Cookies
        /// </summary>
        /// <param name="source">cookies的源</param>
        /// <param name="target">cookies转换至target中</param>
        /// <param name="keys">转换至target的cookies依据</param>
        /// <param name="domain">当参数source中的cookie缺少domain值时，使用指定domain，加入target中。</param>
        public static void LoadCookies(System.Web.HttpCookieCollection source, System.Net.CookieCollection target, string[] keys, string domain = null)
        {
            if (source == null || keys == null || keys.Length == 0)
                return;

            for (int i = 0; i < source.Count; i++)
            {
                if (keys.FirstOrDefault(x => x != null && x.ToLower() == source[i].Name.ToLower()) == null)
                    continue;
                var cookie = source[i];
                if (cookie == null || String.IsNullOrEmpty(cookie.Value)) continue;

                var ncookie = new Cookie(cookie.Name, cookie.Value)
                {
                    Expires = cookie.Expires,
                    Domain = string.IsNullOrEmpty(cookie.Domain) ? domain : cookie.Domain,
                    HttpOnly = cookie.HttpOnly,
                    Secure = cookie.Secure,
                    Path = cookie.Path
                };
                target.Add(ncookie);
            }
        }

        /// <summary>
        /// 转换Cookies
        /// </summary>
        /// <param name="source">cookies的源</param>
        /// <param name="target">cookies转换至target中</param>
        /// <param name="keys">转换至target的cookies依据</param>
        /// <param name="domain">当参数source中的cookie缺少domain值时，使用指定domain，加入target中。</param>
        public static void LoadCookies(System.Net.CookieCollection source, System.Web.HttpCookieCollection target, string[] keys, string domain = null)
        {
            if (source == null || keys == null || keys.Length == 0)
                return;

            for (int i = 0; i < source.Count; i++)
            {
                if (keys.FirstOrDefault(x => x != null && x.ToLower() == source[i].Name.ToLower()) == null)
                    continue;
                var cookie = source[i];

                var hcookie = new System.Web.HttpCookie(cookie.Name, cookie.Value)
                {
                    Path = cookie.Path,
                    Domain = string.IsNullOrEmpty(cookie.Domain) ? domain : cookie.Domain,
                    Expires = cookie.Expires,
                    Secure = cookie.Secure,
                    HttpOnly = cookie.HttpOnly
                };
                target.Add(hcookie);
            }
        }
    }

    public class ResponseResult : IDisposable
    {
        public HttpWebResponse Response { get; private set; }

        public HttpStatusCode StatusCode { get { return Response.StatusCode; } }

        public string Status { get { return Response.StatusDescription; } }

        public Encoding Encoding { get { return Encoding.GetEncoding(Response.CharacterSet); } }

        public string ContentType { get { return Response.ContentType; } }

        public virtual string GetOutputString()
        {
            using (var responseStream = Response.GetResponseStream())
            {
                using (var sm = Decompress(responseStream, Regex.Match(Response.ContentEncoding, "gzip|deflate").Groups[0].Value))
                {
                    using (var rs = new StreamReader(sm))
                    {
                        return rs.ReadToEnd();
                    }
                }
            }
        }

        public virtual Stream GetOutputStream()
        {
            using (var responseStream = Response.GetResponseStream())
            {
                return Decompress(responseStream, Regex.Match(Response.ContentEncoding, "gzip|deflate").Groups[0].Value);
            }
        }

        public ResponseResult(HttpWebResponse response)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            Response = response;
        }

        /// <summary>
        /// 解压数据（不支持的压缩格式，直接返回源数据）
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type">gzip/deflate</param>
        /// <returns></returns>
        protected virtual Stream Decompress(Stream stream, string type)
        {
            switch (type)
            {
                case "gzip":
                    return new GZipStream(stream, CompressionMode.Decompress);
                case "deflate":
                    return new DeflateStream(stream, CompressionMode.Decompress);
                default:
                    return stream;
            }
        }

        #region IDisposable

        private bool _dispose;

        public void Dispose()
        {
            Dispose(_dispose);
        }

        private void Dispose(bool dispose)
        {
            if (dispose)
            {
                using (Response) { };
                _dispose = true;
            }
        }

        #endregion
    }
}
