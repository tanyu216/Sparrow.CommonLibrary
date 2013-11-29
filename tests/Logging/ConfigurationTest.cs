using NUnit.Framework;
using Sparrow.CommonLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparrow.CommonLibrary.Logging.Configuration;

namespace Sparrow.CommonLibrary.Test.Logging
{
    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void LoadConfigTest1()
        {
            var cty = LoggingSettings.Settings.DefaultCategory;
        }
    }
}
