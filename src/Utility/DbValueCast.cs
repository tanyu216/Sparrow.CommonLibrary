using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Utility
{
    /// <summary>
    /// 
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
