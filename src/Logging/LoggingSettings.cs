using Sparrow.CommonLibrary.Logging.Configuration;
using Sparrow.CommonLibrary.Logging.Filter;
using Sparrow.CommonLibrary.Logging.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Logging
{
    public static class LoggingSettings
    {
        /// <summary>
        /// Sparrow.CommonLibrary日志分类。
        /// </summary>
        public static string SparrowCategory { get; private set; }

        /// <summary>
        /// 日志默认的分类。
        /// </summary>
        public static string DefaultCategory { get; private set; }

        private static Type defaultWriterType;
        private static Dictionary<string, string> defaultWriterParameters;

        static LoggingSettings()
        {
            SparrowCategory = "sprlogging";
            DefaultCategory = "default";
            //
            defaultWriterType = typeof(TextLogWriter);
            defaultWriterParameters = new Dictionary<string, string>();
            defaultWriterParameters.Add(TextLogWriter.FolderParamName, "%appdir%/sprlog/%year%%month%//%day%log.log");
            defaultWriterParameters.Add(TextLogWriter.MaxSizeParamName, "8MB");
            //
            filters = new List<ILogFilter>();
            writers = new List<ILogWriter>();
            //
            LoadFromConfig();
        }

        private static void LoadFromConfig()
        {
            var configuration = LoggingConfigurationSection.GetSection();
            if (configuration == null)
                return;
            //
            lock (filters)
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
                        Log.GetLog(LoggingSettings.SparrowCategory).Error("加载日志ILogFilter错误。", ex, new { Name = filterElement.Name, Type = filterElement.Type.FullName });
                    }
                }
            }
            //
            lock (writers)
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
                        Log.GetLog(LoggingSettings.SparrowCategory).Error("加载日志ILogWriter错误。", ex, new { Name = writerElement.Name, Type = writerElement.Type.FullName });
                    }
                }
            }
        }

        private static readonly IList<ILogFilter> filters;

        public static void AddFilter(ILogFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");
            lock (filters)
            {
                filters.Add(filter);
            }
        }

        private static readonly IList<ILogWriter> writers;

        public static void AddWriter(ILogWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            lock (writers)
            {
                writers.Add(writer);
            }
        }

        internal static IList<ILogFilter> GetFilters()
        {
            return filters.ToList();
        }

        internal static IList<ILogWriter> GetWriters()
        {
            if (writers.Count > 0)
            {
                return writers.ToList();
            }

            lock (writers)
            {
                if (writers.Count > 0)
                    return writers.ToList();

                var writer = (ILogWriter)Activator.CreateInstance(defaultWriterType);
                foreach (var keyVal in defaultWriterParameters)
                {
                    writer.AddParameter(keyVal.Key, keyVal.Value);
                }
                writers.Add(writer);

                return new List<ILogWriter>() { writer };
            }
        }
    }
}
