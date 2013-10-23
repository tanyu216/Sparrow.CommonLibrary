using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sparrow.CommonLibrary.Cache;

namespace Sparrow.CommonLibrary.Cache.Test
{
    [TestFixture]
    public class CacheManagerTest
    {
        [SetUp]
        public void Init()
        {
            CacheManager.Set("key1", "value1", DateTime.Now.AddMinutes(60));
            CacheManager.Set("key2", "value2");
            CacheManager.Set("exkey1", "exvalue1", DateTime.Now.AddSeconds(2));

        }

        [TearDown]
        public void Clean()
        {
            CacheManager.RemoveCache("test");
        }

        [Test]
        public void SetICacheCreaterTest()
        {
            CacheManager.SetICacheCreater(x => new LocalCache(x));
            Assert.AreEqual(true, true);
        }

        [Test]
        public void AddCacheTest()
        {
            var result = CacheManager.AddCache("test", new LocalCache("test"));
            Assert.AreEqual(result, true);
        }

        [Test]
        public void GetCacheTest()
        {
            var result = CacheManager.GetCache();
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetCacheTest2()
        {
            var result = CacheManager.GetCache(CacheManager.DefaultRegionName);
            Assert.IsNotNull(result);
        }

        [Test]
        public void ContainsTest()
        {
            var result = CacheManager.Contains("key1");
            Assert.AreEqual(result, true);
        }

        [Test]
        public void GetTest()
        {
            var result = CacheManager.Get<object>("key2");
            Assert.AreEqual(result, "value2");
            result = CacheManager.Get<object>("key1000000000000");
            Assert.IsNull(result);
        }

        [Test]
        public void GetTest2()
        {
            var result = CacheManager.Get(new[] { "key2", "key9000000000000" });
            Assert.IsNotNull(result);
            Assert.Greater(result.Count, 1);
            Assert.IsNotNull(result["key2"]);
            Assert.AreEqual(result["key2"], "value2");
        }

        [Test]
        public void GetTest3()
        {
            var result = CacheManager.Get(new List<string>() { "key2", "key9000000000000" });
            Assert.IsNotNull(result);
            Assert.Greater(result.Count, 1);
            Assert.AreEqual(result["key2"], "value2");
        }

        [Test]
        public void GetTest4()
        {
            System.Threading.Thread.Sleep(new TimeSpan(0, 0, 3));
            var result = CacheManager.Get("exkey1");
            Assert.IsNull(result);
        }

        [Test]
        public void SetTest()
        {
            CacheManager.Set("testkey1", "testvalue1");
        }

        [Test]
        public void SetTest2()
        {
            CacheManager.Set("testkey1", "testvalue1", DateTime.Now.AddMinutes(2));
        }
    }
}
