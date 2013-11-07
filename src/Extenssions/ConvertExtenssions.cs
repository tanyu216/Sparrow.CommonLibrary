using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sparrow.CommonLibrary.Extenssions
{
    public static class ConvertExtenssions
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
        public static int ToInt(this string val)
        {
            return val.ToInt(default(int));
        }
        /// <summary>
        /// 将字符转换成System.Int32类型
        /// </summary>
        /// <param name="val">System.String</param>
        /// <param name="defVal">在转换成 T 失败时，返回的默认值</param>
        /// <returns>如果转换失败将返回 defVal</returns>
        public static int ToInt(this string val, int defVal)
        {
            if (val == null)
                return defVal;

            int val2;
            if (int.TryParse(val, out val2))
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
        public static decimal ToDecimal(this string val)
        {
            return val.ToDecimal(default(decimal));
        }
        /// <summary>
        /// 将字符转换成 System.Decimal类型
        /// </summary>
        /// <param name="val">System.String</param>
        /// <param name="defVal">在转换成 T 失败时，返回的默认值</param>
        /// <returns>如果转换失败将返回 defVal</returns>
        public static decimal ToDecimal(this string val, decimal defVal)
        {
            if (val == null)
                return defVal;

            decimal val2;
            if (decimal.TryParse(val, out val2))
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

        public static DateTime ToDateTime(this string value)
        {
            return ToDateTime(value, default(DateTime));
        }

        public static DateTime ToDateTime(this string value, DateTime defVal)
        {
            if (value == null)
                return defVal;

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

            bool val2;
            if (bool.TryParse(value.ToString(), out val2))
                return val2;
            return defVal;
        }

        #endregion

    }
}
