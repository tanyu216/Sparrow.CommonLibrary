using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Data;
using Sparrow.CommonLibrary.Entity;

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
            var mapper = UserProfile.GetIMapper();
            Assert.IsNotNull(mapper);
        }

        [Test]
        public void CreateInstanceByDataMapper()
        {
            var mapper = UserProfile.GetIMapper();
            var user = mapper.Create();
            Assert.IsNotNull(user);
        }

        [Test]
        public void PropertyTest1()
        {
            var mapper = UserProfile.GetIMapper();
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
            var mapper = UserProfile.GetIMapper();
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
            var mapper = UserProfile.GetIMapper();

            Assert.AreEqual(mapper.MetaInfo[0].PropertyName, "Id");
            Assert.AreEqual(mapper.MetaInfo[1].PropertyName, "Name");
            Assert.AreEqual(mapper.MetaInfo.IndexOf("Email"), 3);

            Assert.IsNotNull(mapper.MetaInfo.Name);
            Assert.AreEqual(mapper.MetaInfo.GetPropertyNames()[3], "Email");
            Assert.AreEqual(mapper.MetaInfo.PropertyCount, 5);

            Assert.IsTrue(((DbMetaInfo)mapper.MetaInfo).IsKey("Id"));
            Assert.IsFalse(((DbMetaInfo)mapper.MetaInfo).IsKey("Sex"));
            Assert.AreEqual(((DbMetaInfo)mapper.MetaInfo).KeyCount, 1);
            Assert.AreEqual(((DbMetaInfo)mapper.MetaInfo).GetKeys()[0], "Id");

        }

        [Test]
        public void MetaInfoTest2()
        {
            var mapper = UserProfile.GetIMapper();
            var field1 = (DbMetaPropertyInfo)mapper.MetaInfo.GetProperties()[0];
            
            Assert.AreEqual(field1.PropertyName, "Id");
            Assert.IsTrue(field1.IsKey);


        }

        [Test]
        public void MapTest1()
        {
            var mapper = UserProfile.GetIMapper();
            var dt = CreateDataTable();
            var single = mapper.MapSingle(dt);
            Assert.IsNotNull(single);
        }

        [Test]
        public void MapTest2()
        {
            var mapper = UserProfile.GetIMapper();
            var dt = CreateDataTable();
            var list = mapper.MapList(dt);
            Assert.IsNotNull(list);
        }
    }
}
