using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Database.DbCommon;

namespace Sparrow.CommonLibrary.Database
{
    /// <summary>
    /// 批量数据导入工具
    /// </summary>
    public interface Importer
    {
        /// <summary>
        /// 导入目标数据库的表名
        /// </summary>
        string DestTableName { get; set; }
        /// <summary>
        /// 批量数据导入超时时间（单位：毫秒）
        /// </summary>
        int Timeout { get; set; }
        /// <summary>
        /// 数据源字段映射目标字段映射
        /// </summary>
        IDictionary<string, string> ColumnMappings { get; }
        /// <summary>
        /// 执行导入操作
        /// </summary>
        /// <param name="dataSource"></param>
        bool Write(DataTable dataSource);
        /// <summary>
        /// 执行导入操作
        /// </summary>
        /// <param name="dataSource"></param>
        bool Write(IDataReader dataSource);
    }
}
