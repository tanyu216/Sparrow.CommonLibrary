using NUnit.Framework;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Database.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Test.Database
{
    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void LoadConfigTest1()
        {
            var configuration = DatabaseConfigurationSection.GetSection();
        }
    }
}
