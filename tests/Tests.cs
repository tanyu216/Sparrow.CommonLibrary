using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sparrow.CommonLibrary.Common;

namespace Sparrow.CommonLibrary.Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test1()
        {
            Assert.IsTrue(typeof(int).IsPrimitive);
            Assert.IsTrue(typeof(int).IsValueType);

            Assert.IsFalse(typeof(Tests).IsPrimitive);
            Assert.IsFalse(typeof(object).IsPrimitive);
        }

        [Test]
        public void Test2()
        {
        }
    }
}
