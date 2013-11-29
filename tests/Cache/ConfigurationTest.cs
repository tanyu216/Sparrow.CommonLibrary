using NUnit.Framework;
using Sparrow.CommonLibrary.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparrow.CommonLibrary.Cache.Configuration;

namespace Sparrow.CommonLibrary.Test.Cache
{
    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void LoadConfigTest1()
        {
            var cty = CacheSettings.Settings.DefaultRegionName;
        }
    }
}
