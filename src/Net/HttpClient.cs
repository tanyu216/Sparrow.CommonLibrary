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
using Sparrow.CommonLibrary.Retrying;

namespace Sparrow.CommonLibrary.Net
{
    /// <summary>
    /// HttpClient，创建一个Http请求，获取请求地址返回的数据。
    /// </summary>
    public class HttpClient
    {
        /// <summary>
        /// 请求编码方式
        /// </summary>
        public Encoding Encoding { get; set; }

        private NameValueCollection _headers;
        /// <summary>
        /// 自定义Http请求的头
        /// </summary>
        public NameValueCollection Headers
        {
            get
            {
                if (_headers == null)
                {
                    _headers = new NameValueCollection();
                }
                return _headers;
            }
            set { _headers = value; }
        }

        /// <summary>
        /// Http请求等待响应的最长时间（单位：毫秒）。
        /// </summary>
        public int Timeout { get; set; }

        private List<Tuple<object, string>> _Certs;
        /// <summary>
        /// 签名证书
        /// </summary>
        public List<Tuple<object, string>> Certs
        {
            get
            {
                if (_Certs.Count > 0)
                    return _Certs.ToList();
                else
                    return new List<Tuple<object, string>>(0);
            }
        }
        /// <summary>
        /// 增加安全证书
        /// </summary>
        /// <param name="certFile">证书文件名称</param>
        /// <param name="password">证书密码</param>
        public void AddCertificates(string certFile, string password)
        {
            if (_Certs == null)
                _Certs = new List<Tuple<object, string>>();

            if (System.IO.File.Exists(certFile) == false)
                throw new FileNotFoundException("指定的证书路径不包含证书文件。", certFile);

            _Certs.Add(new Tuple<object, string>(certFile, password));
        }
        /// <summary>
        /// 增加安全证书
        /// </summary>
        /// <param name="rawdata">证书文件数据</param>
        /// <param name="password">证书密码</param>
        public void AddCertificates(byte[] rawdata, string password)
        {
            if (_Certs == null)
                _Certs = new List<Tuple<object, string>>();

            if (rawdata == null)
                throw new ArgumentNullException("rawdata");

            _Certs.Add(new Tuple<object, string>(rawdata, password));
        }

        /// <summary>
        /// HttpClient默认的浏览器
        /// </summary>
        private static readonly string HTTPCLIENT_USERAGENT = "Sparrow.HttpClient";
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
        /// 支持上传压缩后的表单数据
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

        /// <summary>
        /// Cookie容器
        /// </summary>
        public CookieContainer Cookies { get; set; }

        /// <summary>
        /// 是否允许302跳转
        /// </summary>
        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// http请求代理
        /// </summary>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// http url
        /// </summary>
        protected readonly string Url;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="url">Web请求的Url</param>
        public HttpClient(string url)
        {
            if (string.IsNullOrEmpty(url) || !url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("参数url不合法。");
            }

            Url = url;
            Timeout = 6 * 1000;
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// 将集合序列化成Web编码后的字符串格式
        /// </summary>
        /// <param name="values">参数集合</param>
        /// <returns>编码后的字符串</returns>
        protected virtual string NameValueSerialize(NameValueCollection values)
        {
            if (values != null)
            {
                var builder = new StringBuilder();
                for (var i = 0; i < values.Count; i++)
                {
                    builder.Append(HttpUtility.UrlEncode(values.AllKeys[i]))
                        .Append('=')
                        .Append(HttpUtility.UrlEncode(values[i]))
                        .Append('&');
                }
                if (builder[builder.Length - 1] == '&')
                {
                    builder.Remove(builder.Length - 1, 1);
                }
                return builder.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 创建一个HttpWebRequest对象
        /// </summary>
        /// <param name="method">HttpWebRequest实例对象</param>
        /// <returns>HttpWebRequest实例对象</returns>
        private HttpWebRequest CreateRequest(string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = method;
            request.AllowAutoRedirect = AllowAutoRedirect;
            request.Proxy = Proxy;
            return request;
        }

        /// <summary>
        /// 初始化HttpWebRequest请求
        /// </summary>
        /// <param name="request">HttpWebRequest实例对象</param>
        protected virtual void InitRequest(HttpWebRequest request)
        {
            request.Timeout = Timeout;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Accept-Charset", Encoding.UTF8.WebName);
            request.Headers.Add("Accept-Encoding", "gzip,deflate");//始终接受压缩格式的数据 
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            if (Cookies != null)
            {
                request.CookieContainer = Cookies;
            }

            if (request.RequestUri.AbsoluteUri.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => true);
                request.ProtocolVersion = HttpVersion.Version10;
            }

            if (_Certs != null && _Certs.Count > 0)
            {
                foreach (var item in _Certs)
                {
                    var rawdata = item.Item1 as byte[];
                    if (rawdata != null)
                        request.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate(rawdata, item.Item2));
                    else if (item.Item1 is string && System.IO.File.Exists(item.Item1.ToString()) == false)
                        request.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate(item.Item1.ToString(), item.Item2));
                    else
                        throw new WebException("安全证书数据不正大确。");
                }
            }

            if (_headers != null)
            {
                request.Headers.Add(_headers);
            }
            request.UserAgent = UserAgent;
        }

