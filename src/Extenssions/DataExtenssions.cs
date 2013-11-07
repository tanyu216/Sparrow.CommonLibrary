using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Sparrow.CommonLibrary.Utility;

namespace Sparrow.CommonLibrary.Extenssions
{
    public static class DataExtenssions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T Cast<T>(object dbValue)
        {
            return DbValueCast.Cast<T>(dbValue);
        }

        #region DataTable

        private static int IndexOfColumn(this DataColumnCollection cols, string columnName)
        {
            int index = cols.IndexOf(columnName);
            if (index < 0)
                throw new ArgumentException(string.Format("DataTable不包含列名：{0}。", columnName));
            else
                return index;
        }

        /// <summary>
        /// 获取DataTable中第一行列columnName的值，如果DataTable没有数据时，返回<typeparamref name="T"/>的默认值。
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="columnName">要获取的字段</param>
        /// <returns>columnName字段中的数据</returns>
        public static T Scalar<T>(this DataTable tb)
        {
            if (tb.Rows.Count == 0 || tb.Columns.Count == 0)
                return default(T);

            return Cast<T>(tb.Rows[0][0]);
        }

        /// <summary>
        /// 获取DataTable中第一行列columnName的值，如果DataTable没有数据时，返回<typeparamref name="T"/>的默认值。
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="columnName">要获取的字段</param>
        /// <returns>columnName字段中的数据</returns>
        public static T Scalar<T>(this DataTable tb, string columnName)
        {
            if (tb.Rows.Count == 0)
                return default(T);

            return Cast<T>(tb.Rows[0][columnName]);
        }

        /// <summary>
        /// 获取DataTable中第一行列columnName的值，如果DataTable没有数据时，返回<typeparamref name="T"/>的默认值。
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="indexOfColumn">字段索引</param>
        /// <returns></returns>
        public static T Scalar<T>(this DataTable tb, int indexOfColumn)
        {
            if (tb.Rows.Count == 0)
                return default(T);

            return Cast<T>(tb.Rows[0][indexOfColumn]);
        }

        /// <summary>
        /// 获取<see cref="DataTable"/>中的行转换成<see cref="IDictionary"/>对象。
        /// </summary>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="columnNameOfKey">对应Key的列名称</param>
        /// <param name="columnNameOfvalue">对应Value的列名称</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DataTable tb)
        {
            if (tb.Columns.Count < 2)
                throw new ArgumentException("DataTable.Columns少于两个字段。");

            return tb.ToDictionary<TKey, TValue>(0, 1, x => Cast<TKey>(x), y => Cast<TValue>(y));
        }

        /// <summary>
        /// 获取<see cref="DataTable"/>中的行转换成<see cref="IDictionary"/>对象。
        /// </summary>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="columnNameOfKey">对应Key的列名称</param>
        /// <param name="columnNameOfvalue">对应Value的列名称</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DataTable tb, string columnNameOfKey, string columnNameOfvalue)
        {
            int index1 = tb.Columns.IndexOfColumn(columnNameOfKey);
            int index2 = tb.Columns.IndexOfColumn(columnNameOfvalue);
            return tb.ToDictionary<TKey, TValue>(index1, index2, x => Cast<TKey>(x), y => Cast<TValue>(y));
        }

        /// <summary>
        /// 获取<see cref="DataTable"/>中的行转换成<see cref="IDictionary"/>对象。
        /// </summary>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="columnNameOfKey">对应Key的列名称</param>
        /// <param name="columnNameOfvalue">对应Value的列名称</param>
        /// <param name="keyConvert">键转换，自定义<paramref name="columnNameOfKey"/>字段的转换</param>
        /// <param name="valueConvert">值转换，自定义<paramref name="columnNameOfvalue"/>字段的转换</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DataTable tb, string columnNameOfKey, string columnNameOfvalue, Func<object, TKey> keyConvert, Func<object, TValue> valueConvert)
        {
            int index1 = tb.Columns.IndexOfColumn(columnNameOfKey);
            int index2 = tb.Columns.IndexOfColumn(columnNameOfvalue);
            return tb.ToDictionary<TKey, TValue>(index1, index2, keyConvert, valueConvert);
        }

        /// <summary>
        /// 获取<see cref="DataTable"/>中的行转换成<see cref="IDictionary"/>对象。
        /// </summary>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="indexOfKey">对应Key的索引</param>
        /// <param name="indexOfValue">对应Value的索引</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DataTable tb, int indexOfKey, int indexOfValue)
        {
            return tb.ToDictionary<TKey, TValue>(indexOfKey, indexOfValue, x => Cast<TKey>(x), y => Cast<TValue>(y));
        }

