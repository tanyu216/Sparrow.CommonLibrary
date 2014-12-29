using NUnit.Framework;
using Sparrow.CommonLibrary.Entity;
using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Test.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Test.Entity
{
    [TestFixture]
    public class EntityExplainTest
    {
        [Test]
        public void Test1()
        {
            var accessor = Map.GetAccessor<UserProfile>();
            Assert.IsNotNull(accessor);

            var dbMetaInfo = accessor.MetaInfo as IDbMetaInfo;
            var user = accessor.Create();
            user.Id = 1;
            user.Name = "test";
            user.Email = "test@hotmail.com";

            Assert.AreEqual(accessor.MetaInfo.PropertyCount, 6);
            Assert.AreEqual(dbMetaInfo.KeyCount, 1);
            Assert.AreEqual(accessor.MetaInfo.GetPropertyNames()[0], "Id");
            Assert.AreEqual(accessor.MetaInfo.GetPropertyNames()[1], "Name");
            Assert.AreEqual(dbMetaInfo.GetKeys()[0], "Id");
            Assert.AreEqual(accessor.MetaInfo.GetProperties()[1].PropertyName, "Name");

            var explain = new EntityExplain(user);
            Assert.AreEqual(explain["Id"], 1);
            Assert.AreEqual(explain.GetSettedFields().Skip(1).First(), "Name");
            Assert.AreEqual(explain.GetFieldValues(explain.GetSettedFields()).Skip(1).First().Value, "test");
            Assert.IsTrue(explain.IsSetted(0));
            Assert.IsFalse(explain.IsSetted(4));
        }
    }
}
