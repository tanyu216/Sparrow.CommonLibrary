using System;
using System.Collections.Generic;

namespace Sparrow.CommonLibrary.Logging
{
    /// <summary>
    /// 日志记录实体
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// 日志标识编号
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel Level { get; set; }
        /// <summary>
        /// 日志分类
        /// </summary>
        public string[] Categories { get; set; }
        /// <summary>
        /// 消息描述
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 业务操作关联的惟一编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; private set; }
        /// <summary>
        /// 机器名称
        /// </summary>
        public string Machine { get; set; }
        /// <summary>
        /// 托管线程Id
        /// </summary>
        public int ThreadId { get; set; }
        /// <summary>
        /// 线程名称
        /// </summary>
        public string ThreadName { get; set; }
        /// <summary>
        /// 进程Id
        /// </summary>
        public int ProcessId { get; set; }
        /// <summary>
        /// 进程名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 应用程序域Id
        /// </summary>
        public int AppDomainId { get; set; }
        /// <summary>
        /// 应用程序域名称
        /// </summary>
        public string AppDomainName { get; set; }
        /// <summary>
        /// 业务操作相关的数据
        /// </summary>
        public IDictionary<string, object> Properties { get; set; }

        private readonly long _unix = (new DateTime(1970, 1, 1)).Ticks;

        /// <summary>
        /// 
        /// </summary>
        public LogEntry()
        {
            Timestamp = (DateTime.UtcNow.Ticks - _unix) / 10000000;
        }

        /// <summary>
        /// 设定采集运行环境信息的初始化
        /// </summary>
        /// <param name="machine">机器名</param>
        /// <param name="thread">线程</param>
        /// <param name="process">进程</param>
        /// <param name="appDomain">应用程序域</param>
        public LogEntry(bool machine, bool thread, bool process, bool appDomain)
            : this()
        {
            if (machine)
                Machine = System.Net.Dns.GetHostName();
            //
            if (thread)
            {
                var th = System.Threading.Thread.CurrentThread;
                ThreadId = th.ManagedThreadId;
                ThreadName = th.Name;
            }
            //
            if (process)
            {
                var prs = System.Diagnostics.Process.GetCurrentProcess();
                ProcessId = prs.SessionId;
                ProcessName = prs.ProcessName;
            }
            //
            if (appDomain)
            {
                var domain = System.Threading.Thread.GetDomain();
                AppDomainId = domain.Id;
                AppDomainName = domain.FriendlyName;
            }
        }
    }
}
