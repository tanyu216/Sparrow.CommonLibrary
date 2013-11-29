using System;
using System.Linq;
using System.Collections.Generic;
using Sparrow.CommonLibrary.Logging.Configuration;
using Sparrow.CommonLibrary.Common;
using Sparrow.CommonLibrary.Logging.Writer;
using Sparrow.CommonLibrary.Logging.Filter;

namespace Sparrow.CommonLibrary.Logging
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Log
    {
        private static Buffered<LogEntry> bufer;

        public static event EventHandler<LogEventArgs<LogEntry>> OnFail;

        public static LogLevel LowLevel
        {
            get
            {
                var configuration = LoggingConfigurationSection.GetSection();
                if (configuration != null)
                    return configuration.LowLevel;
                return LogLevel.Debug;
            }
        }

        private static Func<string> _currentUserId;
        public static Func<string> CurrentUserId { get { return _currentUserId; } }

        public static void Flush()
        {
            bufer.Flush();
        }

        static Log()
        {
            _currentUserId = null;
            bufer = new Buffered<LogEntry>();
            bufer.OnFlush += bufer_OnFlush;
        }

        private string[] _categories;
        private Log(string[] categories)
        {
            _categories = categories;
        }

        public static Log GetLog()
        {
            return new Log(new[] { LoggingSettings.Settings.DefaultCategory });
        }

        public static Log GetLog(string category)
        {
            return new Log(new[] { category });
        }

        public static Log GetLog(params string[] categories)
        {
            return new Log(categories);
        }

        private void Add(LogLevel level, string message, int eventId, string code, IDictionary<string, object> properties, Exception exception)
        {
            var isDebug = level == LogLevel.Debug;
            var logEntry = new LogEntry(true, isDebug, isDebug, isDebug)
                               {
                                   Categories = _categories,
                                   Properties = properties,
                                   Code = code,
                                   EventId = eventId,
                                   Exception = exception,
                                   Level = level,
                                   Message = message
                               };
            //
            var getter = CurrentUserId;
            if (getter != null)
                logEntry.UserId = CurrentUserId();
            //
            Add(logEntry);
        }

        public static void Add(LogEntry log)
        {
            bufer.Write(log);
        }

        public void Debug(string message)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, message, 0, null, null, null);
        }

        public void Debug(string message, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, message, 0, null, properties, null);
        }

        public void Debug(string message, object properties)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, message, 0, null, new ExtendProperties(properties).Complete(), null);
        }

        public void Debug(string message, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, message, 0, null, properties.Complete(), null);
        }

        public void Debug(string message, Exception exception)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, message, 0, null, null, exception);
        }

        public void Debug(string message, Exception exception, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, message, 0, null, properties, exception);
        }

        public void Debug(string message, Exception exception, object properties)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, message, 0, null, new ExtendProperties(properties).Complete(), exception);
        }

        public void Debug(string message, Exception exception, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, message, 0, null, properties.Complete(), exception);
        }

        public void Debug(int eventId, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, null, eventId, null, properties, null);
        }

        public void Debug(int eventId, object properties)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, null, eventId, null, new ExtendProperties(properties).Complete(), null);
        }

        public void Debug(int eventId, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, null, eventId, null, properties.Complete(), null);
        }

        public void Debug(int eventId, Exception exception)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, null, eventId, null, null, exception);
        }

        public void Debug(int eventId, Exception exception, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, null, eventId, null, properties, exception);
        }

        public void Debug(int eventId, Exception exception, object properties)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, null, eventId, null, new ExtendProperties(properties).Complete(), exception);
        }

        public void Debug(int eventId, Exception exception, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Debug)
                Add(LogLevel.Debug, null, eventId, null, properties.Complete(), exception);
        }

        public void Warning(string message)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, message, 0, null, null, null);
        }

        public void Warning(string message, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, message, 0, null, properties, null);
        }

        public void Warning(string message, object properties)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, message, 0, null, new ExtendProperties(properties).Complete(), null);
        }

        public void Warning(string message, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, message, 0, null, properties.Complete(), null);
        }

        public void Warning(string message, Exception exception)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, message, 0, null, null, exception);
        }

        public void Warning(string message, Exception exception, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, message, 0, null, properties, null);
        }

        public void Warning(string message, Exception exception, object properties)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, message, 0, null, new ExtendProperties(properties).Complete(), exception);
        }

        public void Warning(string message, Exception exception, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, message, 0, null, properties.Complete(), exception);
        }

        public void Warning(int eventId, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, null, eventId, null, properties, null);
        }

        public void Warning(int eventId, object properties)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, null, eventId, null, new ExtendProperties(properties).Complete(), null);
        }

        public void Warning(int eventId, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, null, eventId, null, properties.Complete(), null);
        }

        public void Warning(int eventId, Exception exception)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, null, eventId, null, null, exception);
        }

        public void Warning(int eventId, Exception exception, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, null, eventId, null, properties, null);
        }

        public void Warning(int eventId, Exception exception, object properties)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, null, eventId, null, new ExtendProperties(properties).Complete(), exception);
        }

        public void Warning(int eventId, Exception exception, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Warning)
                Add(LogLevel.Warning, null, eventId, null, properties.Complete(), exception);
        }

        public void Trace(string message)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, message, 0, null, null, null);
        }

        public void Trace(string message, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, message, 0, null, properties, null);
        }

        public void Trace(string message, object properties)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, message, 0, null, new ExtendProperties(properties).Complete(), null);
        }

        public void Trace(string message, ExtendProperties collect)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, message, 0, null, collect.Complete(), null);
        }

        public void Trace(string message, string code)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, message, 0, code, null, null);
        }

        public void Trace(string message, string code, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, message, 0, code, properties, null);
        }

        public void Trace(string message, string code, object properties)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, message, 0, code, new ExtendProperties(properties).Complete(), null);
        }

        public void Trace(string message, string code, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, message, 0, code, properties.Complete(), null);
        }

        public void Trace(int eventId, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, null, eventId, null, properties, null);
        }

        public void Trace(int eventId, object properties)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, null, eventId, null, new ExtendProperties(properties).Complete(), null);
        }

        public void Trace(int eventId, ExtendProperties collect)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, null, eventId, null, collect.Complete(), null);
        }

        public void Trace(int eventId, string code)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, null, eventId, code, null, null);
        }

        public void Trace(int eventId, string code, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, null, eventId, code, properties, null);
        }

        public void Trace(int eventId, string code, object properties)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, null, eventId, code, new ExtendProperties(properties).Complete(), null);
        }

        public void Trace(int eventId, string code, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Trace)
                Add(LogLevel.Trace, null, eventId, code, properties.Complete(), null);
        }

        public void Info(string message)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, message, 0, null, null, null);
        }

        public void Info(string message, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, message, 0, null, properties, null);
        }

        public void Info(string message, object properties)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, message, 0, null, new ExtendProperties(properties).Complete(), null);
        }

        public void Info(string message, ExtendProperties collect)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, message, 0, null, collect.Complete(), null);
        }

        public void Info(string message, string code)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, message, 0, code, null, null);
        }

        public void Info(string message, string code, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, message, 0, code, properties, null);
        }

        public void Info(string message, string code, object properties)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, message, 0, code, new ExtendProperties(properties).Complete(), null);
        }

        public void Info(string message, string code, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, message, 0, code, properties.Complete(), null);
        }

        public void Info(int eventId, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, null, eventId, null, properties, null);
        }

        public void Info(int eventId, object properties)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, null, eventId, null, new ExtendProperties(properties).Complete(), null);
        }

        public void Info(int eventId, ExtendProperties collect)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, null, eventId, null, collect.Complete(), null);
        }

        public void Info(int eventId, string code)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, null, eventId, code, null, null);
        }

        public void Info(int eventId, string code, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, null, eventId, code, properties, null);
        }

        public void Info(int eventId, string code, object properties)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, null, eventId, code, new ExtendProperties(properties).Complete(), null);
        }

        public void Info(int eventId, string code, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Info)
                Add(LogLevel.Info, null, eventId, code, properties.Complete(), null);
        }

        public void Error(string message, Exception exception)
        {
            if (LowLevel <= LogLevel.Error)
                Add(LogLevel.Error, message, 0, null, null, exception);
        }

        public void Error(string message, Exception exception, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Error)
                Add(LogLevel.Error, message, 0, null, properties, exception);
        }

        public void Error(string message, Exception exception, object properties)
        {
            if (LowLevel <= LogLevel.Error)
                Add(LogLevel.Error, message, 0, null, new ExtendProperties(properties).Complete(), exception);
        }

        public void Error(string message, Exception exception, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Error)
                Add(LogLevel.Error, message, 0, null, properties.Complete(), exception);
        }

        public void Error(int eventId, Exception exception)
        {
            if (LowLevel <= LogLevel.Error)
                Add(LogLevel.Error, null, eventId, null, null, exception);
        }

        public void Error(int eventId, Exception exception, IDictionary<string, object> properties)
        {
            if (LowLevel <= LogLevel.Error)
                Add(LogLevel.Error, null, eventId, null, properties, exception);
        }

        public void Error(int eventId, Exception exception, object properties)
        {
            if (LowLevel <= LogLevel.Error)
                Add(LogLevel.Error, null, eventId, null, new ExtendProperties(properties).Complete(), exception);
        }

        public void Error(int eventId, Exception exception, ExtendProperties properties)
        {
            if (LowLevel <= LogLevel.Error)
                Add(LogLevel.Error, null, eventId, null, properties.Complete(), exception);
        }

        static void bufer_OnFlush(object sender, BufferedFlushEventArgs<LogEntry> e)
        {
            try
            {
                var filters = LoggingSettings.Settings.GetFilters();
                var writers = LoggingSettings.Settings.GetWriters();
                foreach (var writer in writers)
                {
                    var filter = filters.FirstOrDefault(x => x.Name == writer.FilterName);
                    if (filter != null)
                    {
                        var logs = filter.Filter(e.List);
                        writer.Write(logs);
                    }
                    else
                    {
                        writer.Write(e.List);
                    }
                }
            }
            catch (Exception ex)
            {
                GetLog(LoggingSettings.Settings.LogCategory).Error("日志输出至存储介质时失败。", ex);
                if (OnFail != null)
                    OnFail(null, new LogEventArgs<LogEntry>(e.List));
            }
        }

    }

    public class LogEventArgs<T> : EventArgs
    {
        public IList<T> Logs { get; private set; }

        public LogEventArgs(IList<T> logs)
        {
            this.Logs = logs;
        }
    }
}
