using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Sparrow.CommonLibrary.Common;

namespace Sparrow.CommonLibrary.Logging.Writer
{
    public class TextLogWriter : LogWriterBase
    {
        public static readonly string MaxSizeParamName = "maxsize";
        public static readonly string FolderParamName = "folder";

        private readonly PathBuilder _pathBuilder;
        public PathBuilder PathBuilder { get { return _pathBuilder; } }

        public TextLogWriter()
        {
            _pathBuilder = new PathBuilder();
            _pathBuilder.SetVariant("%filtername%", (x) => ((TextLogWriter)x).FilterName);
        }

        #region implement

        public override void Write(IList<LogEntry> logs)
        {
            using (var sm = CreateStream(GetPath()))
            {
                using (var sw = new StreamWriter(sm, Encoding.UTF8))
                {
                    foreach (var entry in logs)
                        OutputToStream(sw, entry);
                }
            }
        }

        #endregion

        protected virtual string GetPath()
        {
            var filepath = _pathBuilder.Build(GetParameter(FolderParamName), this);
            filepath = _pathBuilder.RebuildPathByFileSize(filepath, GetParameter(MaxSizeParamName));
            //
            if (!Directory.Exists(Path.GetDirectoryName(filepath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            }
            return filepath;
        }

        protected virtual Stream CreateStream(string filepath)
        {
            return new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 1024 * 8);
        }

        protected virtual void OutputToStream(StreamWriter sw, LogEntry log)
        {
            sw.WriteLine("Message:{0}", log.Message);
            sw.WriteLine("LogLevel:{0}", log.Level);
            sw.WriteLine("Categories:{0}", string.Join(",", log.Categories ?? new string[0]));
            sw.WriteLine("EventId:{0}", log.EventId);
            sw.WriteLine("Code:{0}", log.Code);
            sw.WriteLine("UserId:{0}", log.UserId);
            sw.WriteLine("UnixTimestamp:{0}", log.Timestamp);
            sw.WriteLine("UTC:{0}", ((DateTime)(Timestamp)log.Timestamp).ToString());
            sw.WriteLine("Machine:{0}", log.Machine);
            sw.WriteLine("ThreadId:{0}", log.ThreadId);
            sw.WriteLine("ThreadName:{0}", log.ThreadName);
            sw.WriteLine("ProcessId:{0}", log.ProcessId);
            sw.WriteLine("ProcessName:{0}", log.ProcessName);
            sw.WriteLine("AppDomainId:{0}", log.AppDomainId);
            sw.WriteLine("AppDomainName:{0}", log.AppDomainName);
            sw.WriteLine("ExtendProperties:{0}", PropertiesSerializer(log.Properties));
            sw.WriteLine("Exception:{0}", ExceptionSerializer(log.Exception));
            sw.WriteLine("[<<END]");
        }

    }
}
