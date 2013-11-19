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
using System.Linq.Expressions;
using System.Collections;

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
        public void LogicalExpressionTest()
        {
            int i = new Random().Next();
            //TestExpression<UserProfile>(x => x.Id == 1);
            var logicalExp0 = SqlExpression.Expression<UserProfile>(x => x.Id == i);
            var logicalExp1 = SqlExpression.Expression<UserProfile>(x => x.Id == i && (x.Name == (object)new object[] { "test0", "test1", i } || x.Sex == 1));
            var logicalExp2 = SqlExpression.Expression<UserProfile>(x => x.Id == i && (x.Name == (object)new List<object> { "test0", "test1", i } || x.Sex == 1));
            var logicalExp3 = SqlExpression.Expression<UserProfile>(x => x.Id > 10 || x.Id < 2);
            var logicalExp4 = SqlExpression.Expression<UserProfile>(x => x.Id >= 10 || x.Id <= 2);

        }

        private void TestExpression<T>(Expression<Func<T, bool>> expression)
        {
            var value = Expression.Lambda<Func<object>>(Expression.MakeUnary(System.Linq.Expressions.ExpressionType.Convert, ((System.Linq.Expressions.BinaryExpression)expression.Body).Right, typeof(object))).Compile()();
            int val = (int)value;

        }
    }
}
