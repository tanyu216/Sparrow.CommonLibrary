using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sparrow.CommonLibrary.Utility.Extenssions;

namespace Sparrow.CommonLibrary.Utility
{
    /// <summary>
    /// 文件路径生成辅助工具
    /// </summary>
    public class PathBuilder
    {
        private readonly DynamicKeyValueContainer<string, string> _variant;

        public PathBuilder()
        {
            _variant = new DynamicKeyValueContainer<string, string>();
            //
            _variant.Add("%appdir%", (x) => Environment.GetFolderPath(Environment.SpecialFolder.Startup));
            //windir|systemroot|temp返回的是同一个值（主要是与windows操作系统的习惯保持一致）
            _variant.Add("%windir%", (x) => Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            _variant.Add("%systemroot%", (x) => Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            _variant.Add("%temp%", (x) => Path.GetTempPath());
            //
            _variant.Add("%rndname%", (x) => Path.GetRandomFileName());
            // 日期类
            _variant.Add("%date%", (x) => DateTime.Now.ToString("yyMMdd"));
            _variant.Add("%year%", (x) => DateTime.Now.Year.ToString("yyyy"));
            _variant.Add("%month%", (x) => DateTime.Now.Month.ToString("MM"));
            _variant.Add("%day%", (x) => DateTime.Now.Day.ToString("dd"));
            _variant.Add("%hour%", (x) => DateTime.Now.Hour.ToString("hh"));
            _variant.Add("%minute%", (x) => DateTime.Now.Minute.ToString("mm"));
        }

        private void ValidVariantName(string variantName)
        {
            if (string.IsNullOrWhiteSpace(variantName))
                throw new ArgumentException();
            if (!Regex.IsMatch(variantName, "%[_a-zA-Z][_a-zA-Z0-9]*%"))
                throw new ArgumentException("变量名称不合法，请遵循：%[_a-zA-Z][_a-zA-Z0-9]*%。");
        }

        /// <summary>
        /// 设置变量
        /// </summary>
        /// <param name="variantName">变量名称</param>
        /// <param name="func">获取变量的方法</param>
        public void SetVariant(string variantName, Func<object, string> func)
        {
            ValidVariantName(variantName);
            _variant[variantName] = func;
        }

        /// <summary>
        /// 解析1kb/1mb/1gb，将其统一转换为字节单位。如：1KB=1024字节。
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public int ParseFileSize(string fileSize)
        {
            if (string.IsNullOrWhiteSpace(fileSize))
                return int.MaxValue;
            //
            fileSize = fileSize.Trim();
            var match = Regex.Match(fileSize, @"^(\d+)(kb|k)$", RegexOptions.IgnoreCase);
            if (match.Groups.Count > 1)
            {
                unchecked
                {
                    return match.Groups[1].Value.ToInt() * 1024; //千字节转换成字节
                }
            }
            match = Regex.Match(fileSize, @"^(\d+)(mb|m)$", RegexOptions.IgnoreCase);
            if (match.Groups.Count > 1)
            {
                unchecked
                {
                    return match.Groups[1].Value.ToInt() * 1024 * 1024; //兆节转换成字节
                }
            }
            match = Regex.Match(fileSize, @"^(\d+)(gb|g)$", RegexOptions.IgnoreCase);
            if (match.Groups.Count > 1)
            {
                unchecked
                {
                    return match.Groups[1].Value.ToInt() * 1024 * 1024 * 1024; //千兆节转换成字节
                }
            }
            return int.MaxValue;
        }

        /// <summary>
        /// 依据文件容量动态分配新的路径。当文件达到限制大小时，生成一个新的文件路径（新路径在原有的路径上加上一个从1开始的序号）。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileSizeLimit"></param>
        /// <returns></returns>
        public string RebuildPathByFileSize(string path, string fileSizeLimit)
        {
            var size = ParseFileSize(fileSizeLimit);
            return RebuildPathByFileSize(path, 1, size);
        }

        private string RebuildPathByFileSize(string path, int startIndex, int sizeLimit)
        {
            // 计算日志文件的大小，日志文件超出配置大小时，重新分配一个新日志文件。
            if (sizeLimit > 0 && File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                if (fileInfo.Length >= sizeLimit)
                {
                    var ext = Path.GetExtension(path);
                    if (string.IsNullOrEmpty(ext))
                    {
                        //不带扩展名的文件
                        path = string.Concat(path, "-", startIndex.ToString());
                    }
                    else
                    {
                        path = new StringBuilder(path).Insert(path.Length - ext.Length, "_" + startIndex.ToString()).ToString();
                    }
                    return RebuildPathByFileSize(path, startIndex + 1, sizeLimit);
                }
            }
            return path;
        }

        /// <summary>
        /// 生成路径
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="state">变量的上下文</param>
        /// <returns></returns>
        public string Build(string path, object state)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");
            //
            StringBuilder pattern = new StringBuilder();
            foreach (var key in _variant.Keys)
                pattern.Append('(').Append(key).Append(')').Append('|');
            pattern.Remove(pattern.Length - 1, 1);
            //
            var regex = new Regex(pattern.ToString(), RegexOptions.IgnoreCase);
            var filepath = regex.Replace(path,
                    x =>
                    {
                        var val = _variant.GetValue(x.Value, state);
                        if (!string.IsNullOrEmpty(val))
                            return val;
                        return x.Value.Replace("%", "");
                    });
            return filepath;
        }

        /// <summary>
        /// 将两段字符串合并成一个路径
        /// </summary>
        /// <param name="state"></param>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public string Combine(object state, string path1, string path2)
        {
            return Build(Path.Combine(path1, path2), state);
        }

        /// <summary>
        /// 将三段字符串合并成一个路径
        /// </summary>
        /// <param name="state"></param>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="path3"></param>
        /// <returns></returns>
        public string Combine(object state, string path1, string path2, string path3)
        {
            return Build(Path.Combine(path1, path2, path3), state);
        }

        /// <summary>
        /// 将四段字符串合并成一个路径
        /// </summary>
        /// <param name="state"></param>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="path3"></param>
        /// <param name="path4"></param>
        /// <returns></returns>
        public string Combine(object state, string path1, string path2, string path3, string path4)
        {
            return Build(Path.Combine(path1, path2, path3, path4), state);
        }

        /// <summary>
        /// 将多段字符串合并成一个路径
        /// </summary>
        /// <param name="state"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public string Combine(object state, params string[] paths)
        {
            return Build(Path.Combine(paths), state);
        }
    }
}
