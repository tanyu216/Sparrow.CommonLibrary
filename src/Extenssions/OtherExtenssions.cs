using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sparrow.CommonLibrary.Extenssions
{
    public static class OtherExtenssions
    {
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

    }
}
