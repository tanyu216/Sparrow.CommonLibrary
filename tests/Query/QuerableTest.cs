﻿using System;
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
                .OutputSqlString(parameter);

            Assert.IsNotNullOrEmpty(sql);
        }

        [Test]
        public void QueryableListTest1()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile>()
                .Where(x => (object)x.Id == (object)new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })
                .ExecuteList();

            Assert.IsNotNull(list);
        }

        [Test]
        public void QueryableListTest2()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile>()
                .Select(x => x.Id, x => x.Name)
                .Where(x => (object)x.Id == (object)new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })
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
        public void QueryablePageOfListTest1()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile>()
                .Where(x => (object)x.Id == (object)new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })
                .RowLimit(0, 10)
                .OrderBy(x => x.Id)
                .ExecuteList();

            Assert.IsNotNull(list);
        }

        [Test]
        public void QueryablePageOfListTest2()
        {
            var database = DatabaseHelper.GetHelper("test");

            var list = database.CreateQueryable<UserProfile>()
                .Where(x => (object)x.Id == (object)new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })
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

        }

        private void TestExpression<T>(Expression<Func<T, bool>> expression)
        {
            var value = Expression.Lambda<Func<object>>(Expression.MakeUnary(System.Linq.Expressions.ExpressionType.Convert, ((System.Linq.Expressions.BinaryExpression)expression.Body).Right, typeof(object))).Compile()();
            int val = (int)value;

        }
    }
}