using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sparrow.CommonLibrary.Common;

namespace Sparrow.CommonLibrary.Test.Common
{
    [TestFixture]
    public class BufferedTest
    {
        Buffered<int> buffered;
        [SetUp]
        public void Init()
        {
            buffered = new Buffered<int>(500);
            buffered.MaxBuferSize = 100000;
            buffered.OnFlush += buffered_OnFlush;
        }

        void buffered_OnFlush(object sender, BufferedFlushEventArgs<int> e)
        {
            Console.WriteLine("输出缓冲对象：{0}个", e.List.Count);
        }

        [TearDown]
        public void End()
        {
            buffered.Dispose();
        }

        [Test]
        public void Test()
        {
            for (var i = 0; i < 100000; i++)
            {
                buffered.Write(i);
                if (i % 100 == 0)
                    System.Threading.Thread.Sleep(5);
            }
        }
    }
}
