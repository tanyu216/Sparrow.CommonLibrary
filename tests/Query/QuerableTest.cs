using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Query;
using Sparrow.CommonLibrary.Test.Mapper;

namespace Sparrow.CommonLibrary.Test.Query
{
    [TestFixture]
    public class QuerableTest
    {
        [Test]
        public void Test1()
        {
            var database = DatabaseHelper.GetHelper("test");
            var queryable = database.CreateQueryable<UserProfile>();

        }

    }
}
