using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Data.Import
{
    /// <summary>
    /// 适用于SqlServer数据库的导入工具
    /// </summary>
    public class SqlServerImporter : Importer
    {
        private readonly IDictionary<string, string> _columnMappings;
        private readonly DbCommon.ICommandExecuter _executer;

        public SqlServerImporter(DbCommon.ICommandExecuter executer)
        {
            _columnMappings = new Dictionary<string, string>();
            _executer = executer;
        }

        public int Timeout
        {
            get;
            set;
        }

        public string DestTableName
        {
            get;
            set;
        }

        public IDictionary<string, string> ColumnMappings
        {
            get { return _columnMappings; }
        }

        public bool Write(System.Data.DataTable dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");
            if (string.IsNullOrEmpty(DestTableName))
                throw new ArgumentNullException("未设置DestTableName");
            if (_columnMappings.Count == 0)
                throw new ArgumentNullException("未设置ColumnMappings");

            using (var conn = _executer.GetNotWrapperedConnection())
            {
                using (SqlBulkCopy sqlBulk = CreateSqlBulkCopy((SqlConnection)conn))
                {
                    sqlBulk.WriteToServer(dataSource);
                }
            }
            return true;
        }

        public bool Write(System.Data.IDataReader dataSource)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");
            if (string.IsNullOrEmpty(DestTableName))
                throw new ArgumentNullException("未设置DestTableName");
            if (_columnMappings.Count == 0)
                throw new ArgumentNullException("未设置ColumnMappings");

            using (var conn = _executer.GetNotWrapperedConnection())
            {
                using (SqlBulkCopy sqlBulk = CreateSqlBulkCopy((SqlConnection)conn))
                {
                    sqlBulk.WriteToServer(dataSource);
                }
            }
            return true;
        }

        private SqlBulkCopy CreateSqlBulkCopy(SqlConnection conn)
        {
            SqlBulkCopy sqlBulk = new SqlBulkCopy(conn);
            sqlBulk.BulkCopyTimeout = Timeout;
            foreach (var keyVal in _columnMappings)
            {
                sqlBulk.ColumnMappings.Add(keyVal.Key, keyVal.Value);
            }
            return sqlBulk;
        }
    }
}
