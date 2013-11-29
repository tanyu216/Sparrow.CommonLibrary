using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Database.DbCommon;

namespace Sparrow.CommonLibrary.Database
{
    /// <summary>
    /// 提供获取<see cref="Importer"/>接口的实例
    /// </summary>
    public static class ImporterManager
    {
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

            throw new NotSupportedException(string.Format("{0}不支持数据导入工具。", executer.DbProvider.ProviderName));
        }

        private static Type GetImporterTypeFromConfig(string name)
        {
            if (!string.IsNullOrEmpty(name))
                return Configuration.DatabaseSettings.Settings.GetImporterType(name);

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
