using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparrow.CommonLibrary.Utility;

namespace Sparrow.CommonLibrary.Test.Utility
{
    [TestFixture]
    public class PathBuilderTest
    {
        [Test]
        public void Test()
        {
            var pbuilder = new PathBuilder();
            var path = pbuilder.Build("%appdir%\\sprlog\\%year%_%month%\\%day%log.log");
            Assert.IsNotNullOrEmpty(path);
        }
    }
}
