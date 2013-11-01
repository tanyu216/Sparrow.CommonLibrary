using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Script.Serialization;

namespace Sparrow.CommonLibrary.Logging.Writer
{
    public abstract class LogWriterBase : ILogWriter
    {
        public LogWriterBase()
        {
            _parameters = new Dictionary<string, string>();
        }

        private string ExceptionDetail(Exception ex, int depth = 0)
        {
            if (depth < 0 || ex == null)
                return string.Empty;

            var str = new StringBuilder();
            var indent = string.Empty;
            if (depth == 0)
            {
                str.Append("Exception.Type:").AppendFormat("{0},{1},{2}", ex.GetType().FullName, ex.GetType().Assembly.FullName, ex.GetType().Assembly.ImageRuntimeVersion).AppendLine();
            }
            else
            {
                indent = new string('-', depth * 2) + ">";
                str.Append(indent).AppendLine("Exception.Inner:" + depth);
                str.Append(indent).AppendFormat("{0},{1},{2}", ex.GetType().FullName, ex.GetType().Assembly.FullName, ex.GetType().Assembly.ImageRuntimeVersion).AppendLine();
            }

            str.Append(indent).Append("Exception.Message:").AppendLine(ex.Message);

            var externalException = ex as ExternalException;
            if (externalException != null)
            {
                str.Append(indent).Append("Exception.ErrorCode:").AppendLine(externalException.ErrorCode.ToString(CultureInfo.InvariantCulture));
                var socketException = ex as SocketException;
                if (socketException != null)
                {
                    str.Append(indent).Append("Exception.SocketErrorCode:").AppendLine(socketException.SocketErrorCode.ToString());
                    str.Append(indent).Append("Exception.NativeErrorCode:").AppendLine(socketException.NativeErrorCode.ToString(CultureInfo.InvariantCulture));
                }
            }
            str.Append(indent).Append("Exception.Trace:").AppendLine(ex.StackTrace);

            var enumerator = ex.Data.GetEnumerator();
            while (enumerator.MoveNext())
            {
                str.Append(indent).Append("Exception.Database:[").Append(enumerator.Key).Append(']').Append(enumerator.Value).AppendLine();
            }

            str.Append(ExceptionDetail(ex.InnerException, ++depth));
            return str.ToString();
        }

        protected virtual string ExceptionSerializer(Exception ex)
        {
            return ExceptionDetail(ex, 0);
        }

        protected virtual string PropertiesSerializer(object data)
        {
            return Sparrow.CommonLibrary.Utility.JsonSerialize.Serialize(data);
        }

        #region implement

        public string FilterName { get; set; }

        private readonly IDictionary<string, string> _parameters;

        public void AddParameter(string name, string value)
        {
            _parameters[name] = value;
        }

        public string GetParameter(string name)
        {
            string value;
            if (_parameters.TryGetValue(name, out value))
                return value;
            return null;
        }

        public abstract void Write(IList<LogEntry> logs);

        #endregion

    }
}
