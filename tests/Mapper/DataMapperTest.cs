using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Data;

namespace Sparrow.CommonLibrary.Test.Mapper
{
    [TestFixture]
    public class DataMapperTest
    {
        public DataTable CreateDataTable()
        {
            var charList = new StringBuilder();
            for (var i = 0; i < 26; i++)
            {
                charList.Append((char)(97 + i));
            }

            var rand = new Random();
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Email", typeof(string));

            for (var i = 1; i <= 1000; i++)
            {
                var dataRow = dt.NewRow();
                dataRow[0] = i;
                var len = rand.Next(1, 100);
                var note = new StringBuilder();
                for (var m = 0; m < len; m++)
                {
                    note.Append(charList[rand.Next(0, 26)]);
                }
                dataRow[1] = note.ToString();
                dataRow[2] = "test@hotmail.com";
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        [SetUp]
        public void Init()
        {
        }

        [TearDown]
        public void Clean()
        {
        }

        [Test]
        public void CreateDataMapper()
        {
            var mapper = UserProfile.GetMapper();
            Assert.IsNotNull(mapper);
        }

        [Test]
        public void CreateInstanceByDataMapper()
        {
            var mapper = UserProfile.GetMapper();
            var user = mapper.Create();
            Assert.IsNotNull(user);
        }

        [Test]
        public void PropertyTest1()
        {
            var mapper = UserProfile.GetMapper();
            var user = mapper.Create();

            Assert.AreEqual(user.Id, 0);
            user.Id = 1;

            Assert.IsNull(user.Name);
            user.Name = "name";
            Assert.AreEqual(user.Name, "name");

        }

        [Test]
        public void PropertyTest2()
        {
            var mapper = UserProfile.GetMapper();
            var user = mapper.Create();

            Assert.AreEqual(user.Id, 0);
            mapper["Id"].SetValue(user, 1);
            Assert.AreEqual(user.Id, 1);

            Assert.IsNull(user.Name);
            mapper["Name"].SetValue(user, "name");
            Assert.AreEqual(mapper["Name"].GetValue(user), "name");

        }

        [Test]
        public void MetaInfoTest1()
        {
            var mapper = UserProfile.GetMapper();

            Assert.AreEqual(mapper.FieldName(0), "Id");
            Assert.AreEqual(mapper.FieldName(1), "Name");
            Assert.AreEqual(mapper.IndexOf("Email"), 3);

            Assert.IsNull(mapper.MetaInfo.Name);
            Assert.IsNotNull(mapper.MetaInfo.GetFieldNames());
            Assert.IsNotNull(mapper.MetaInfo.GetKeys());

            Assert.IsTrue(mapper.MetaInfo.IsKey("Id"));
            Assert.IsFalse(mapper.MetaInfo.IsKey("Sex"));

            Assert.AreEqual(mapper.MetaInfo.FieldCount, 5);
            Assert.AreEqual(mapper.MetaInfo.KeyCount, 1);

        }

        [Test]
        public void MetaInfoTest2()
        {
            var mapper = UserProfile.GetMapper();
            var field1 = mapper.MetaInfo.GetFields()[0];
            var field2 = mapper.MetaInfo.GetFields()[3];

            Assert.AreEqual(field1.FieldName, "Id");
            Assert.AreEqual(field2.FieldName, "Email");
            Assert.IsTrue(field1.IsKey);
            Assert.IsFalse(field2.IsKey);

        }

        [Test]
        public void MapTest1()
        {
            var mapper = UserProfile.GetMapper();
            var dt = CreateDataTable();
            var single = mapper.MapSingle(dt);
            Assert.IsNotNull(single);
        }

        [Test]
        public void MapTest2()
        {
            var mapper = UserProfile.GetMapper();
            var dt = CreateDataTable();
            var list = mapper.MapList(dt);
            Assert.IsNotNull(list);
        }
    }
}
