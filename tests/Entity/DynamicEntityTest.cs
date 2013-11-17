using NUnit.Framework;
using Sparrow.CommonLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Test.Entity
{
    [TestFixture]
    public class DynamicEntityTest
    {
        [Test]
        public void EntityTest1()
        {
            dynamic entity = new DynamicEntity("test", new[] { "Id" });
            entity.Id = 1;
            entity.Name = "Test";
            entity.Email = "test@hotmail.com";
            entity.Note = (string)null;

            Assert.AreEqual(entity.Id, 1);
            Assert.AreEqual(entity.Name, "Test");
            Assert.AreEqual(entity.Email, "test@hotmail.com");
            Assert.IsNull(entity.Note);
        }

        [Test]
        public void EntityExplain1()
        {
            dynamic entity = new DynamicEntity("Test", new[] { "Id" });
            entity.Id = 1;
            entity.Name = "Test";
            entity.Email = "test@hotmail.com";
            entity.Note = (string)null;

            var expl = (IEntityExplain)entity;
            Assert.IsTrue(expl.AnySetted());
            Assert.AreEqual(expl.ColumnCount, 4);
            Assert.AreEqual(expl.GetColumnNames()[3], "Note");
            Assert.AreEqual(expl.GetFieldValues(expl.GetKeys()).First().Value, 1);
            Assert.AreEqual(expl.GetSettedFields().Skip(1).First(), "Name");
            Assert.IsNull(expl.Increment);
            Assert.IsTrue(expl.IsKey("Id"));
            Assert.IsTrue(expl.IsSetted(1));
            Assert.AreEqual(expl.KeyCount, 1);
            Assert.AreEqual(expl.OperationState, DataState.New);
            Assert.AreEqual(expl.TableName, "Test");

        }

        [Test]
        public void EntityExplain2()
        {
            dynamic entity = new DynamicEntity("Test", new[] { "Id" }, "Id", "TestId");
            entity.Id = 1;
            entity.Name = "Test";
            entity.Email = "test@hotmail.com";
            entity.Note = (string)null;

            var expl = (IEntityExplain)entity;
            Assert.IsNotNull(expl.Increment);
            Assert.AreEqual(expl.Increment.ColumnName, "Id");
            Assert.AreEqual(expl.Increment.IncrementName, "TestId");
            Assert.IsTrue(expl.Increment.IsKey);
            Assert.AreEqual(expl.Increment.StartVal, 1);
        }

    }
}