        /// <summary>
        /// 将请求的QueryString和表单数据载入至HttpWebRequest对象中
        /// </summary>
        /// <param name="request">HttpWebRequest实例对象</param>
        /// <param name="data"></param>
        protected virtual void LoadRequestData(HttpWebRequest request, string data)
        {
            if (data == null)
            {
                return;
            }

            using (var reqStream = request.GetRequestStream())
            {
                if (CompressData)
                {
                    request.Headers.Add("Content-Encoding", "gzip");
                    CompressByGZip(data, reqStream, Encoding);
                }
                else
                {
                    var content = Encoding.GetBytes(data);
                    reqStream.Write(content, 0, content.Length);
                }
            }
        }

        /// <summary>
        /// 提交一个Web请求
        /// </summary>
        /// <param name="method">Web请求方式</param>
        /// <returns>Web请求返回结果</returns>
        protected virtual ResponseResult SubmitRequest(string method)
        {
            return SubmitRequest(method, (string)null);
        }

        /// <summary>
        /// 提交一个Web请求
        /// </summary>
        /// <param name="method">Web请求方式</param>
        /// <param name="data"></param>
        /// <returns>Web请求返回结果</returns>
        protected virtual ResponseResult SubmitRequest(string method, NameValueCollection data)
        {
            return SubmitRequest(method, NameValueSerialize(data));
        }

        /// <summary>
        /// 提交一个Web请求
        /// </summary>
        /// <param name="method">Web请求方式</param>
        /// <param name="data"></param>
        /// <returns>Web请求返回结果</returns>
        protected virtual ResponseResult SubmitRequest(string method, string data)
        {
            var request = CreateRequest(method);
            InitRequest(request);
            LoadRequestData(request, data);

            try
            {
                return Retry.DoExecute<ResponseResult>(() => CreateReponseResult((HttpWebResponse)request.GetResponse()));
            }
            catch (WebException ex)
            {
                if ((HttpWebResponse)ex.Response == null)
                {
                    throw ex;
                }
                return CreateReponseResult((HttpWebResponse)ex.Response);
            }
        }

        /// <summary>
        /// 包装一个HttpWebResponse
        /// </summary>
        /// <param name="response">Web请求返回结果</param>
        /// <returns>封装后的Web请求返回结果</returns>
        protected virtual ResponseResult CreateReponseResult(HttpWebResponse response)
        {
            return new ResponseResult(response);
        }

