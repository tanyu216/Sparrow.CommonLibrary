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

        [Test]
        public void Test2()
        {
            Regex orderReplace = new Regex(@"\s+(DESC|ASC)\s*($|,)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var reverseOrderby = orderReplace.Replace("id   desc , name asc", y =>
            {
                switch ((y.Groups[1].Value ?? string.Empty).ToUpper())
                {
                    case "ASC":
                        if ((y.Groups[2].Value ?? string.Empty) == ",")
                            return " DESC,";
                        return " DESC ";
                    case "DESC":
                        if ((y.Groups[2].Value ?? string.Empty) == ",")
                            return " ASC,";
                        return " ASC ";
                }
                return string.Empty;
            });

        }
    }
}
