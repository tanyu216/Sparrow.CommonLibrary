using NUnit.Framework;
using Sparrow.CommonLibrary.Weblog;
using Sparrow.CommonLibrary.Weblog.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Test.Weblog
{
    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void LoadConfigTest1()
        {
            var configuration = WeblogConfigurationSection.GetSection();
        }
    }
}