        /// <summary>
        /// 压缩数据，数据压缩格式为GZip
        /// </summary>
        /// <param name="encodedText">url编码后的表单数据</param>
        /// <param name="output">输出流</param>
        /// <param name="encoding"></param>
        protected virtual void CompressByGZip(string encodedText, Stream output, Encoding encoding)
        {
            using (var gs = new GZipStream(output, CompressionMode.Compress))
            {
                var data = encoding.GetBytes(encodedText);
                gs.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// 以GET传值方式向目标服务器发出Http请求。
        /// </summary>
        /// <returns>封装后的Web请求返回结果</returns>
        public ResponseResult Get()
        {
            return SubmitRequest("GET");
        }

        /// <summary>
        /// 以PUT传值方式向目标服务器发出Http请求。
        /// </summary>
        /// <param name="data">向目标服务发送的数据</param>
        /// <returns>封装后的Web请求返回结果</returns>
        public ResponseResult Put(NameValueCollection data)
        {
            return SubmitRequest("PUT", data);
        }

        /// <summary>
        /// 以PUT传值方式向目标服务器发出Http请求。
        /// </summary>
        /// <param name="data">向目标服务发送的数据</param>
        /// <returns>封装后的Web请求返回结果</returns>
        public ResponseResult Put(string data)
        {
            return SubmitRequest("PUT", data);
        }

        /// <summary>
        /// 以DELETE传值方式向目标服务器发出Http请求。
        /// </summary>
        /// <returns>封装后的Web请求返回结果</returns>
        public ResponseResult Delete()
        {
            return SubmitRequest("DELETE");
        }

        /// <summary>
        /// 以POST的方式向目标服务器发出Http请求。
        /// </summary>
        /// <param name="data">向目标服务发送的数据</param>
        /// <returns>封装后的Web请求返回结果</returns>
        public ResponseResult Post(NameValueCollection data)
        {
            return SubmitRequest("POST", data);
        }

        /// <summary>
        /// 以POST的方式向目标服务器发出Http请求。
        /// </summary>
        /// <param name="data">向目标服务发送的数据</param>
        /// <returns>封装后的Web请求返回结果</returns>
        public ResponseResult Post(string data)
        {
            return SubmitRequest("POST", data);
        }

        /// <summary>
        /// 向目标服务提交一个文件流
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public ResponseResult PostFileStream(Stream stream, string filename)
        {
            var request = CreateRequest("POST");
            InitRequest(request);

            //时间戳
            string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");
            //请求头部信息
            string strPostHeader = new StringBuilder()
                .Append("--")
                .Append(strBoundary)
                .Append("\r\n")
                .Append("Content-Disposition: form-data; name=\"")
                .Append("file")
                .Append("\"; filename=\"")
                .Append(filename)
                .Append("\"")
                .Append("\r\n")
                .Append("Content-Type: ")
                .Append("application/octet-stream")
                .Append("\r\n")
                .Append("\r\n").ToString();
            byte[] postHeaderBytes = Encoding.GetBytes(strPostHeader);
            request.AllowWriteStreamBuffering = false;
            request.ContentType = "multipart/form-data; boundary=" + strBoundary;
            long length = stream.Length + postHeaderBytes.Length + boundaryBytes.Length;
            request.ContentLength = length;

            var requestStream = request.GetRequestStream();
            //发送请求头部消息
            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            //
            var size = 0;
            var bufferLength = 4096;
            byte[] buffer = new byte[bufferLength];

            do
            {
                size = stream.Read(buffer, 0, bufferLength);
                if (size == 0)
                    break;
                requestStream.Write(buffer, 0, size);
            } while (true);
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            requestStream.Close();

            try
            {
                return Retry.DoExecute<ResponseResult>(() => CreateReponseResult((HttpWebResponse)request.GetResponse()));
            }
            catch (WebException ex)
            {
                if ((HttpWebResponse)ex.Response == null)
                {
                    throw ex;
                }
                return CreateReponseResult((HttpWebResponse)ex.Response);
            }
        }

        #region 简单的url请求封装
        /// <summary>
        /// 发起一个简单的GET请求，获取请求响应的数据
        /// </summary>
        /// <param name="url">url</param>
        /// <returns></returns>
        public static string Get(string url)
        {
            var httpClient = new HttpClient(url);
            using (var result = httpClient.Get())
            {
                if (result.StatusCode == HttpStatusCode.OK)
                    return result.GetOutputString();
            }
            return null;
        }
        /// <summary>
        /// 发起一个简单的POST请求，获取请求响应的数据
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="data">编码后的form表单，或json字符串。</param>
        /// <returns></returns>
        public static string Post(string url, string data)
        {
            var httpClient = new HttpClient(url);
            using (var result = httpClient.Post(data))
            {
                if (result.StatusCode == HttpStatusCode.OK)
                    return result.GetOutputString();
            }
            return null;
        }
        /// <summary>
        /// 发起一个简单的POST请求，获取请求响应的数据
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="data">form表单。</param>
        /// <returns></returns>
        public static string Post(string url, NameValueCollection data)
        {
            var httpClient = new HttpClient(url);
            using (var result = httpClient.Post(data))
            {
                if (result.StatusCode == HttpStatusCode.OK)
                    return result.GetOutputString();
            }
            return null;
        }
        #endregion
    }

    /// <summary>
    /// HttpWebResponse对象封装
    /// </summary>
    public class ResponseResult : IDisposable
    {
        /// <summary>
        /// HttpWebResponse对象
        /// </summary>
        public HttpWebResponse Response { get; private set; }

        /// <summary>
        /// http响应状态码
        /// </summary>
        public HttpStatusCode StatusCode { get { return Response.StatusCode; } }

        /// <summary>
        /// http响应状态码描述
        /// </summary>
        public string Status { get { return Response.StatusDescription; } }

        private Encoding _encoding;
        /// <summary>
        /// 获取或设置HTTP响应内容编码方式
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                if (_encoding != null)
                    return _encoding;

                var encoding = Response.CharacterSet;
                if (string.IsNullOrEmpty(encoding))
                {
                    var contentType = Response.Headers["Content-Type"];
                    foreach (var item in contentType.Split(';'))
                    {
                        if (item.Trim().StartsWith("encoding", StringComparison.OrdinalIgnoreCase) == false)
                            continue;
                        encoding = item.Split('=')[1].Trim();
                        break;
                    }
                }
                if (string.IsNullOrEmpty(encoding))
                    return _encoding = _encoding ?? System.Text.Encoding.UTF8;
                else
                    return _encoding = _encoding ?? Encoding.GetEncoding(encoding);
            }
            set { _encoding = value; }
        }

        /// <summary>
        /// 响应的内容类型，text/html、application/json
        /// </summary>
        public string ContentType { get { return Response.ContentType; } }

        /// <summary>
        /// 获取响应内容的字符串形式
        /// </summary>
        /// <returns>响应内容的字符串形式</returns>
        public virtual string GetOutputString()
        {
            using (var rs = new StreamReader(GetOutputStream(), Encoding))
            {
                return rs.ReadToEnd();
            }
        }

        /// <summary>
        /// 获取响应内容的数据流
        /// </summary>
        /// <returns>响应内容的数据流</returns>
        public virtual Stream GetOutputStream()
        {
            return Response.GetResponseStream();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="response">Web请求的返回结果对象</param>
        public ResponseResult(HttpWebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            Response = response;
        }

        #region IDisposable

        private bool _dispose;

        /// <summary>
        /// 资源释放
        /// </summary>
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
