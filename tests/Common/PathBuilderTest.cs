using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparrow.CommonLibrary.Common;

namespace Sparrow.CommonLibrary.Test.Common
{
    [TestFixture]
    public class PathBuilderTest
    {
        [Test]
        public void Test()
        {
            var pbuilder = new PathBuilder();
            pbuilder.SetVariant("%test%", x => "123");
            var path = pbuilder.BuildWithVariant("%appdir%\\sprlog\\%year%_%month%\\%day%log.log");

            Assert.IsNotNullOrEmpty(path);
        }
        [Test]
        public void Test2()
        {
            var pbuilder = new PathBuilder();
            var path = pbuilder.RebuildNextPath(@"D:\clding\trunk\projects\ClDing\Tests\ClDing.Utility.Tests\bin\Debug\cldinglog\error\2014\141109.log");
            var path2 = pbuilder.RebuildNextPathByFileSize(@"D:\clding\trunk\projects\ClDing\Tests\ClDing.Utility.Tests\bin\Debug\cldinglog\error\2014\141109.log", "1MB");
            Assert.IsNotNullOrEmpty(path);
            Assert.IsNotNullOrEmpty(path2);
        }
    }
}
