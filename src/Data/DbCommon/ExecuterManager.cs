using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Data.DbCommon
{
    /// <summary>
    /// 提供获取<see cref="Sparrow.CommonLibrary.Data.DbCommon.ICommandExecuter"/>接口的实例
    /// </summary>
    /// <remarks>该对象所有成员均保证是多线程安全的。所有继承自该类的对象都应遵守该约定。</remarks>
    public static class ExecuterManager
    {
        #region Crypt

        private static Func<string, string> decrypter;

        /// <summary>
        /// 自定义链接字符串解密方法
        /// </summary>
        /// <param name="decrypter"></param>
        public static void SetDecrypter(Func<string, string> decrypter)
        {
            ExecuterManager.decrypter = decrypter;
        }

        /// <summary>
        /// 链接字符串解密
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static string Decrypt(string connectionString)
        {
            if (decrypter != null)
                return decrypter(connectionString);
            return connectionString;
        }

        #endregion

        /// <summary>
        /// 新建一个<see cref="Sparrow.CommonLibrary.Data.DbCommon.ICommandExecuter"/>对象。
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public static ICommandExecuter Create(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
                throw new ArgumentNullException("connectionName");

            var connString = ConfigurationManager.ConnectionStrings[connectionName];

            if (connString == null)
                throw new ConfigurationErrorsException(string.Format("未找到{0}连接字符串配置。", connectionName));
            if (string.IsNullOrEmpty(connString.ConnectionString))
                throw new ConfigurationErrorsException(string.Format("{0}未配置连接字符串。", connectionName));
            if (string.IsNullOrEmpty(connString.ProviderName))
                throw new ConfigurationErrorsException("连接字符串未配置providerName。");

            var dbProvider = DbProvider.GetDbProvider(connString.ProviderName);
            
            Type iCommandExecuterType = GetICommandExecuterTypeFromConfig(connectionName);
            if (iCommandExecuterType != null)
                return (ICommandExecuter)Activator.CreateInstance(iCommandExecuterType, connectionName, Decrypt(connString.ConnectionString), dbProvider);
            
            return new CommonExecuter(connectionName, Decrypt(connString.ConnectionString), dbProvider);
        }

        /// <summary>
        /// 新建一个<see cref="Sparrow.CommonLibrary.Data.DbCommon.ICommandExecuter"/>对象。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static ICommandExecuter Create(string connectionString, string providerName)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException("providerName");

            var dbProvider = DbProvider.GetDbProvider(providerName);

            Type iCommandExecuterType = GetICommandExecuterTypeFromConfig(providerName);
            if (iCommandExecuterType != null)
                return (ICommandExecuter)Activator.CreateInstance(iCommandExecuterType, Decrypt(connectionString), dbProvider);

            return new CommonExecuter(null, Decrypt(connectionString), dbProvider);
        }

        private static Type GetICommandExecuterTypeFromConfig(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var configuration = Configuration.DataConfigurationSection.GetSection();
            if (configuration == null)
                return null;

            var providerElement = configuration.Providers[name];
            if (providerElement != null && providerElement.Executer != null)
            {
                return providerElement.Executer.Type;
            }

            return null;
        }

        private static ICommandExecuter CreateCommandExecuter(Type iCommandExecuterType, string connName, string connectionString, string providerName)
        {
            return (ICommandExecuter)Activator.CreateInstance(iCommandExecuterType, connName, connectionString, DbProvider.GetDbProvider(providerName));
        }

    }
}
