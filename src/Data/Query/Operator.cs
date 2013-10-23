using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Data.Query
{
    /// <summary>
    /// 运算符
    /// </summary>
    public enum Operator : byte
    {
        /// <summary>
        /// 等于。示例：col = 1
        /// </summary>
        Equal = 0,
        /// <summary>
        /// 小于。示例：col &lt; 1
        /// </summary>
        Less = 1,
        /// <summary>
        /// 小于或等于。示例：col &lt;= 1
        /// </summary>
        LessEqual = 2,
        /// <summary>
        /// 大于。示例：col &gt; 1
        /// </summary>
        Greater = 3,
        /// <summary>
        /// 大于或等于。示例：col &gt;= 1
        /// </summary>
        GreaterEqual = 4,
        /// <summary>
        /// 包含。示例：col in (1,2,3,4,5)
        /// </summary>
        In = 5,
        /// <summary>
        /// 模糊查找。示例：col like 'abcdef%'
        /// </summary>
        StartWith = 6,
        /// <summary>
        /// 模糊查找。示例：col like '%abcdef'
        /// </summary>
        EndWith = 7,
        /// <summary>
        /// 模糊查找。示例：col like '%abcdef%',fuzzy
        /// </summary>
        All = 8,
        /// <summary>
        /// 不等于。示例：col&lt;&gt;1
        /// </summary>
        NotEqual = 9,
        /// <summary>
        /// 区间。示例：col BETWEEN 1 AND 10
        /// </summary>
        Between = 10
    }
}
