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
            _databaseTypes = new ConcurrentDictionary<string, Type>();
            _sqlbuilderInstances = new ConcurrentDictionary<string, ISqlBuilder>();
        }

        public DatabaseSettings(DatabaseConfigurationSection configuration)
            : this()
        {
            LoadConfig(configuration);
        }

        public void LoadConfig()
        {
            var configuration = DatabaseConfigurationSection.GetSection();
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

                if (element.Database != null)
                    _databaseTypes[element.Name] = element.Database.Type;

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

            switch (name)
            {
                case "System.Data.SqlClient":
                    return SqlBuilder.SqlServerStatementBuilder.Default;
                case "System.Data.MySqlClient":
                    return SqlBuilder.MySqlStatementBuilder.Default;
                case "System.Data.OracleClient":
                    return SqlBuilder.OracleStatementBuilder.Default;
            }
            return null;
        }

        private ConcurrentDictionary<string, Type> _databaseTypes;

        public void SetDatabaseHelperType(string name, Type databaseType)
        {
            if (databaseType == null)
                throw new ArgumentNullException("databaseType");

            _databaseTypes[name] = databaseType;
        }

        public Type GetDatabaseHelperType(string name)
        {
            Type databaseType;

            if (_databaseTypes.TryGetValue(name, out databaseType))
                return databaseType;

            return null;
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
