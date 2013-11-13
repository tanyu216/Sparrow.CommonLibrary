using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Database.DbCommon;

namespace Sparrow.CommonLibrary.Database.Import
{
    /// <summary>
    /// 提供获取<see cref="Importer"/>接口的实例
    /// </summary>
    public static class ImporterManager
    {
        static readonly ConcurrentDictionary<string, Type> importers;

        static ImporterManager()
        {
            importers = new ConcurrentDictionary<string, Type>();
            importers["provider:system.data.sqlclient"] = typeof(ImporterForSqlServer);
        }

        public static void SetImporterType(string providerName, Type type)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException("providerName");
            if (type == null)
                throw new ArgumentNullException("type");
            if (!type.GetInterfaces().Any(x => x == typeof(Importer)))
                throw new ArgumentException(string.Format("{0}未实现接口:{1}", type.FullName, typeof(Importer).FullName));

            importers["provider:" + providerName.ToLower()] = type;
        }

        public static Importer Create(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
                throw new ArgumentNullException("connectionName");

            return Create(ExecuterManager.Create(connectionName));
        }

        public static Importer Create(string connectionString, string providerName)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException("providerName");

            return Create(ExecuterManager.Create(connectionString, providerName));
        }

        public static Importer Create(ICommandExecuter executer)
        {
            if (executer == null)
                throw new ArgumentNullException("executer");

            var type = GetImporterTypeFromConfig(executer.ConnName);
            if (type != null)
                return CreateImporter(type, executer);

            type = GetImporterTypeFromConfig(executer.DbProvider.ProviderName);
            if (type != null)
                return CreateImporter(type, executer);

            if (importers.TryGetValue("provider:" + executer.DbProvider.ProviderName.ToLower(), out type))
                return CreateImporter(type, executer);

            throw new NotSupportedException(string.Format("{0}不支持数据导入。", executer.DbProvider.ProviderName));
        }

        private static Type GetImporterTypeFromConfig(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var configuration = Configuration.DatabaseConfigurationSection.GetSection();
            if (configuration != null)
            {
                var providerElement = configuration.Providers[name];
                if (providerElement != null && providerElement.Importer != null)
                {
                    return providerElement.Importer.Type;
                }
            }
            return null;
        }

        private static Importer CreateImporter(Type type, ICommandExecuter executer)
        {
            var importer = (Importer)Activator.CreateInstance(type, executer);
            return importer;
        }

        public static Importer CreateImporter(this DatabaseHelper database)
        {
            return Create(database.Executer);
        }
    }
}
