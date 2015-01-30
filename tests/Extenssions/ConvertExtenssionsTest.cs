using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparrow.CommonLibrary.Extenssions;

namespace Sparrow.CommonLibrary.Test.Extenssions
{
    [TestFixture]
    public class ConvertExtenssionsTest
    {
        [Test]
        public void CastTest1()
        {
            object obj = 1;
            int val = obj.Cast<int>();
            decimal val2 = obj.Cast<decimal>();
            long val3 = obj.Cast<long>();
            float val4 = obj.Cast<float>();
            short val5 = obj.Cast<short>();

        }

        [Test]
        public void CastTest2()
        {
            object obj = "1";
            int val = obj.Cast<int>();
            decimal val2 = obj.Cast<decimal>();
            long val3 = obj.Cast<long>();
            float val4 = obj.Cast<float>();
            short val5 = obj.Cast<short>();
        }


        [Test]
        public void CastTest3()
        {

            int val = "1".ToInt();
            decimal val2 = "1.2".ToDecimal();
            var val3 = "1".ToLong();
            val3 = "1.".ToLong();
            val3 = "1.1".ToLong();
            DateTime date = "2014-1-1".ToDateTime();
            bool b = "true".ToBoolean();

        }

        [Test]
        public void CastTest4()
        {
            int? val1 = "".ToNullableInt();
            Assert.AreEqual(val1, null);

            int? val2 = "2".ToNullableInt();
            Assert.AreEqual(val2, 2);

            int? val3 = ",".ToNullableInt();
            Assert.AreEqual(val3, null);

        }

        [Test]
        public void CastTest5()
        {
            decimal? val1 = "".ToNullableDecimal();

            decimal? val2 = "2".ToNullableDecimal();

            decimal? val3 = ",".ToNullableDecimal();


            Assert.AreEqual(val2, 2);
            Assert.AreEqual(val1, null);
            Assert.AreEqual(val3, null);

        }

        [Test]
        public void CastTest6()
        {
            string val1 = "".ToSafeString();
            Assert.AreEqual(val1, "");

            string val2 = ((string)null).ToSafeString();
            Assert.AreEqual(val1, string.Empty);

        }

        [Test]
        public void CastTest7()
        {
            bool val1 = "".ToBoolean();
            Assert.AreEqual(val1, false);

            bool val2 = "false".ToBoolean();
            Assert.AreEqual(val2, false);

            bool val3 = "true".ToBoolean();
            Assert.AreEqual(val3, true);

            bool val4 = "True".ToBoolean();
            Assert.AreEqual(val4, true);

            bool val5 = "TRUE".ToBoolean();
            Assert.AreEqual(val5, true);
        }

        [Test]
        public void CastTest8()
        {
            decimal? val1 = "2".ToNullableDecimal();//2
            decimal? val2 = "2，".ToNullableDecimal();//转换失败返回：null
            int? val3 = "2".ToNullableInt();//2
            int? val4 = "2，".ToNullableInt();//转换失败返回：null
            string safestring = ((string)null).ToSafeString();//可以必免发生空引用异常
            string safestring2 = "123".ToSafeString();
        }
    }
}
