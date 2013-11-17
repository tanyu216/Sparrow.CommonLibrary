using Sparrow.CommonLibrary.Mapper.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Entity
{
    public interface IDbMetaInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// 自动增长序列
        /// </summary>
        IDbIncrementMetaPropertyInfo Increment { get; }

        /// <summary>
        /// 表中主键的数量
        /// </summary>
        int KeyCount { get; }

        /// <summary>
        /// 表中字段的数量
        /// </summary>
        int ColumnCount { get; }
        
        /// <summary>
        /// 验证是否为主键主字段
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        bool IsKey(string columnName);

        /// <summary>
        /// 获取所有的主键字段
        /// </summary>
        /// <returns></returns>
        string[] GetKeys();

        /// <summary>
        /// 获取所有的字段
        /// </summary>
        /// <returns></returns>
        string[] GetColumnNames();
    }
}
