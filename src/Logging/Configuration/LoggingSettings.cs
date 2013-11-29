using Sparrow.CommonLibrary.Logging.Configuration;
using Sparrow.CommonLibrary.Logging.Filter;
using Sparrow.CommonLibrary.Logging.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Logging.Configuration
{
    public class LoggingSettings
    {
        private static readonly object syncObj = new object();

        private static LoggingSettings _settings;

        public static LoggingSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    lock (syncObj)
                    {
                        if (_settings == null)
                        {
                            _settings = new LoggingSettings();
                            _settings.LoadConfig();
                        }
                    }
                }
                return _settings;
            }
        }

        /// <summary>
        /// Sparrow.CommonLibrary日志分类。
        /// </summary>
        internal protected string LogCategory { get; protected set; }

        protected Log Log { get { return Log.GetLog(LogCategory); } }

        /// <summary>
        /// 日志默认的分类。
        /// </summary>
        public string DefaultCategory { get; protected set; }

        /// <summary>
        /// 接受的最低级别的日志
        /// </summary>
        public LogLevel LowLevel { get; protected set; }

        public LoggingSettings()
        {
            LogCategory = "sprlogging";
            DefaultCategory = "default";

            filters = new List<ILogFilter>();
            writers = new List<ILogWriter>();
        }

        public LoggingSettings(LoggingConfigurationSection configuration)
            :this()
        {
            LoadConfig(configuration);
        }

        public void LoadConfig()
        {
            var configuration = LoggingConfigurationSection.GetSection();
            if (configuration == null)
            {
                configuration = new LoggingConfigurationSection();
                configuration.LowLevel = LogLevel.Debug;
                configuration.Writers = new WriterElementCollection();
                var writerElement = new WriterElement() { Name = "writer1", Type = typeof(TextLogWriter), Params = new ParamElementCollection() };
                writerElement.Params.Add(new ParamElement() { Name = TextLogWriter.FolderParamName, Value = "%appdir%/sprlog/%year%%month%//%day%log.log" });
                writerElement.Params.Add(new ParamElement() { Name = TextLogWriter.MaxSizeParamName, Value = "8MB" });
                configuration.Writers.Add(writerElement);
            }

            LoadConfig(configuration);
        }

        public void LoadConfig(LoggingConfigurationSection configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            LowLevel = configuration.LowLevel;

            if (configuration.Filters != null)
            {
                foreach (FilterElement filterElement in configuration.Filters)
                {
                    try
                    {
                        var filter = (ILogFilter)Activator.CreateInstance(filterElement.Type);
                        filter.Name = filterElement.Name;
                        filter.Categories = (filterElement.Categories ?? string.Empty).Split(',').Where(x => x != null && x.Trim() != string.Empty).ToArray();
                        filter.LogLevel = filterElement.LogLevel;
                        filters.Add(filter);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("加载日志ILogFilter错误。", ex, new { Name = filterElement.Name, Type = filterElement.Type.FullName });
                    }
                }
            }

            if (configuration.Writers != null)
            {
                foreach (WriterElement writerElement in configuration.Writers)
                {
                    try
                    {
                        var writer = (ILogWriter)Activator.CreateInstance(writerElement.Type);
                        writer.FilterName = writerElement.FilterName;
                        foreach (ParamElement param in writerElement.Params)
                            writer.AddParameter(param.Name, param.Value);
                        writers.Add(writer);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("加载日志ILogWriter错误。", ex, new { Name = writerElement.Name, Type = writerElement.Type.FullName });
                    }
                }
            }
        }

        private readonly IList<ILogFilter> filters;

        public void AddFilter(ILogFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");
            lock (filters)
            {
                filters.Add(filter);
            }
        }

        public IList<ILogFilter> GetFilters()
        {
            lock (filters)
            {
                return filters.ToList();
            }
        }

        private readonly IList<ILogWriter> writers;

        public void AddWriter(ILogWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            lock (writers)
            {
                writers.Add(writer);
            }
        }

        public IList<ILogWriter> GetWriters()
        {
            if (writers.Count > 0)
            {
                return writers.ToList();
            }

            lock (writers)
            {
                return writers.ToList();
            }
        }

        public static void ResetSettings(LoggingSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            lock (syncObj)
            {
                _settings = settings;
            }
        }
    }
}
