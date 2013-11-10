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
            var mapper = Map.GetIMapper<UserProfile>();
            var user = mapper.Create();
            user.Id = 1;
            user.Name = "test";
            user.Email = "test@hotmail.com";

            var explain = new EntityExplain(user);
            Assert.AreEqual(explain.FieldCount, 5);
            Assert.AreEqual(explain.KeyCount, 1);
            Assert.AreEqual(explain.GetFieldNames()[0], "Id");
            Assert.AreEqual(explain.GetFieldNames()[1], "Name");
            Assert.AreEqual(explain.GetKeys()[0], "Id");
            Assert.AreEqual(explain.GetFields()[1].FieldName, "Name");

            Assert.AreEqual(explain.GetSettedFields().Skip(1).First(), "Name");
            Assert.AreEqual(explain.GetValues(explain.GetSettedFields()).Skip(1).First().Value, "test");
            Assert.IsTrue(explain.IsSetted(0));
            Assert.IsFalse(explain.IsSetted(4));
        }
    }
}
