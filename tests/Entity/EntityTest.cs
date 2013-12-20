using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparrow.CommonLibrary.Entity;
using Sparrow.CommonLibrary.Mapper;
using Sparrow.CommonLibrary.Test.Mapper;

namespace Sparrow.CommonLibrary.Test.Entity
{
    [TestFixture]
    public class EntityTest
    {
        [Test]
        public void Test1()
        {
            var accessor = Map.GetAccessor<UserProfile>();
            var user = accessor.Create();
            user.Id = 1;
            user.Name = "test";
            user.Email = "test@hotmail.com";

            var entity = (IEntity)user;
            Assert.IsTrue(entity.AnySetted());
            Assert.IsTrue(entity.IsSetted(0));
            Assert.IsFalse(entity.IsSetted(2));
            Assert.AreEqual(entity.EntityType, typeof(UserProfile));
            Assert.AreEqual(entity.OperationState, DataState.New);

        }
    }
}