        /// <summary>
        /// 获取<see cref="DataTable"/>中的行转换成<see cref="IDictionary"/>对象。
        /// </summary>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="indexOfKey">对应Key的索引</param>
        /// <param name="indexOfValue">对应Value的索引</param>
        /// <param name="keyConvert">键转换，自定义<paramref name="itemColumn"/>字段的转换</param>
        /// <param name="valueConvert">值转换，自定义<paramref name="valueColumn"/>字段的转换</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DataTable tb, int indexOfKey, int indexOfValue, Func<object, TKey> keyConvert, Func<object, TValue> valueConvert)
        {
            if (indexOfKey < 0 || indexOfKey >= tb.Columns.Count)
                throw new IndexOutOfRangeException("indexOfKey下标越界。");

            if (indexOfValue < 0 || indexOfValue >= tb.Columns.Count)
                throw new IndexOutOfRangeException("indexOfValue下标越界。");

            if (tb.Rows.Count == 0)
                return new Dictionary<TKey, TValue>(0);

            var ls = new Dictionary<TKey, TValue>(tb.Rows.Count);
            foreach (DataRow row in tb.Rows)
            {
                ls.Add(keyConvert(row[indexOfKey]), valueConvert(row[indexOfValue]));
            }
            return ls;
        }

        /// <summary>
        /// 将DataTable指定的列以集合的方式取出
        /// </summary>
        /// <typeparam name="T">字段的数据类型</typeparam>
        /// <param name="tb"><see cref="System.Data.DataTable"/></param>
        /// <param name="columnName">字段名称</param>
        /// <returns>columnName字段中的数据集合</returns>
        public static IList<T> GetValues<T>(this DataTable tb)
        {
            if (tb.Columns.Count == 0)
                throw new ArgumentException("DataTable未包含任何字段。");

            return tb.GetValues<T>(0, x => (T)x);
        }

        /// <summary>
        /// 将DataTable指定的列以集合的方式取出
        /// </summary>
        /// <typeparam name="T">字段的数据类型</typeparam>
        /// <param name="tb"><see cref="System.Data.DataTable"/></param>
        /// <param name="columnName">字段名称</param>
        /// <returns>columnName字段中的数据集合</returns>
        public static IList<T> GetValues<T>(this DataTable tb, string columnName)
        {
            return tb.GetValues<T>(tb.Columns.IndexOfColumn(columnName), x => (T)x);
        }

        /// <summary>
        /// 将DataTable指定的列以集合的方式取出
        /// </summary>
        /// <typeparam name="T">字段的数据类型</typeparam>
        /// <param name="tb"><see cref="System.Data.DataTable"/></param>
        /// <param name="columnName">字段名称</param>
        /// <returns>columnName字段中的数据集合</returns>
        public static IList<T> GetValues<T>(this DataTable tb, string columnName, Func<object, T> convert)
        {
            return tb.GetValues<T>(tb.Columns.IndexOfColumn(columnName), convert);
        }

        /// <summary>
        /// 将DataTable指定的列以集合的方式取出
        /// </summary>
        /// <typeparam name="T">字段的数据类型</typeparam>
        /// <param name="tb"><see cref="System.Data.DataTable"/></param>
        /// <param name="indexOfColumn">字段索引</param>
        /// <returns>columnName字段中的数据集合</returns>
        public static IList<T> GetValues<T>(this DataTable tb, int indexOfColumn)
        {
            return tb.GetValues<T>(indexOfColumn, x => Cast<T>(x));
        }

        /// <summary>
        /// 将DataTable指定的列以集合的方式取出
        /// </summary>
        /// <typeparam name="T">字段的数据类型</typeparam>
        /// <param name="tb"><see cref="System.Data.DataTable"/></param>
        /// <param name="indexOfColumn">字段索引</param>
        /// <returns>columnName字段中的数据集合</returns>
        public static IList<T> GetValues<T>(this DataTable tb, int indexOfColumn, Func<object, T> convert)
        {
            if (tb.Rows.Count == 0)
                return new List<T>(0);

            if (indexOfColumn < 0 || indexOfColumn >= tb.Columns.Count)
                throw new IndexOutOfRangeException("indexOfColumn下标越界。");

            List<T> ls = new List<T>(tb.Rows.Count);
            foreach (DataRow row in tb.Rows)
            {
                ls.Add(convert(row[indexOfColumn]));
            }
            return ls;
        }

