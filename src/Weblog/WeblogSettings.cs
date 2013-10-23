using Sparrow.CommonLibrary.Logging;
using Sparrow.CommonLibrary.Weblog.Configuration;
using Sparrow.CommonLibrary.Weblog.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog
{
    internal static class WeblogSettings
    {
        public static string Version { get; private set; }

        private static string[] _items;
        public static string[] Items
        {
            get
            {
                var dest = new string[_items.Length];
                Array.Copy(_items, 0, dest, 0, _items.Length);
                return dest;
            }
        }

        private static string[] _ignores;
        public static string[] Ignores
        {
            get
            {
                var dest = new string[_ignores.Length];
                Array.Copy(_ignores, 0, dest, 0, _ignores.Length);
                return dest;
            }
        }

        public static Type Writer { get; private set; }

        public static IDictionary<string, string> WriterParameters { get; private set; }

        static WeblogSettings()
        {
            try
            {
                var configuration = Configuration.WeblogConfigurationSection.GetSection();
                if (configuration == null)
                {
                    return;
                }
                //
                Version = configuration.Version;
                //
                _items = configuration.Collect.Value.Split(',');
                //
                var ignoreList = new List<string>();
                foreach (IgnoreElement ignore in configuration.Ignores)
                    if (!string.IsNullOrWhiteSpace(ignore.Match))
                        ignoreList.Add(ignore.Match);
                //
                if (!configuration.Writer.Type.GetInterfaces().Any(x => x == typeof(IWeblogWriter)))
                    throw new System.Configuration.ConfigurationErrorsException("节点writer包含的type未实现IWeblogWriter接口");
                Writer = configuration.Writer.Type;
                //
                WriterParameters = new Dictionary<string, string>();
                foreach (ParamElement param in configuration.Writer.Params)
                    WriterParameters[param.Name] = param.Value;
            }
            catch (Exception ex)
            {
                Log.GetLog(LoggingSettings.SparrowCategory).Error("载入Weblog配置失败。", ex);
                //
                Version = "0.0.0.1";
                _items = new string[] { "req_type", "domain", "absolute_path", "query_string", "user_agent", "url_referrer", "status_code", "server_host", "user_host", "visit_time", "load_time" };
                _ignores = new string[0];
                Writer = typeof(TextWeblogWriter);
                WriterParameters = new Dictionary<string, string>();
                WriterParameters[TextWeblogWriter.FolderParamName] = "%appdir%\\sprweblog\\%year%_%month%\\%day%weblog.log";
                WriterParameters[TextWeblogWriter.MaxSizeParamName] = "8MB";
                //
                Logging.Log.GetLog(LoggingSettings.SparrowCategory).Info("Weblog启用默认配置。", ex);
            }
        }
    }
}
