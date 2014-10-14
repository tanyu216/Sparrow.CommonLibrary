using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sparrow.CommonLibrary.Database;
using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Repository;
using Sparrow.CommonLibrary.Test.Mapper;

namespace Sparrow.CommonLibrary.Test.Repository
{
    [TestFixture]
    public class RepositoryTest
    {
        private IRepository<UserProfile> Create()
        {
            var db = DatabaseHelper.GetHelper("test");
            var repos = new RepositoryDatabase<UserProfile>(db);
            return repos;
        }

        [Test]
        public void IsnertTest()
        {
            var db = DatabaseHelper.GetHelper("test");
            var repos = new RepositoryDatabase<UserProfile>(db);
            var userprofile = Map.Create<UserProfile>();
            /*
             * Map.Create<UserProfile>()创建一个实体对象，与 new UserProfile()类似。
             * 但它们不同在于，Map.Create<UserProfile>()创建的对象可以探测到只赋值过的属性
             */
            userprofile.Name = "test";
            userprofile.Email = "test@hotmail.com";
            userprofile.FixPhone = "860108888888";
            userprofile.Sex = 1;
            repos.Insert(userprofile);

            var insertedUser = repos.Get(userprofile.Id);
            Assert.IsNotNull(insertedUser);
            Assert.AreEqual(userprofile.Id, insertedUser.Id);
            Assert.AreEqual(userprofile.Name, insertedUser.Name);
            Assert.AreEqual(userprofile.Email, insertedUser.Email);
            Assert.AreEqual(userprofile.FixPhone, insertedUser.FixPhone);
        }

        [Test]
        public void UpdateTest()
        {
            var db = DatabaseHelper.GetHelper("test");
            var repos = new RepositoryDatabase<UserProfile>(db);
            var userprofile = Map.Create<UserProfile>();
            /*
             * 修改一行数据时，Map.Create<UserProfile>()与new UserProfile()对象完全会达到不一样的效果。
             * 以下场景是所有表操作经常出现的一种，
             * Map.Create<UserProfile>()创建的对象,Repository会知道对象哪些属性被赋值过，只生成有过赋值的属性。
             *      如：UPDATE UserProfile SET Email=@email WHERE Id=@id
             * 如果使用new UserProfile()
             *      如：UPDATE UserProfile SET Name=@name,Email=@email,FixPhone=@fixphone....... WHERE Id=@id
             */
            userprofile.Id = 1;
            userprofile.Email = "update@126.com";
            repos.Update(userprofile);

        }

        [Test]
        public void IsnertOrUpdateTest()
        {
            var db = DatabaseHelper.GetHelper("test");
            var repos = new RepositoryDatabase<UserProfile>(db);
            var userprofile = Map.Create<UserProfile>();
            userprofile.Id = 1;
            userprofile.Name = "test";
            userprofile.Email = "test@hotmail.com";
            userprofile.FixPhone = "860108888888";
            userprofile.Sex = 1;
            repos.Save(userprofile);
        }

        [Test]
        public void QueryTest()
        {
            var db = DatabaseHelper.GetHelper("test");
            var repos = new RepositoryDatabase<UserProfile>(db);

            var list1 = repos.GetList(x => (object)x.Id == new[] { 1, 2, 3, 4, 5 });
            //where Sex=1 and Name like 'test%'
            var list2 = repos.GetList(x => x.Sex == 1 && x.Name.StartsWith("test"));
            var list3 = repos.GetList(x => x.Id > 1 && x.Id < 10 && (x.Sex == 1 || x.Sex == 2));

            Assert.IsNotNull(list1);
            Assert.Greater(list1.Count, 0);
            Assert.IsNotNull(list1);
            Assert.Greater(list2.Count, 0);
        }

        [Test]
        public void QueryTest2()
        {
            var db = DatabaseHelper.GetHelper("test");
            var repos = new RepositoryDatabase<UserProfile>(db);

            var obj1 = repos.Get(1);
            var obj2 = repos.Get(x => x.Email == "123@126.com" && x.Sex == 1);

            var list1 = repos.GroupbyCount<int>(x => x.Sex, x => x.Id > 1 && x.Id < 1000);

            var list2 = repos.GroupbyMax<int, int>(x => x.Sex, x => x.Id, x => x.Id > 1 && x.Id < 1000);

            var list3 = repos.GroupbySum<int, int>(x => x.Sex, x => x.Id, x => x.Id > 1 && x.Id < 1000);

            var sumValue = repos.Sum<int>(x => x.Id, x => x.Id > 1 && x.Id < 1000 && x.Sex == 1);

            var maxValue = repos.Max<int>(x => x.Id, x => x.Id > 1 && x.Id < 1000 && x.Sex == 1);

        }

        [Test]
        public void SqlTest()
        {
            var sql = "update Account set money+=@money where id=@id";

            var db = DatabaseHelper.GetHelper("test");
            var repos = new RepositoryDatabase<UserProfile>(db);


            var paras = db.CreateParamterCollection();
            var param1 = paras.Append("money", 10);
            param1.Size = 10;
            param1.Direction = System.Data.ParameterDirection.Output;

            paras.Append("id", 1);

            base.DoExeucte(sql,paras);
        }
    }
}
