using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Data;
using Sparrow.CommonLibrary.Entity;
using Sparrow.CommonLibrary.Mapper;

namespace Sparrow.CommonLibrary.Test.Mapper
{
    [TestFixture]
    public class ObjectAccessorTest
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
            var accessor = UserProfile.GetObjectAccessor();
            Assert.IsNotNull(accessor);
        }

        [Test]
        public void CreateInstanceByDataMapper()
        {
            var accessor = UserProfile.GetObjectAccessor();
            var user = accessor.Create();
            Assert.IsNotNull(user);
        }

        [Test]
        public void PropertyTest1()
        {
            var accessor = UserProfile.GetObjectAccessor();
            var user = accessor.Create();

            Assert.AreEqual(user.Id, 0);
            user.Id = 1;

            Assert.IsNull(user.Name);
            user.Name = "name";
            Assert.AreEqual(user.Name, "name");

        }

        [Test]
        public void PropertyTest2()
        {
            var accessor = UserProfile.GetObjectAccessor();
            var user = accessor.Create();

            Assert.AreEqual(user.Id, 0);
            accessor["Id"].SetValue(user, 1);
            Assert.AreEqual(user.Id, 1);

            Assert.IsNull(user.Name);
            accessor["Name"].SetValue(user, "name");
            Assert.AreEqual(accessor["Name"].GetValue(user), "name");

        }

        [Test]
        public void MetaInfoTest1()
        {
            var accessor = UserProfile.GetObjectAccessor();

            Assert.AreEqual(accessor.MetaInfo[0].PropertyName, "Id");
            Assert.AreEqual(accessor.MetaInfo[1].PropertyName, "Name");
            Assert.AreEqual(accessor.MetaInfo.IndexOf("Email"), 3);

            Assert.IsNotNull(accessor.MetaInfo.Name);
            Assert.AreEqual(accessor.MetaInfo.GetPropertyNames()[3], "Email");
            Assert.AreEqual(accessor.MetaInfo.PropertyCount, 6);

            Assert.IsTrue(((IDbMetaInfo)accessor.MetaInfo).IsKey("Id"));
            Assert.IsFalse(((IDbMetaInfo)accessor.MetaInfo).IsKey("Sex"));
            Assert.AreEqual(((IDbMetaInfo)accessor.MetaInfo).KeyCount, 1);
            Assert.AreEqual(((IDbMetaInfo)accessor.MetaInfo).GetKeys()[0], "Id");

        }

        [Test]
        public void MetaInfoTest2()
        {
            var accessor = UserProfile.GetObjectAccessor();
            var field1 = (DbMetaPropertyInfo)accessor.MetaInfo.GetProperties()[0];
            
            Assert.AreEqual(field1.PropertyName, "Id");
            Assert.IsTrue(field1.IsKey);


        }

        [Test]
        public void MapTest1()
        {
            var dt = CreateDataTable();
            var single = Map.Single<UserProfile>(dt);

            Assert.IsNotNull(single);
        }

        [Test]
        public void MapTest2()
        {
            var dt = CreateDataTable();
            var list = Map.List<UserProfile>(dt);

            Assert.IsNotNull(list);
        }

        [Test]
        public void MapTest3()
        {
            var dt = CreateDataTable();
            var list = Map.List<UserProfile2>(dt);
            Assert.IsNotNull(list);
        }

        [Test]
        public void MapTest4()
        {
            var dt = CreateDataTable();
            var list = Map.List<UserProfile3>(dt);
            Assert.IsNotNull(list);
        }

        [Test]
        public void MapTest5()
        {

            var user = Map.Create<UserProfile>();
            user.Id = 1;
            user.Email = "newemail@126.com";


        }
    }
}
