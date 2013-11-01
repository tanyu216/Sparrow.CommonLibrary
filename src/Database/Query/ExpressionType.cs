using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Database.Query
{
    /// <summary>
    /// 表达式类型
    /// </summary>
    public enum ExpressionType
    {
        /// <summary>
        /// 加法运算
        /// </summary>
        Add = 0,
        /// <summary>
        /// 条件与
        /// </summary>
        AndAlso,
        /// <summary>
        /// 介于两者之间
        /// </summary>
        Between,
        /// <summary>
        /// 集合
        /// </summary>
        Collection,
        /// <summary>
        /// 常量
        /// </summary>
        Constant,
        /// <summary>
        /// 包含集合
        /// </summary>
        In,
        /// <summary>
        /// 除法运算
        /// </summary>
        Divide,
        /// <summary>
        /// 等于比较运算
        /// </summary>
        Equal,
        /// <summary>
        /// 函数
        /// </summary>
        Function,
        /// <summary>
        /// 大于运算
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 大于或等于运算
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// 判断不为空
        /// </summary>
        IsNotNull,
        /// <summary>
        /// 判断空
        /// </summary>
        IsNull,
        /// <summary>
        /// 小于运算
        /// </summary>
        LessThan,
        /// <summary>
        /// 小于或等于运算
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// 类似比较
        /// </summary>
        Like,
        /// <summary>
        /// 取模运算
        /// </summary>
        Modulo,
        /// <summary>
        /// 除法运算
        /// </summary>
        Multiply,
        /// <summary>
        /// 不等于运算
        /// </summary>
        NotEqual,
        /// <summary>
        /// 空
        /// </summary>
        Null,
        /// <summary>
        /// 条件或
        /// </summary>
        OrElse,
        /// <summary>
        /// 参数
        /// </summary>
        Parameter,
        /// <summary>
        /// SQL查询
        /// </summary>
        Query,
        /// <summary>
        /// 减法运算
        /// </summary>
        Subtract,
        /// <summary>
        /// 数据库表
        /// </summary>
        Table,
        /// <summary>
        /// 表的成员字段
        /// </summary>
        TableField,
        /// <summary>
        /// 变量名称
        /// </summary>
        VariableName
    }
}
