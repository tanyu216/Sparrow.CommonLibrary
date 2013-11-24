using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Sparrow.CommonLibrary.Test.Database
{
    [TestFixture]
    public class DatabaseTest
    {
        [Test]
        public void Test1()
        {
            var guid = Guid.NewGuid().ToString();
        }
    }
}
