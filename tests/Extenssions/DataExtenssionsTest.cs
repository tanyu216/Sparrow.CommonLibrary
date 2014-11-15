using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sparrow.CommonLibrary.Extenssions;
using Sparrow.CommonLibrary.Database;

namespace Sparrow.CommonLibrary.Test.Extenssions
{
    [TestFixture]
    public class DataExtenssionsTest
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
            dt.Columns.Add("Note", typeof(string));

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
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        [Test]
        public void ScalarTest()
        {
            var dt = CreateDataTable();
            var val1 = dt.Scalar<int>();
            var val2 = dt.Scalar<int>("Id");
            var val3 = dt.Scalar<int>(0);

            var val5 = dt.Scalar<string>("Note");
            var val6 = dt.Scalar<string>(1);
        }

        [Test]
        public void DictionaryTest()
        {
            var dt = CreateDataTable();
            var dic1 = dt.ToDictionary<int, string>();//第0列为key，第1列为value
            var dic2 = dt.ToDictionary<int, string>("Id", "Note");
            var dic3 = dt.ToDictionary<int, string>(0, 1);//第0列为key，第1列为value

            var list1 = dt.GetValues<int>();//第0列
            var list2 = dt.GetValues<int>("Id");
            var list3 = dt.GetValues<int>(0);//第0列
            var list4 = dt.GetValues<string>(1);//第1列
        }

        [Test]
        public void DictionaryTest2()
        {
            using (var dt = DatabaseHelper.GetHelper("test").ExecuteReader("select id,name from UserProfile(nolock)"))
            {
                var dic2 = dt.ToDictionary<int, string>("Id", "name");
                Assert.IsNotNull(dic2);
                Assert.IsTrue(dic2.Count > 0);
            }
        }
    }
}
