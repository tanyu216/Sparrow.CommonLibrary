using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sparrow.CommonLibrary.Extenssions
{
    public static class ValidationExtenssions
    {

        /// <summary>
        /// 判断整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDigits(this string str)
        {
            return Regex.IsMatch(str, @"^\d+$");
        }

        /// <summary>
        /// 判断数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumber(this string str)
        {
            return Regex.IsMatch(str, @"^-?(?:\d+|\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$");
        }

        /// <summary>
        /// 判断邮件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmail(this string str)
        {
            return Regex.IsMatch(str, @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$");
        }

        /// <summary>
        /// 判断中国的固定电话
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsFixPhoneCHN(this string str)
        {
            return Regex.IsMatch(str, @"^(\+86|0?86)?-?(\d{3,4}-?)?\d{7,8}$");
        }

        /// <summary>
        /// 判断中国的手机号码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsMobilePhoneCHN(this string str)
        {
            return Regex.IsMatch(str, @"^(\+86|0?86)?-?1\d{10}$");
        }

        /// <summary>
        /// 判断Url
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsUrl(this string str)
        {
            return Regex.IsMatch(str, @"^(https?|s?ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$");
        }

        /// <summary>
        /// 判断日期
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDate(this string str)
        {
            DateTime date;
            return DateTime.TryParse(str, out date);
        }

        /// <summary>
        /// 判断ISO格式日期
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDateISO(this string str)
        {
            return Regex.IsMatch(str, @"^\d{4}[\/\-]([1-9]|0[1-9]|1[1-2])[\/\-]([1-9]|0[1-9]|[12][0-9]|3[0-1])$");
        }

        /// <summary>
        /// 判断信用卡号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsCreditCard(this string str)
        {
            if (!Regex.IsMatch(str, @"[^0-9 \-]+"))
                return false;

            var nCheck = 0;
            var nDigit = 0;
            var bEven = false;

            str = Regex.Replace(str, @"\D", "", RegexOptions.IgnoreCase);

            for (var n = str.Length - 1; n >= 0; n--)
            {
                nDigit = Convert.ToInt32(str[n].ToString());
                if (bEven)
                {
                    if ((nDigit *= 2) > 9)
                    {
                        nDigit -= 9;
                    }
                }
                nCheck += nDigit;
                bEven = !bEven;
            }

            return (nCheck % 10) == 0;
        }

        private static readonly char[] IDCardCHNVerifyCode = "1,0,x,9,8,7,6,5,4,3,2".Split(',').Select(x => x[0]).ToArray();
        private static readonly int[] IDCardCHNWI = "7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2".Split(',').Select(x => Convert.ToInt32(x)).ToArray();

        public static bool IsIDCardCHN(this string str)
        {
            if (!Regex.IsMatch(str, @"^\d{15}|\d{17}[0-9xX]$"))
                return false;

            string ai;
            if (str.Length == 18)
                ai = str.Substring(0, 17);
            else
                ai = str.Insert(6, "19");

            var dateString = string.Concat(str.Substring(6, 4), "-", str.Substring(10, 2), "-", str.Substring(12, 2));
            DateTime date;
            if (!DateTime.TryParse(dateString, out date))
                return false;

            if (str.Length == 18)
            {
                int totalAiWi = 0;
                for (var i = 0; i < 17; i++)
                {
                    totalAiWi += Convert.ToInt32(ai.Substring(i, 1)) * IDCardCHNWI[i];
                }
                return IDCardCHNVerifyCode[totalAiWi % 11] == str[17];
            }
            return true;
        }

        /// <summary>
        /// 判断字符长度介于指定范围区间
        /// </summary>
        /// <param name="str"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static bool IsRangeLength(this string str, int minLength, int maxLength)
        {
            return str.Length >= minLength && str.Length <= maxLength;
        }

        /// <summary>
        /// 判断集合长度介于指定范围区间
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static bool IsRangeLength(this ICollection collection, int minLength, int maxLength)
        {
            return collection.Count >= minLength && collection.Count <= maxLength;
        }

        /// <summary>
        /// 判断数字大小介于指定范围区间
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsRange(this short value, short min, short max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 判断数字大小介于指定范围区间
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsRange(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 判断数字大小介于指定范围区间
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsRange(this long value, long min, long max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 判断数字大小介于指定范围区间
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsRange(this decimal value, decimal min, decimal max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 判断数字大小介于指定范围区间
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsRange(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// 判断数字大小介于指定范围区间
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsRange(this double value, double min, double max)
        {
            return value >= min && value <= max;
        }
    }
}
