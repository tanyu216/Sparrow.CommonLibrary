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
            var mapper = MapperFinder.GetIMapper<UserProfile>();
            Assert.IsNotNull(mapper);
        }
        [Test]
        public void Test2()
        {
            var mapper = MapperFinder.GetIMapper<UserProfile2>();
            Assert.IsNotNull(mapper);
        }
        [Test]
        public void Test3()
        {
            var mapper = MapperFinder.GetIMapper<UserProfile3>();
            Assert.IsNotNull(mapper);
        }
    }
}
