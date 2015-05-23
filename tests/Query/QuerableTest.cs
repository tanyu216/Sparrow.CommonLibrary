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
using Sparrow.CommonLibrary.Extenssions;

namespace Sparrow.CommonLibrary.Test.Query
{
    [TestFixture]
    public class QuerableTest
    {
        [Test]
        public void QueryableTest1()
        {
            var database = DatabaseHelper.GetHelper("test");
            var parameter = database.CreateParamterCollection();

            var queryable = database.CreateQueryable<UserProfile>();
            var sql = queryable.Select(x => x.Id, x => x.Name)
                .Where(x => x.Id >= 1 && x.Id < 11 && (x.Name == "test" || x.Name == "test2"))
                .OutputSqlString(parameter);
            Assert.IsNotNullOrEmpty(sql);
        }

        [Test]
        public void QueryableTest2()
        {
            var database = DatabaseHelper.GetHelper("test");
            var parameter = database.CreateParamterCollection();

            var queryable = database.CreateQueryable<UserProfile>();
            var sql = queryable.Select(x => x.Id, x => x.Name)
                .Where(x => (object)x.Id == (object)new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })
                .Where(x => x.Name == "test")
                .OutputSqlString(parameter);
            Assert.IsNotNullOrEmpty(sql);
        }

        [Test]
        public void QueryableTest3()
        {
            var database = DatabaseHelper.GetHelper("test");
            var parameter = database.CreateParamterCollection();

            var sql = database.CreateQueryable<UserProfile>()
                .Select(x => x.Id, x => x.Name)
                .Where(x => x.Id == 4 || (x.Id >= 1 && x.Id <= 10) || x.Id == null || (x.Id != 3 && x.Id != null))
                .Where(x => x.Sex == 1)
                .OutputSqlString(parameter);

            Assert.IsNotNullOrEmpty(sql);
        }

        [Test]
        public void QueryableTest4()
        {
            var database = DatabaseHelper.GetHelper("test");
            var parameter = database.CreateParamterCollection();

            var sql = database.CreateQueryable<UserProfile>()
                .Select(x => x.Id, x => x.Name)
                .Where(x => x.Name.Contains("1"))
                .OutputSqlString(parameter);

            Assert.IsNotNullOrEmpty(sql);
        }

        [Test]
        public void QueryableTest5()
        {
            var database = DatabaseHelper.GetHelper("test");
            var parameter = database.CreateParamterCollection();

            var sql = database.CreateQueryable<UserProfile>()
                .Select(x => x.Id, x => x.Name)
                .Where(x => x.Sex == 1 && x.Name == "123" && (x.Id >= 1 || x.Id <= 10))
                .OutputSqlString(parameter);

            Assert.IsNotNullOrEmpty(sql);
        }

