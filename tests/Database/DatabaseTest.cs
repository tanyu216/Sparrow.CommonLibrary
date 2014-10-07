using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Test.Mapper;
using Sparrow.CommonLibrary.Mapper;

namespace Sparrow.CommonLibrary.Test.Database
{
    [TestFixture]
    public class DatabaseTest
    {
        [Test]
        public void Test1()
        {
            //没有参数的查询，返回一个列表集合
            var db = DatabaseHelper.GetHelper("test");//test是一个数据库连接
            var list = db.ExecuteList<UserProfile3>("select * from UserProfile(nolock)");

            //参数化查询，返回一个列表集合
            var pars = db.CreateParamterCollection(3);
            pars.Append("sex", 1);
            pars.Append("name", "test");
            var list2 = db.ExecuteList<UserProfile3>(@"select * from UserProfile(nolock) 
                                                        where Sex=@sex and Name like @name+'%'", pars);

            //参数化查询，返回一个列表集合
            var list3 = db.ExecuteList<UserProfile3>(@"select * from UserProfile(nolock) 
                                                        where Sex={0} and Name like {1}+'%'", 1, "test");

            //参数化查询,只返回一行数据
            var user1 = db.ExecuteFirst<UserProfile3>("select * from UserProfile(nolock) where id={0}", 2);


            //执行一组查询，返回DataSet。适合大数据集合操作
            var batch = new SqlBatch();
            batch.Append("select * from UserProfile(nolock)");
            batch.AppendFormat(@"select * from UserProfile(nolock) 
                                  where Sex={0} and Name like {1}+'%'", 1, "test");
            var ds = db.ExecuteDataSet(batch);
            var list4 = Map.List<UserProfile3>(ds.Tables[0]);
            var list5 = Map.List<UserProfile3>(ds.Tables[1]);

            //执行一组查询，返回IDataReader。连接字符串中需要加入配置节：MultipleActiveResultSets=True
            //适合小数据集操作，只有两三个数据集合，且每个集合数据量只有几十条。
            var batch2 = new SqlBatch();
            batch.Append("select * from UserProfile(nolock)");
            batch.AppendFormat(@"select * from UserProfile(nolock) 
                                  where Sex={0} and Name like {1}+'%'", 1, "test");
            using (var reader = db.ExecuteReader(batch))
            {
                var list6 = Map.List<UserProfile3>(reader);
                reader.NextResult();
                var list7 = Map.List<UserProfile3>(reader);
            }

            Assert.IsNotNull(list);
            Assert.Greater(list.Count, 0);

            Assert.IsNotNull(list2);
            Assert.Greater(list2.Count, 0);

            Assert.IsNotNull(list3);
            Assert.Greater(list3.Count, 0);
        }
    }
}