        #endregion

        #region IDataReader

        private static int IndexOfColumn(this IDataReader reader, string columnName)
        {
            try
            {
                var index = reader.GetOrdinal(columnName);
                if (index < 0)
                    throw new ArgumentException(string.Format("IDataReader不包含列名：{0}。", columnName));
                return index;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("IDataReader不包含列名：{0}。", columnName), ex);
            }
        }

        /// <summary>
        /// 获取IDataReader中第一行列columnName的值，如果IDataReader没有数据时，返回<typeparamref name="T"/>的默认值。
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="columnName">要获取的字段</param>
        /// <returns>columnName字段中的数据</returns>
        public static T Saclar<T>(this IDataReader reader)
        {
            if (reader.FieldCount == 0 || !reader.Read())
                return default(T);

            return Cast<T>(reader[0]);
        }

        /// <summary>
        /// 获取IDataReader中第一行列columnName的值，如果IDataReader没有数据时，返回<typeparamref name="T"/>的默认值。
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="indexOfColumn">要获取的字段</param>
        /// <returns>columnName字段中的数据</returns>
        public static T Saclar<T>(this IDataReader reader, string columnName)
        {
            if (reader.Read())
                return Cast<T>(reader[columnName]);
            else
                return default(T);
        }

        /// <summary>
        /// 获取IDataReader中第一行列columnName的值，如果IDataReader没有数据时，返回<typeparamref name="T"/>的默认值。
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="indexOfColumn">字段索引</param>
        /// <returns>indexOfColumn字段中的数据</returns>
        public static T Saclar<T>(this IDataReader reader, int indexOfColumn)
        {
            if (reader.Read())
                return Cast<T>(reader[indexOfColumn]);
            else
                return default(T);
        }

        /// <summary>
        /// 获取<see cref="IDataReader"/>中的行转换成<see cref="IDictionary"/>对象。
        /// </summary>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDataReader reader)
        {
            if (reader.FieldCount < 2)
                throw new ArgumentException("IDataReader少于两个字段。");

            return reader.ToDictionary<TKey, TValue>(0, 1, x => Cast<TKey>(x), y => Cast<TValue>(y));
        }

        /// <summary>
        /// 获取<see cref="IDataReader"/>中的行转换成<see cref="IDictionary"/>对象。
        /// </summary>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="indexOfColumn">对应Item的列名称</param>
        /// <param name="indexOfValue">对应Value的列名称</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDataReader reader, string columnNameOfKey, string columnNameOfValue)
        {
            int index1 = reader.IndexOfColumn(columnNameOfKey);
            int index2 = reader.IndexOfColumn(columnNameOfValue);
            return reader.ToDictionary<TKey, TValue>(index1, index2, x => Cast<TKey>(x), y => Cast<TValue>(y));
        }

        /// <summary>
        /// 获取<see cref="IDataReader"/>中的行转换成<see cref="IDictionary"/>对象。
        /// </summary>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="indexOfKey">对应Item的列名称</param>
        /// <param name="indexOfValue">对应Value的列名称</param>
        /// <param name="keyConvert">键转换，自定义<paramref name="indexOfKey"/>字段的转换</param>
        /// <param name="valueConvert">值转换，自定义<paramref name="indexOfValue"/>字段的转换</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDataReader reader, string columnNameOfKey, string columnNameOfValue, Func<object, TKey> keyConvert, Func<object, TValue> valueConvert)
        {
            int index1 = reader.IndexOfColumn(columnNameOfKey);
            int index2 = reader.IndexOfColumn(columnNameOfValue);
            return reader.ToDictionary<TKey, TValue>(index1, index2, keyConvert, valueConvert);
        }

        /// <summary>
        /// 获取<see cref="IDataReader"/>中的行转换成<see cref="IDictionary"/>对象。
        /// </summary>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="itemColumn">对应Item的列名称</param>
        /// <param name="valueColumn">对应Value的列名称</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDataReader reader, int indexOfKey, int indexOfValue)
        {
            return reader.ToDictionary<TKey, TValue>(indexOfKey, indexOfValue, x => Cast<TKey>(x), y => Cast<TValue>(y));
        }

        /// <summary>
        /// 获取<see cref="IDataReader"/>中的行转换成<see cref="IDictionary"/>对象。
        /// </summary>
        /// <typeparam name="TKey">键的类型</typeparam>
        /// <typeparam name="TValue">值的类型</typeparam>
        /// <param name="itemColumn">对应Item的列名称</param>
        /// <param name="valueColumn">对应Value的列名称</param>
        /// <param name="keyConvert">键转换，自定义<paramref name="itemColumn"/>字段的转换</param>
        /// <param name="valueConvert">值转换，自定义<paramref name="valueColumn"/>字段的转换</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDataReader reader, int indexOfKey, int indexOfValue, Func<object, TKey> keyConvert, Func<object, TValue> valueConvert)
        {

            if (indexOfKey < 0 || indexOfKey >= reader.FieldCount)
                throw new IndexOutOfRangeException("indexOfKey下标越界。");

            if (indexOfValue < 0 || indexOfValue >= reader.FieldCount)
                throw new IndexOutOfRangeException("indexOfValue下标越界。");

            if (!reader.Read())
                return new Dictionary<TKey, TValue>(0);

            var dic = new Dictionary<TKey, TValue>(reader.RecordsAffected < 1 ? 4 : reader.RecordsAffected);
            do
            {
                dic.Add(keyConvert(reader[indexOfKey]), valueConvert(reader[indexOfValue]));
            } while (reader.Read());
            return dic;
        }

        /// <summary>
        /// 将IDataReader指定的列以集合的方式取出
        /// </summary>
        /// <typeparam name="T">字段的数据类型</typeparam>
        /// <param name="reader"><see cref="System.Data.IDataReader"/></param>
        /// <returns>columnName字段中的数据集合</returns>
        public static IList<T> GetValues<T>(this IDataReader reader)
        {
            if (reader.FieldCount == 0)
                throw new ArgumentException("IDataReader未包含任何字段。");

            return reader.GetValues<T>(0, x => (T)x);
        }

        /// <summary>
        /// 将IDataReader指定的列以集合的方式取出
        /// </summary>
        /// <typeparam name="T">字段的数据类型</typeparam>
        /// <param name="reader"><see cref="System.Data.IDataReader"/></param>
        /// <param name="columnName">字段名称</param>
        /// <returns>columnName字段中的数据集合</returns>
        public static IList<T> GetValues<T>(this IDataReader reader, string columnName)
        {
            return reader.GetValues<T>(reader.IndexOfColumn(columnName), x => (T)x);
        }

        /// <summary>
        /// 将IDataReader指定的列以集合的方式取出
        /// </summary>
        /// <typeparam name="T">字段的数据类型</typeparam>
        /// <param name="reader"><see cref="System.Data.IDataReader"/></param>
        /// <param name="columnName">字段名称</param>
        /// <returns>columnName字段中的数据集合</returns>
        public static IList<T> GetValues<T>(this IDataReader reader, string columnName, Func<object, T> convert)
        {
            return reader.GetValues<T>(reader.IndexOfColumn(columnName), convert);
        }

        /// <summary>
        /// 将IDataReader指定的列以集合的方式取出
        /// </summary>
        /// <typeparam name="T">字段的数据类型</typeparam>
        /// <param name="reader"><see cref="System.Data.IDataReader"/></param>
        /// <param name="indexOfColumn">字段索引</param>
        /// <returns>columnName字段中的数据集合</returns>
        public static IList<T> GetValues<T>(this IDataReader reader, int indexOfColumn)
        {
            return reader.GetValues<T>(indexOfColumn, x => (T)x);
        }

        /// <summary>
        /// 将IDataReader指定的列以集合的方式取出
        /// </summary>
        /// <typeparam name="T">字段的数据类型</typeparam>
        /// <param name="reader"><see cref="System.Data.IDataReader"/></param>
        /// <param name="indexOfColumn">字段索引</param>
        /// <returns>columnName字段中的数据集合</returns>
        public static IList<T> GetValues<T>(this IDataReader reader, int indexOfColumn, Func<object, T> convert)
        {
            if (!reader.Read())
                return new List<T>(0);

            if (indexOfColumn < 0 || indexOfColumn >= reader.FieldCount)
                throw new IndexOutOfRangeException("indexOfColumn下标越界。");

            List<T> ls = new List<T>(reader.RecordsAffected < 1 ? 4 : reader.RecordsAffected);
            do
            {
                ls.Add(convert(reader[indexOfColumn]));
            } while (reader.Read());
            return ls;
        }

        #endregion
    }
}
