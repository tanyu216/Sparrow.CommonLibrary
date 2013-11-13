using NUnit.Framework;
using Sparrow.CommonLibrary.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Test.Web
{
    [TestFixture]
    public class RecognizeSpiderTest
    {
        [Test]
        public void IsSpiderTest()
        {
            var spider = new RecognizeSpider();
            Assert.IsTrue(spider.IsSpider("Baiduspider+(+http://www.baidu.com/search/spider.htm”)"));
            Assert.IsTrue(spider.IsSpider("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)"));
            Assert.IsTrue(spider.IsSpider("Googlebot/2.1 (+http://www.googlebot.com/bot.html)"));
            Assert.IsTrue(spider.IsSpider("Googlebot/2.1 (+http://www.google.com/bot.html)"));
            Assert.IsTrue(spider.IsSpider("Mozilla/5.0 (compatible; Yahoo! Slurp China; http://misc.yahoo.com.cn/help.html)"));
            Assert.IsTrue(spider.IsSpider("Mozilla/5.0 (compatible; Yahoo! Slurp; http://help.yahoo.com/help/us/ysearch/slurp)"));
            Assert.IsTrue(spider.IsSpider("iaskspider/2.0(+http://iask.com/help/help_index.html”)"));
            Assert.IsTrue(spider.IsSpider("Mozilla/5.0 (compatible; iaskspider/1.0; MSIE 6.0)"));
            Assert.IsTrue(spider.IsSpider("Sogou web spider/3.0(+http://www.sogou.com/docs/help/webmasters.htm#07)"));
            Assert.IsTrue(spider.IsSpider("Sogou Push Spider/3.0(+http://www.sogou.com/docs/help/webmasters.htm#07)"));
            Assert.IsTrue(spider.IsSpider("Mozilla/5.0 (compatible; YodaoBot/1.0;http://www.yodao.com/help/webmaster/spider/;)"));
            Assert.IsTrue(spider.IsSpider("msnbot/1.0 (+http://search.msn.com/msnbot.htm)"));
            Assert.IsTrue(spider.IsSpider("Sosospider+(+http://help.soso.com/webspider.htm)"));
            Assert.IsTrue(spider.IsSpider("Mozilla/5.0 (compatible; YodaoBot/1.0; http://www.yodao.com/help/webmaster/spider/; )"));

            Assert.IsFalse(spider.IsSpider("Mozilla/4.0 (compatible; MSIE 10.0; Windows NT 6.0)"));
            Assert.IsFalse(spider.IsSpider("Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.0)"));
            Assert.IsFalse(spider.IsSpider("Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)"));
            Assert.IsFalse(spider.IsSpider("Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2)"));
            Assert.IsFalse(spider.IsSpider("Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)"));
            Assert.IsFalse(spider.IsSpider("Mozilla/5.0 (Windows; U; Windows NT 5.2) Gecko/2008070208 Firefox/3.0.1 "));
            Assert.IsFalse(spider.IsSpider("Opera/9.27 (Windows NT 5.2; U; zh-cn)"));
            Assert.IsFalse(spider.IsSpider("Mozilla/5.0 (Macintosh; PPC Mac OS X; U; en) Opera 8.0"));
            Assert.IsFalse(spider.IsSpider("Mozilla/5.0 (Windows; U; Windows NT 5.2) AppleWebKit/525.13 (KHTML, like Gecko) Version/3.1 Safari/525.13"));
            Assert.IsFalse(spider.IsSpider("Mozilla/5.0 (iPhone; U; CPU like Mac OS X) AppleWebKit/420.1 (KHTML, like Gecko) Version/3.0 Mobile/4A93 Safari/419.3"));
            Assert.IsFalse(spider.IsSpider("Mozilla/5.0 (Windows; U; Windows NT 5.2) AppleWebKit/525.13 (KHTML, like Gecko) Chrome/0.2.149.27 Safari/525.13"));
            Assert.IsFalse(spider.IsSpider("Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.12) Gecko/20080219 Firefox/2.0.0.12 Navigator/9.0.0.6"));

        }
        [Test]
        public void IsSpiderTest2()
        {
            var spider = new RecognizeSpider();
            string spiderName;
            Assert.IsTrue(spider.IsSpider("Baiduspider+(+http://www.baidu.com/search/spider.htm”)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("Googlebot/2.1 (+http://www.googlebot.com/bot.html)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("Googlebot/2.1 (+http://www.google.com/bot.html)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("Mozilla/5.0 (compatible; Yahoo! Slurp China; http://misc.yahoo.com.cn/help.html)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("Mozilla/5.0 (compatible; Yahoo! Slurp; http://help.yahoo.com/help/us/ysearch/slurp)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("iaskspider/2.0(+http://iask.com/help/help_index.html”)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("Mozilla/5.0 (compatible; iaskspider/1.0; MSIE 6.0)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("Sogou web spider/3.0(+http://www.sogou.com/docs/help/webmasters.htm#07)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("Sogou Push Spider/3.0(+http://www.sogou.com/docs/help/webmasters.htm#07)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("Mozilla/5.0 (compatible; YodaoBot/1.0;http://www.yodao.com/help/webmaster/spider/;)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("msnbot/1.0 (+http://search.msn.com/msnbot.htm)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("Sosospider+(+http://help.soso.com/webspider.htm)", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);
            Assert.IsTrue(spider.IsSpider("Mozilla/5.0 (compatible; YodaoBot/1.0; http://www.yodao.com/help/webmaster/spider/; )", out spiderName));
            Assert.IsNotNullOrEmpty(spiderName);


            Assert.IsFalse(spider.IsSpider("Mozilla/5.0 (Windows; U; Windows NT 5.2) AppleWebKit/525.13 (KHTML, like Gecko) Chrome/0.2.149.27 Safari/525.13", out spiderName));
            Assert.IsNull(spiderName);
        }
    }
}
