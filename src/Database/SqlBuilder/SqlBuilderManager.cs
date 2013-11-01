using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.SqlBuilder
{
    /// <summary>
    /// 提供获取<see cref="ISqlBuilder"/>接口的实例
    /// </summary>
    public static class SqlBuilderManager
    {
        private readonly static ConcurrentDictionary<string, ISqlBuilder> builders;

        static SqlBuilderManager()
        {
            builders = new ConcurrentDictionary<string, ISqlBuilder>();
            builders["provider:system.Data.SqlClient"] = SqlServerStatementBuilder.Default;
            builders["provider:System.Data.oracleclient"] = OracleStatementBuilder.Default;
            builders["provider:System.Data.mysqlclient"] = MySqlStatementBuilder.Default;
        }

        /// <summary>
        /// 设置链接字符串对应的<see cref="ISqlBuilder"/>实例。
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="builder"></param>
        public static void SetSqlBuilderByConnName(string connectionName, ISqlBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
                throw new ArgumentNullException("connectionName");
            if (builder == null)
                throw new ArgumentNullException("builder");

            builders[string.Concat("conn:", connectionName.ToLower())] = builder;
        }

        /// <summary>
        /// 设置数据库驱动对应的<see cref="ISqlBuilder"/>实例。
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="builder"></param>
        public static void SetSqlBuilderByProvider(string providerName, ISqlBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentNullException("providerName");
            if (builder == null)
                throw new ArgumentNullException("builder");

            builders[string.Concat("provider:", providerName.ToLower())] = builder;
        }

        /// <summary>
        /// 获取<see cref="ISqlBuilder"/>的实力对象。
        /// </summary>
        /// <param name="connectionName">链接字符串名称</param>
        /// <param name="providerName">受<see cref="System.Data.Common.DbProviderFactory"/>管理的提供程序名称。</param>
        /// <returns></returns>
        public static ISqlBuilder GetSqlBuilder(string connectionName, string providerName)
        {
            ISqlBuilder builder;

            builder = CreateSqlBuilderFromConfig(connectionName);
            if (builder != null)
                return builder;

            builder = CreateSqlBuilderFromConfig(providerName);
            if (builder != null)
                return builder;

            if (builders.TryGetValue("conn:" + connectionName.ToLower(), out builder))
                return builder;

            if (builders.TryGetValue("provider:" + providerName.ToLower(), out builder))
                return builder;

            throw new NotSupportedException(string.Format("不受支持的：{0}/{1}", connectionName, providerName));
        }

        private static ISqlBuilder CreateSqlBuilderFromConfig(string name)
        {
            var configuration = Configuration.DatabaseConfigurationSection.GetSection();
            if (configuration != null)
            {
                var providerElement = configuration.Providers[name];
                if (providerElement != null && providerElement.Builder != null)
                {
                    try
                    {
                        return (ISqlBuilder)Activator.CreateInstance(providerElement.Builder.Type);
                    }
                    catch (Exception ex)
                    {
                        throw new ConfigurationErrorsException(string.Format("data/providers[name={0}]/builder[type]未继承指定类型:{1}.", name, typeof(ISqlBuilder).FullName), ex);
                    }
                }
            }
            return null;
        }
    }
}
