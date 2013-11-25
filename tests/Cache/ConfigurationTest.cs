using NUnit.Framework;
using Sparrow.CommonLibrary.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Test.Cache
{
    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void LoadConfigTest1()
        {
            var cty = CacheSettings.DefaultRegionName;
        }
    }
}
