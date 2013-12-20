using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Query;
using Sparrow.CommonLibrary.Database.SqlBuilder;

namespace Sparrow.CommonLibrary.Database
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DatabaseHelper
    {
        #region ExecuteTextFirst

        /// <summary>
        /// 执行sql语句，将结果集的第一行转换成类型<typeparamref name="T"/>实例对象并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <returns>返回类型<typeparamref name="T"/>的实例，如果数据库无返回的结集，则返回空。</returns>
        public T ExecuteFirst<T>(string commandText)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, (ParameterCollection)null))
            {
                return Map.Single<T>(reader);
            }
        }

        /// <summary>
        /// 执行sql语句，将结果集的第一行转换成类型<typeparamref name="T"/>实例对象并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <returns>返回类型<typeparamref name="T"/>的实例，如果数据库无返回的结集，则返回空。</returns>
        public T ExecuteFirst<T>(string commandText, ParameterCollection parameters)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, parameters))
            {
                return Map.Single<T>(reader);
            }
        }

        /// <summary>
        /// 执行sql语句，将结果集的第一行转换成类型<typeparamref name="T"/>实例对象并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <returns>返回类型<typeparamref name="T"/>的实例，如果数据库无返回的结集，则返回空。</returns>
        public T ExecuteFirst<T>(string commandText, params object[] parameters)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, parameters))
            {
                return Map.Single<T>(reader);
            }
        }

        /// <summary>
        /// 执行sql语句，将结果集的第一行转换成类型<typeparamref name="T"/>实例对象并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>返回类型<typeparamref name="T"/>的实例，如果数据库无返回的结集，则返回空。</returns>
        public T ExecuteFirst<T>(string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, parameters, transaction))
            {
                return Map.Single<T>(reader);
            }
        }

        /// <summary>
        /// 执行sql语句，将结果集的第一行转换成类型<typeparamref name="T"/>实例对象并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>返回类型<typeparamref name="T"/>的实例，如果数据库无返回的结集，则返回空。</returns>
        public T ExecuteFirst<T>(string commandText, DbTransaction transaction, params object[] parameters)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, parameters, transaction))
            {
                return Map.Single<T>(reader);
            }
        }

        #endregion

        #region ExecuteTextList

        /// <summary>
        /// 执行sql语句，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> ExecuteList<T>(string commandText)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, (ParameterCollection)null))
            {
                return Map.List<T>(reader);
            }
        }

        /// <summary>
        /// 执行sql语句，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> ExecuteList<T>(string commandText, ParameterCollection parameters)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, parameters))
            {
                return Map.List<T>(reader);
            }
        }

        /// <summary>
        /// 执行sql语句，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> ExecuteList<T>(string commandText, params object[] parameters)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, parameters))
            {
                return Map.List<T>(reader);
            }
        }

        /// <summary>
        /// 执行sql语句，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> ExecuteList<T>(string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, parameters, transaction))
            {
                return Map.List<T>(reader);
            }
        }

        /// <summary>
        /// 执行sql语句，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">sql语句</param>
        /// <param name="parameters">sql语句的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> ExecuteList<T>(string commandText, DbTransaction transaction, params object[] parameters)
        {
            using (var reader = ExecuteReader(CommandType.Text, commandText, parameters, transaction))
            {
                return Map.List<T>(reader);
            }
        }

        #endregion

        #region ExecuteSprocList

        /// <summary>
        /// 执行存储过程，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">存储过程</param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> SprocExecuteList<T>(string commandText)
        {
            using (var reader = ExecuteReader(CommandType.StoredProcedure, commandText, (ParameterCollection)null))
            {
                return Map.List<T>(reader);
            }
        }

        /// <summary>
        /// 执行存储过程，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">存储过程</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> SprocExecuteList<T>(string commandText, ParameterCollection parameters)
        {
            using (var reader = ExecuteReader(CommandType.StoredProcedure, commandText, parameters))
            {
                return Map.List<T>(reader);
            }
        }

        /// <summary>
        /// 执行存储过程，将结果集转换成IList&lt;<typeparamref name="T"/>&gt; 集合，并返回。
        /// </summary>
        /// <typeparam name="T"><typeparamref name="T"/></typeparam>
        /// <param name="commandText">存储过程</param>
        /// <param name="parameters">存储过程的参数</param>
        /// <param name="transaction"> </param>
        /// <returns>IList&lt;<typeparamref name="T"/>&gt; 集合</returns>
        public IList<T> SprocExecuteList<T>(string commandText, ParameterCollection parameters, DbTransaction transaction)
        {
            using (var reader = ExecuteReader(CommandType.StoredProcedure, commandText, parameters, transaction))
            {
                return Map.List<T>(reader);
            }
        }

        #endregion

    }
}
