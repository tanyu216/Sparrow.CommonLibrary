using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Sparrow.CommonLibrary.Utility;
using Sparrow.CommonLibrary.Data.Database;

namespace Sparrow.CommonLibrary.Logging.Writer
{
    public class DbLogWriter : LogWriterBase
    {
        private const string ParamConnName = "connname";
        private const string ParamTableName = "tablename";
        private const string ParamColumns = "columns";
        private const string ParamSql = "sql";

        public DbLogWriter()
        {
        }

        #region implement

        public override void Write(IList<LogEntry> logs)
        {
            var sql = BuildInsertSql();

            if (logs.Count <= 2000)
            {
                OutputToDb(logs, logs.Count, sql);
            }
            else
            {
                var max = Math.Ceiling((double)(logs.Count / 2000));
                for (var i = 0; i < max; i++)
                {
                    var count = i + 1 == max ? logs.Count % 2000 : 2000;
                    OutputToDb(logs.Skip(i * 2000).Take(count), count, sql);
                }
            }
        }

        #endregion

        protected virtual string BuildInsertSql()
        {
            string tableName;
            string columns;
            string values;
            // 自定SQL
            var sql = GetParameter(ParamSql);
            if (!string.IsNullOrWhiteSpace(sql))
            {
                return sql;
            }
            // 自定义表名
            tableName = GetParameter(ParamTableName);
            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = "SprLog";
            }
            // 自定义需要的字段
            if (!string.IsNullOrWhiteSpace(GetParameter(ParamColumns)))
            {
                var cols = GetParameter(ParamColumns).Split(',');
                var dic = new Dictionary<string, string>() { { "Message", "{0}" }, { "LogLevel", "{1}" }, { "Categories", "{2}" }, { "EventId", "{3}" }, { "Code", "{4}" }, { "UserId", "{5}" }, { "UnixTimestamp", "{6}" }, { "ExtendProperties", "{7}" }, { "Exception", "{8}" }, { "Machine", "{9}" }, { "ThreadId", "{10}" }, { "ThreadName", "{11}" }, { "ProcessId", "{12}" }, { "ProcessName", "{13}" }, { "AppDomainId", "{14}" }, { "AppDomainName", "{15}" } };
                var colList = new List<string>();
                var valueList = new List<string>();
                foreach (var col in cols)
                {
                    string value;
                    if (!dic.TryGetValue(col, out value))
                        continue;//字段不存在，直接忽略

                    colList.Add(col);
                    valueList.Add(value);
                }
                columns = string.Join(",", colList);
                values = string.Join(",", valueList);
            }
            else
            {
                // 默认的字段
                columns = "Message,LogLevel,Categories,EventId,Code,UserId,UnixTimestamp,ExtendProperties,Exception,Machine";
                values = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}";
            }

            return string.Format("INSERT INTO {0}({1})VALUES({2})", tableName, columns, values);
        }

        protected virtual void OutputToDb(IEnumerable<LogEntry> logs, int count, string sql)
        {
            var connName = GetParameter(ParamConnName);
            if (string.IsNullOrWhiteSpace(connName))
                throw new System.Configuration.ConfigurationErrorsException("配置缺少参数：" + connName);

            var helper = DatabaseHelper.GetHelper(connName);
            var batch = new SqlBatch(count);

            foreach (var entry in logs)
            {
                batch.AppendFormat(sql, entry.Message, entry.Level, string.Join(",", entry.Categories ?? new string[0]), entry.EventId, entry.Code, entry.UserId, entry.Timestamp, PropertiesSerializer(entry.Properties), ExceptionSerializer(entry.Exception), entry.Machine, entry.ThreadId, entry.ThreadName, entry.ProcessId, entry.ProcessName, entry.AppDomainId, entry.AppDomainName);
            }
            helper.ExecuteNonQuery(batch);
        }
    }
}
