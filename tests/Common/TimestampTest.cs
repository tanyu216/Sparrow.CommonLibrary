using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparrow.CommonLibrary.Common;
using Sparrow.CommonLibrary.Extenssions;

namespace Sparrow.CommonLibrary.Test.Common
{
    [TestFixture]
    public class TimestampTest
    {
        [Test]
        public void Test1()
        {
            var now = DateTime.Now.ToUniversalTime();
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
            var timestamp = (Timestamp)now;
            var datetime = (DateTime)timestamp;
            Assert.AreEqual(now, datetime);

            var longVal = (long)timestamp;
            Assert.AreEqual(timestamp, (Timestamp)longVal);
        }

        [Test]
        public void Test2()
        {
            var timestamp = Timestamp.Now;
            var datetime = (DateTime)timestamp;
            var longVal = (long)timestamp;
            Assert.AreEqual(timestamp, longVal);
        }

        [Test]
        public void Test3()
        {
            var now = Timestamp.Now;
            var now2 = Timestamp.Now - (Timestamp)100000;
            Assert.IsTrue(now > now2);
            Assert.IsTrue(now2 < now);
            Assert.IsTrue(now >= now2);
            Assert.IsTrue(now2 <= now);
            Assert.IsFalse(now == now2);
            Assert.IsTrue(now != now2);


            var now3 = now2 + (Timestamp)100000;
            Assert.IsTrue(now == now3);

            var now4 = now - new TimeSpan(0, 0, 100000);
            var now5 = now4 + new TimeSpan(0, 0, 100000);
            Assert.IsTrue(now4 < now5);
            Assert.IsTrue(now == now5);
        }

        [Test]
        public void Test4()
        {
            var val = 100000;
            var timestamp = val.Cast<Timestamp>();
        }

    }
}
