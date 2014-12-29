using NUnit.Framework;
using Sparrow.CommonLibrary.Net;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
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

        //[Test]
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

        //[Test]
        public void Test3()
        {
            var httpClient = new HttpClient("http://image.clding.com/image/img_add");
            var fs = System.IO.File.OpenRead(@"C:\Users\tanyu_000\Desktop\QQ截图20141029160629.png");
            var result = httpClient.PostFileStream(fs, "QQ截图20141029160629.png");
            var str = result.GetOutputString();
            Assert.IsNotNull(str);
        }

        //[Test]
        public void Test4()
        {

            //根据url获取远程文件流
            //var httpClient2 = new HttpClient("http://img3.clding.com/2014/11/3/130594800410817364776211_120.jpg");
            var httpClient2 = new HttpClient("http://www.clding.com/111.jpg");
            using (var result2 = httpClient2.Get())
            {
                var stream = result2.GetOutputStream();
                using (var ms = new System.IO.MemoryStream())
                {
                    var buffer = new Byte[4096];
                    var count = 0;
                    while ((count = stream.Read(buffer, 0, 4096)) > 0)
                    {
                        ms.Write(buffer, 0, count);
                    }
                    ms.Position = 0;
                    //上传到微信
                    var httpClient = new HttpClient("http://image.clding.com/image/img_add");
                    httpClient.Timeout = 20 * 1000;
                    using (var result = httpClient.PostFileStream(ms, "stream.jpg"))
                    {
                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            var str = result.GetOutputString();
                            //解析数据 返回media_id 

                        }
                    }
                }
            }
        }

        //[Test]
        public void Test5()
        {
            var httpClient2 = new HttpClient("https://api.weixin.qq.com/cgi-bin/menu/create?access_token=DobAnJEMoj6Rf6QU9XhqR9zbgGY_FqZA03eGkumpXPz1qcq53gLEJzHb5kUz-iYwRO7tFCsz4IhK_R8MbbYr4bAoCAbLvf1SaZe2XW9Te8Y");
            string weixin1 = "";
            weixin1 += "{\n";
            weixin1 += "\"button\":[\n";
            weixin1 += "{\n";
            weixin1 += "\"type\":\"click\",\n";
            weixin1 += "\"name\":\"今日歌曲\",\n";
            weixin1 += "\"key\":\"V1001_TODAY_MUSIC123eee\"\n";
            weixin1 += "},\n";
            weixin1 += "{\n";
            weixin1 += "\"type\":\"click\",\n";
            weixin1 += "\"name\":\"歌手简介\",\n";
            weixin1 += "\"key\":\"V1001_TODAY_SINGER123eee\"\n";
            weixin1 += "},\n";
            weixin1 += "{\n";
            weixin1 += "\"name\":\"菜单\",\n";
            weixin1 += "\"sub_button\":[\n";
            weixin1 += "{\n";
            weixin1 += "\"type\":\"click\",\n";
            weixin1 += "\"name\":\"hello word\",\n";
            weixin1 += "\"key\":\"V1001_HELLO_WORLD123eee\"\n";
            weixin1 += "},\n";
            weixin1 += "{\n";
            weixin1 += "\"type\":\"click\",\n";
            weixin1 += "\"name\":\"赞一下我们\",\n";
            weixin1 += "\"key\":\"V1001_GOOD123eee\"\n";
            weixin1 += "}]\n";
            using (var result2 = httpClient2.Post(weixin1))
            {
                var result = result2.GetOutputString();
                Assert.IsNotNull(result);
            }
        }
    }
}
