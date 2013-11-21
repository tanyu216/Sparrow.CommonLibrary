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
            var repos = Create();
            var userprofile = Map.Create<UserProfile>();
            userprofile.Name = "test" + DateTime.Now.ToShortDateString();
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
    }
}
