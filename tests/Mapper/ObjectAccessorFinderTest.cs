using NUnit.Framework;
using Sparrow.CommonLibrary.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Test.Mapper
{
    [TestFixture]
    public class ObjectAccessorFinderTest
    {
        [Test]
        public void Test1()
        {
            var accessor = ObjectAccessorFinder.FindObjAccessor<UserProfile>();
            Assert.IsNotNull(accessor);
        }
        [Test]
        public void Test2()
        {
            var accessor = ObjectAccessorFinder.FindObjAccessor<UserProfile2>();
            Assert.IsNotNull(accessor);
        }
        [Test]
        public void Test3()
        {
            var accessor = ObjectAccessorFinder.FindObjAccessor<UserProfile3>();
            Assert.IsNotNull(accessor);
        }

    }
}
