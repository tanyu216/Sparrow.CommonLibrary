using NUnit.Framework;
using Sparrow.CommonLibrary.Retrying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Test.Retrying
{
    [TestFixture]
    public class IncrementalIntervalTest
    {

        [Test]
        public void DoTest1()
        {
            var retry = new IncrementalInterval();
            var i = 0;

            retry.DoExecute(() => { if (i++ < 3) { throw new Exception("test"); } });

        }

        [Test]
        public void DoTest2()
        {
            var retry = new IncrementalInterval(3, new TimeSpan(0, 0, 3), false);
            var i = 0;

            retry.DoExecute(() => { if (i++ < 3) { throw new Exception("test"); } });

        }

        [Test]
        public void DoTest3()
        {
            var retry = new IncrementalInterval(3, new TimeSpan(0, 0, 3), false);

            try
            {
                retry.DoExecute(() => { throw new Exception("test"); });
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "test");
            }

        }

        [Test]
        public void DoTest4()
        {
            var retry = new IncrementalInterval(5, new TimeSpan(0, 0, 2));
            var list = new List<TimeSpan>();
            var i = 0;

            retry.OnRetrying += (sender, e) => { list.Add(e.Delay); };
            retry.DoExecute(() => { if (i++ < 4) { throw new Exception("test"); } });

            Assert.AreEqual(list.Count, 4);
            Assert.AreEqual(list[0], new TimeSpan(0, 0, 2));
            Assert.AreEqual(list[1], new TimeSpan(0, 0, 4));
            Assert.AreEqual(list[2], new TimeSpan(0, 0, 6));
            Assert.AreEqual(list[3], new TimeSpan(0, 0, 8));

        }

    }
}
