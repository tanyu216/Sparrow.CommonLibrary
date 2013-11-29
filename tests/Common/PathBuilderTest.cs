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
            var path = pbuilder.BuildWithVariant("%appdir%\\sprlog\\%year%_%month%\\%day%log.log");
            Assert.IsNotNullOrEmpty(path);
        }
    }
}
