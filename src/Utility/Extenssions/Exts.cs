using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sparrow.CommonLibrary.Utility.Extenssions
{
    public static class Exts
    {
        public static byte[] ToUtf8Bytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static string Base64StringEncode(this string base64String)
        {
            return Regex.Replace(base64String, "\\+|=|/", x =>
                                                          {
                                                              switch (x.Value)
                                                              {
                                                                  case "+":
                                                                      return "_";
                                                                  case "=":
                                                                      return "`";
                                                                  default:
                                                                      return "-";
                                                              }
                                                          });
        }

        public static string Base64StringDecode(this string base64String)
        {
            return Regex.Replace(base64String, "_|\\-|`", x =>
                                                          {
                                                              switch (x.Value)
                                                              {
                                                                  case "_":
                                                                      return "+";
                                                                  case "`":
                                                                      return "=";
                                                                  default:
                                                                      return "/";
                                                              }
                                                          });
        }

        #region To T

        /// <summary>
        /// 将字符转换成自己的类型
        /// </summary>
        /// <param name="val">System.String</param>
        /// <returns>如果转换失败将返回 T 的默认值</returns>
        public static T To<T>(this object val)
        {
            if (val != null)
            {
                return val.To(default(T));
            }
            return default(T);
        }

        /// <summary>
        /// 将字符转换成自己的类型
        /// </summary>
        /// <param name="val">System.Object</param>
        /// <param name="defVal">在转换成 T 失败时，返回的默认值</param>
        /// <returns>类型 T 的值</returns>
        public static T To<T>(this object val, T defVal)
        {
            if (val == null)
                return defVal;
            if (val is T)
                return (T)val;

            Type type = typeof(T);
            try
            {
                if (type.BaseType == typeof(Enum))
                {
                    return (T)Enum.Parse(type, val.ToString(), true);
                }
                if (type == typeof(DateTime?))
                {
                    DateTime r;
                    if (DateTime.TryParse(val.ToString(), out r))
                        return (T)(object)r;
                    return (T)val;
                }
                return (T)Convert.ChangeType(val, type);
            }
            catch
            {
                return defVal;
            }
        }

        #endregion

        #region To int

        /// <summary>
        /// 将字符转换成System.Int32类型
        /// </summary>
        /// <param name="val">System.String</param>
        /// <returns>如果转换失败将返回 System.Int32的默认值</returns>
        public static int ToInt(this object val)
        {
            return val.ToInt(default(int));
        }
        /// <summary>
        /// 将字符转换成System.Int32类型
        /// </summary>
        /// <param name="val">System.String</param>
        /// <param name="defVal">在转换成 T 失败时，返回的默认值</param>
        /// <returns>如果转换失败将返回 defVal</returns>
        public static int ToInt(this object val, int defVal)
        {
            if (val == null)
                return defVal;
            if (val is int)
                return (int)val;

            int val2;
            if (int.TryParse(val.ToString(), out val2))
                return val2;
            return defVal;
        }
        /// <summary>
        /// 将字符转换成可空的System.Int32类型
        /// </summary>
        /// <param name="val">System.String</param>
        /// <returns>如果转换失败将返回 Null</returns>
        public static int? ToNullableInt(this string val)
        {
            int result;
            if (int.TryParse(val, out result))
                return result;
            return null;
        }

        #endregion

        #region To decimal

        /// <summary>
        /// 将字符转换成 System.Decimal类型
        /// </summary>
        /// <param name="val">System.String</param>
        /// <returns>如果转换失败将返回 System.Decimal 的默认值</returns>
        public static decimal ToDecimal(this object val)
        {
            return val.ToDecimal(default(decimal));
        }
        /// <summary>
        /// 将字符转换成 System.Decimal类型
        /// </summary>
        /// <param name="val">System.String</param>
        /// <param name="defVal">在转换成 T 失败时，返回的默认值</param>
        /// <returns>如果转换失败将返回 defVal</returns>
        public static decimal ToDecimal(this object val, decimal defVal)
        {
            if (val == null)
                return defVal;
            if (val is decimal)
                return (decimal)val;

            decimal val2;
            if (decimal.TryParse(val.ToString(), out val2))
                return val2;
            return defVal;
        }
        /// <summary>
        /// 将字符转换成可空的System.Decimal类型
        /// </summary>
        /// <param name="val">System.String</param>
        /// <returns>如果转换失败将返回 Null</returns>
        public static decimal? ToNullableDecimal(this string val)
        {
            decimal result;
            if (Decimal.TryParse(val, out result))
                return result;
            return null;
        }

        #endregion

        #region To String

        public static string ToSafeString(this string value)
        {
            return value ?? string.Empty;
        }

        #endregion

        #region To DateTime

        public static DateTime ToDateTime(this object value)
        {
            return ToDateTime(value, default(DateTime));
        }

        public static DateTime ToDateTime(this object value, DateTime defVal)
        {
            if (value == null)
                return defVal;
            if (value is DateTime)
                return (DateTime)value;
            try
            {
                return Convert.ToDateTime(value);
            }
            catch { return defVal; }
        }

        #endregion

        #region To bool

        public static bool ToBoolean(this object value)
        {
            return ToBoolean(value, default(bool));
        }

        public static bool ToBoolean(this object value, bool defVal)
        {
            if (value == null)
                return defVal;
            if (value is bool)
                return (bool)value;
            bool val2;
            if (bool.TryParse(value.ToString(), out val2))
                return val2;
            return defVal;
        }

        #endregion

        public static string Defualt(this string str, string def)
        {
            if (string.IsNullOrEmpty(str))
                return def;
            return str;
        }

        public static string Cut(this string str, int index)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            if (str.Length <= index)
                return null;
            return str[index].ToString(CultureInfo.InvariantCulture);
        }

        public static string Cut(this string str, int startIndex, int endIndex)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            if (str.Length <= startIndex)
                return null;
            if (str.Length > endIndex)
                return str.Substring(startIndex, endIndex - startIndex + 1);
            return str.Substring(startIndex);
        }

        /// <summary>
        /// 判断是否为数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumber(this string str)
        {
            return Regex.IsMatch(str, @"^\d+$");
        }

        /// <summary>
        /// 判断是否为浮点数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDouble(this string str)
        {
            return Regex.IsMatch(str, @"^\d+(\.\d+)?$");
        }

        /// <summary>
        /// 随机打乱集合的顺序
        /// </summary>
        /// <param name="list"></param>
        public static void RandomList(this IList list)
        {
            var random = new Random();
            var len = list.Count / 2;
            var max = list.Count;
            for (int i = 0; i <= len; i++)
            {
                int rnd = random.Next(i + 1, max);
                object val = list[i];
                list[i] = list[rnd];
                list[rnd] = val;
            }
        }

        /// <summary>
        /// 随机打乱集合的顺序
        /// </summary>
        /// <param name="arrary"></param>
        public static void RandomList(this object[] arrary)
        {
            var random = new Random();
            var len = arrary.Length / 2;
            var max = arrary.Length;
            for (int i = 0; i <= len; i++)
            {
                int rnd = random.Next(i + 1, max);
                object val = arrary[i];
                arrary[i] = arrary[rnd];
                arrary[rnd] = val;
            }
        }
    }
}
