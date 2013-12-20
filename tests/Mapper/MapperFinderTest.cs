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
    public class MapperFinderTest
    {
        [Test]
        public void Test1()
        {
            var mapper = ObjectAccessorFinder.FindObjAccessor<UserProfile>();
            Assert.IsNotNull(mapper);
        }
        [Test]
        public void Test2()
        {
            var mapper = ObjectAccessorFinder.FindObjAccessor<UserProfile2>();
            Assert.IsNotNull(mapper);
        }
        [Test]
        public void Test3()
        {
            var mapper = ObjectAccessorFinder.FindObjAccessor<UserProfile3>();
            Assert.IsNotNull(mapper);
        }

    }
}
