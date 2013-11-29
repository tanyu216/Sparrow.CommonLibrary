using Sparrow.CommonLibrary.Weblog.Collect;
using Sparrow.CommonLibrary.Weblog.Configuration;
using Sparrow.CommonLibrary.Weblog.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Weblog.Configuration
{
    public class WeblogSettings
    {
        private static readonly object syncObj = new object();

        private static WeblogSettings _settings;

        public static WeblogSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    lock (syncObj)
                    {
                        if (_settings == null)
                        {
                            _settings = new WeblogSettings();
                            _settings.LoadConfig();
                        }
                    }
                }
                return _settings;
            }
        }

        public string Version { get; protected set; }

        private string[] _collecters;
        public string[] Collecters
        {
            get
            {
                var dest = new string[_collecters.Length];
                Array.Copy(_collecters, 0, dest, 0, _collecters.Length);
                return dest;
            }
            protected set { _collecters = value; }
        }

        private string[] _ignores;
        public string[] Ignores
        {
            get
            {
                var dest = new string[_ignores.Length];
                Array.Copy(_ignores, 0, dest, 0, _ignores.Length);
                return dest;
            }
            protected set { _ignores = value; }
        }

        public Type Writer { get; protected set; }

        private IDictionary<string, string> _writerParameters;
        public IDictionary<string, string> WriterParameters
        {
            get { return _writerParameters.ToDictionary(x => x.Key, y => y.Value); }
            protected set { _writerParameters = value; }
        }

        /// <summary>
        /// 用于记录日志的Category
        /// </summary>
        internal protected string LogCategory { get; protected set; }

        protected Logging.Log Log { get { return Logging.Log.GetLog(LogCategory); } }

        public WeblogSettings()
        {
            LogCategory = "sprweblog";
        }

        public void LoadConfig()
        {
            var configuration = Configuration.WeblogConfigurationSection.GetSection();
            if (configuration == null)
            {
                configuration = new WeblogConfigurationSection();
                configuration.Version = "0.1";
                configuration.Collect = new CollectElement() { Value = string.Join(",", new[] { "req_type", "domain", "absolute_path", "query_string", "user_agent", "url_referrer", "status_code", "server_host", "user_host", "visit_time", "load_time" }) };
                configuration.Writer = new WriterElement() { Type = typeof(TextWeblogWriter) };
                configuration.Writer.Params.Add(new ParamElement() { Name = TextWeblogWriter.FolderParamName, Value = "%appdir%/sprweblog/%year%_%month%/%day%weblog.log" });
                configuration.Writer.Params.Add(new ParamElement() { Name = TextWeblogWriter.MaxSizeParamName, Value = "8MB" });
            }

            LoadConfig(configuration);
        }

        public void LoadConfig(WeblogConfigurationSection configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            try
            {
                Version = configuration.Version;
                _collecters = configuration.Collect.Value.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                if (_collecters.Length == 0)
                    throw new System.Configuration.ConfigurationErrorsException(string.Format("Weblog没有配置任何一个可用的采集选项({0})。", typeof(ICollecter).FullName));

                var ignoreList = new List<string>();
                if (configuration.Ignores != null)
                {
                    foreach (IgnoreElement ignore in configuration.Ignores)
                        if (!string.IsNullOrWhiteSpace(ignore.Match))
                            ignoreList.Add(ignore.Match);
                }
                _ignores = ignoreList.ToArray();

                Writer = configuration.Writer.Type;

                WriterParameters = new Dictionary<string, string>();
                if (configuration.Writer.Params != null)
                {
                    foreach (Sparrow.CommonLibrary.Weblog.Configuration.ParamElement param in configuration.Writer.Params)
                        WriterParameters[param.Name] = param.Value;
                }
            }
            catch (Exception ex)
            {
                Log.Error("加载Weblog配置失败。", ex);
                throw ex;
            }
        }
    }
}
