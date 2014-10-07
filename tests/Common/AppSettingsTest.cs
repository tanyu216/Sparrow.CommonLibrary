using NUnit.Framework;
using Sparrow.CommonLibrary.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Test.Common
{
    [TestFixture]
    public class AppSettingsTest
    {
        [Test]
        public void Test()
        {

            var result = AppSettings.GetValue<int>("key1");

        }
        [Test]
        public void Test2()
        {

            AppSettings.SetDefualtValue("key1", 2);

        }
    }
}
