using Sparrow.CommonLibrary.Database.DbCommon;
using Sparrow.CommonLibrary.Database.SqlBuilder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.Configuration
{
    public class DatabaseSettings
    {
        private static readonly object syncObj = new object();

        private static DatabaseSettings _settings;

        public static DatabaseSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    lock (syncObj)
                    {
                        if (_settings == null)
                        {
                            _settings = new DatabaseSettings();
                            _settings.LoadConfig();
                        }
                    }
                }
                return _settings;
            }
        }

        public DatabaseSettings()
        {
            _executerTypes = new ConcurrentDictionary<string, Type>();
            _importerTypes = new ConcurrentDictionary<string, Type>();
            _sqlbuilderInstances = new ConcurrentDictionary<string, ISqlBuilder>();
        }

        public DatabaseSettings(DatabaseConfigurationSection configuration)
            :this()
        {
            LoadConfig(configuration);
        }

        public void LoadConfig()
        {
            var configuration = DatabaseConfigurationSection.GetSection();
            if (configuration == null)
            {
                configuration = new DatabaseConfigurationSection();
                configuration.Providers = new ProviderElementCollection();
                configuration.Providers.Add(new ProviderElement() { Name = "System.Data.SqlClient", Executer = new ExecuterElement() { Type = typeof(DbCommon.CommonExecuter) }, Builder = new BuilderElement() { Type = typeof(SqlBuilder.SqlServerStatementBuilder) }, Importer = new ImporterElement() { Type = typeof(ImporterForSqlServer) } });
                configuration.Providers.Add(new ProviderElement() { Name = "System.Data.MySqlClient", Executer = new ExecuterElement() { Type = typeof(DbCommon.CommonExecuter) }, Builder = new BuilderElement() { Type = typeof(SqlBuilder.MySqlStatementBuilder) } });
                configuration.Providers.Add(new ProviderElement() { Name = "System.Data.OracleClient", Executer = new ExecuterElement() { Type = typeof(DbCommon.CommonExecuter) }, Builder = new BuilderElement() { Type = typeof(SqlBuilder.OracleStatementBuilder) } });
            }

            LoadConfig(configuration);
        }

        public void LoadConfig(DatabaseConfigurationSection configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            foreach (ProviderElement element in configuration.Providers)
            {
                if (element.Builder != null)
                    _sqlbuilderInstances[element.Name] = (ISqlBuilder)Activator.CreateInstance(element.Builder.Type);

                if (element.Importer != null)
                    _importerTypes[element.Name] = element.Importer.Type;

                if (element.Executer != null)
                    _executerTypes[element.Name] = element.Executer.Type;
            }
        }

        private ConcurrentDictionary<string, ISqlBuilder> _sqlbuilderInstances;

        public void SetISqlBuilder(string name, ISqlBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");

            _sqlbuilderInstances[name] = builder;
        }

        public ISqlBuilder GetISqlBuilder(string name)
        {
            ISqlBuilder builder;

            if (_sqlbuilderInstances.TryGetValue(name, out builder))
                return builder;

            return null;
        }

        private ConcurrentDictionary<string, Type> _importerTypes;

        public void SetImporterType(string name, Type importer)
        {
            if (importer == null)
                throw new ArgumentNullException("importer");

            _importerTypes[name] = importer;
        }

        public Type GetImporterType(string name)
        {
            Type importer;

            if (_importerTypes.TryGetValue(name, out importer))
                return importer;

            return null;
        }

        private ConcurrentDictionary<string, Type> _executerTypes;

        public void SetICommonExecuterType(string name, Type executer)
        {
            if (executer == null)
                throw new ArgumentNullException("executer");

            _executerTypes[name] = executer;
        }

        public Type GetICommonExecuterType(string name)
        {
            Type executer;

            if (_executerTypes.TryGetValue(name, out executer))
                return executer;

            return typeof(DbCommon.CommonExecuter);
        }

        public static void ResetSettings(DatabaseSettings settings)
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
