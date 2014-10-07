using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Common
{
    /// <summary>
    /// 将DbValue转换为程序要求的数据类型，当值是一个空值(null/DBNull)时返回类型的默认值。
    /// </summary>
    public static class DbValueCast
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Cast<T>(object dbValue)
        {
            if (dbValue is T)
                return (T)dbValue;
            if (dbValue == null || dbValue == DBNull.Value)
                return default(T);

            try
            {
                return (T)dbValue;
            }
            catch
            {
                return (T)Convert.ChangeType(dbValue, typeof(T));
            }
        }
    }
}