        [Test]
        public void QueryableListTest1()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = new Queryable<UserProfile>(database)
                .Where(x => (object)x.Id == (object)new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })
                .ExecuteList();

            Assert.IsNotNull(list);
        }

        [Test]
        public void QueryableListTest2()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile3>()
                .ExecuteList();

            Assert.IsNotNull(list);
        }

        [Test]
        public void QueryableListTest3()
        {
            var database = DatabaseHelper.GetHelper("test");

            using (var read = database.CreateQueryable<UserProfile>()
                .Select(x => x.Name)
                .Count(x => x.Id)
                .Where(x => (object)x.Id == (object)new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })
                .GroupBy(x => x.Name)
                .ExecuteReader())
            {
                var values = read.ToDictionary<string, int>();
            }

        }

        [Test]
        public void QueryableListTest4()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile>()
                .Where(x => x.Birthday < DateTime.Now)
                .ExecuteList();

        }

        [Test]
        public void QueryableScalarTest1()
        {
            var database = DatabaseHelper.GetHelper("test");

            var count = database.CreateQueryable<UserProfile>()
                .Count(x => x.Id)
                .Where(x => x.Birthday < DateTime.Now)
                .ExecuteScalar<int>();

        }


        private int GetId()
        {
            return 1;
        }
        [Test]
        public void QueryableListTest5()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile>()
                .Where(x => x.Id > GetId())
                .ExecuteList();

        }

        private int MinId { get { return 1; } }
        [Test]
        public void QueryableListTest6()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile>()
                .Where(x => x.Id > MinId)
                .ExecuteList();

        }

        [Test]
        public void QueryableListTest7()
        {
            QueryableListTest7_1(1);
        }

        private void QueryableListTest7_1(int id)
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile>()
                .Where(x => x.Id > id)
                .ExecuteList();
        }

        public class Test8
        {
            public static int GetId()
            {
                return 3;
            }
        }
        [Test]
        public void QueryableListTest8()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile>()
                .Where(x => x.Id > Test8.GetId())
                .ExecuteList();

        }

        private static int[] List1()
        {
            return new[] { 1, 2, 3, 4, 5, 6 };
        }
        [Test]
        public void QueryableListTest9()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile>()
                .Where(x => (object)x.Id == List1())
                .ExecuteList();
            //var type = List1().GetType();
            //Assert.IsTrue(type.IsArray);
            //Assert.IsTrue(type.IsSubclassOf(typeof(IEnumerable)));
            //Assert.IsFalse(type.IsSubclassOf(typeof(TestCaseData)));
        }
        [Test]
        public void QueryableListTest10()
        {
            var database = DatabaseHelper.GetHelper("test");

            var query = database.CreateQueryable<UserProfile>()
                .Where(x => (object)x.Id != List1());

            ParameterCollection p = database.CreateParamterCollection();
            var sql = query.OutputSqlString(p);

            Assert.IsNotNull(sql);
                //.ExecuteList();
            //var type = List1().GetType();
            //Assert.IsTrue(type.IsArray);
            //Assert.IsTrue(type.IsSubclassOf(typeof(IEnumerable)));
            //Assert.IsFalse(type.IsSubclassOf(typeof(TestCaseData)));
        }

        [Test]
        public void QueryablePageOfListTest1()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile>()
                .Where(x => (object)x.Id == (object)new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })//条件
                .RowLimit(0, 10)//分页
                .OrderBy(x => x.Id)//排序
                .ExecuteList();//返回列表

            Assert.IsNotNull(list);
        }

        [Test]
        public void QueryablePageOfListTest2()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile>()
                .Where(x => new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }.Contains(x.Id))
                .RowLimit(0, 10)
                .ExecuteList();

            Assert.IsNotNull(list);
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

            var array = new object[] { "test0", "test1", i };
            var list = new List<object> { "test0", "test1", i };
            var logicalExp5 = SqlExpression.Expression<UserProfile>(x => array.Contains(x.Name));
            var logicalExp6 = SqlExpression.Expression<UserProfile>(x => list.Contains(x.Name));
        }

        [Test]
        public void QueryTest000()
        {
            var sql = @"SELECT A.HID,A.HotelName,A.HotelStar,A.DefaultImgUrl,HotelIntroduction,A.Lat,A.Lng,A.CityID,A.RegionalInfo,T.LowPrice LowFee,T.Price ListPrice,T.ReturnAmount,A.HasWeixinSite,A.HotelFacilitie FROM HotelInfo(NOLOCK) A 
JOIN (
    SELECT Hid,LowPrice,Price,ReturnAmount
        FROM HotelSearch(NOLOCK) B 
        WHERE B.Hid >= 100000  and B.zhidingvisible = 1) T  ON A.Hid = T.Hid AND A.HotelStatus = 1   ";

            var database = DatabaseHelper.GetHelper("test");
            var list = database.ExecuteList<HotelSimpleInfoModel>(sql);

        }

        private void TestExpression<T>(Expression<Func<T, bool>> expression)
        {
            var value = Expression.Lambda<Func<object>>(Expression.MakeUnary(System.Linq.Expressions.ExpressionType.Convert, ((System.Linq.Expressions.BinaryExpression)expression.Body).Right, typeof(object))).Compile()();
            int val = (int)value;

        }
    }
}
