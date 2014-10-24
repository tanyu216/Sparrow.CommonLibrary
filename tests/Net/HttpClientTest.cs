using NUnit.Framework;
using Sparrow.CommonLibrary.Net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Test.Web
{
    [TestFixture]
    public class HttpClientTest
    {
        //[Test]
        public void Test()
        {
            var httpClient = new HttpClient("http://open.test.com/user/1");
            var result = httpClient.Get();

            var httpClient2 = new HttpClient("http://open.test.com/user");
            var forms = new NameValueCollection();
            forms.Add("id", "1");
            forms.Add("name", "姓名");
            var result2 = httpClient2.Post(forms);


        }
        
        [Test]
        public void Test2()
        {
            var httpClient = new HttpClient("https://api.weixin.qq.com/cgi-bin/shorturl?access_token=ACCESS_TOKEN");
            var str = "{\"action\":\"long2short\",\"long_url\":\"http://wap.koudaitong.com/v2/showcase/goods?alias=128wi9shh&spm=h56083&redirect_count=1\"}";
            using (var response = httpClient.Post(str))
            {
                var result = response.GetOutputString();
                Assert.IsNotNull(result);
            }
        }
    }
}
