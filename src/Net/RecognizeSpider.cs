using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Sparrow.CommonLibrary.Net
{
    /// <summary>
    /// 识别网络爬虫
    /// </summary>
    public class RecognizeSpider
    {
        private static readonly string DefaultSpiders = "bot|spider|Yahoo! Slurp|ia_archiver|heritrix|P.Arthur|Mediapartners-Google|FeedFetcher-Google|sqlmap|Paros|pangolin";
        private static readonly string DefaultSpiderNames = "Googlebot|FeedFetcher-Google|Mediapartners-Google|Baiduspider|Sosospider|Sosoimagespider|Yahoo! Slurp|Sogou Push Spider|Sogou Orion spider|Sogou web spider|Sogou head spider|YodaoBot|heritrix|ia_archiver|iaskspider|P.Arthur|QihooBot|msnbot|EtaoSpider|sqlmap|Paros|pangolin";

        private string[] spiders;
        private string[] spiderNames;
        private Regex spiderReg;
        private Regex spiderNameReg;

        public RecognizeSpider()
            : this(DefaultSpiders.Split('|'), DefaultSpiderNames.Split('|'))
        {

        }

        public RecognizeSpider(string[] spiders, string[] spiderNames)
        {
            this.spiders = spiders;
            this.spiderNames = spiderNames;
            spiderReg = new Regex(string.Join("|", spiders.Select(x => Regex.Escape(x))), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            spiderNameReg = new Regex(string.Join("|", spiderNames.Select(x => Regex.Escape(x))), RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public void AddNewSpider(string[] spiders)
        {
            if (spiders == null)
                throw new ArgumentNullException("spiders");
            var list = new List<string>(spiders.Length + this.spiders.Length);
            list.AddRange(this.spiders);
            list.AddRange(spiders);
            this.spiders = list.ToArray();
            spiderReg = new Regex(string.Join("|", list.Distinct().Select(x => Regex.Escape(x))), RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public void AddNewSpiderName(string[] spiderNames)
        {
            if (spiderNames == null)
                throw new ArgumentNullException("spiderNames");
            var list = new List<string>(spiderNames.Length + this.spiderNames.Length);
            list.AddRange(this.spiderNames);
            list.AddRange(spiderNames);
            this.spiderNames = list.ToArray();
            spiderNameReg = new Regex(string.Join("|", list.Distinct().Select(x => Regex.Escape(x))), RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证是否是爬虫
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsSpider(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return spiderReg.IsMatch(str);
        }

        /// <summary>
        /// 验证是否是爬虫
        /// </summary>
        /// <param name="str"></param>
        /// <param name="spiderName">识别出的抓虫名称</param>
        /// <returns></returns>
        public bool IsSpider(string str, out string spiderName)
        {
            spiderName = null;

            if (IsSpider(str))
            {
                // 网络爬虫
                var name = spiderNameReg.Match(str).Groups[0].Value;
                if (!string.IsNullOrEmpty(name))
                {
                    spiderName = name;
                }
                else
                {
                    //被认出是爬虫，但不以收录的spiderNames中。
                    spiderName = "other";
                }
                return true;
            }
            //
            return false;
        }

    }
}
