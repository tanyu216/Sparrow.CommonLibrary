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
    public class FieldFlagTest
    {
        [Test]
        public void Test1()
        {
            var flag = new FieldFlag(26);
            flag.Mark(1);
            flag.Mark(23);
            Assert.IsFalse(flag.HasMarked(0));
            Assert.IsTrue(flag.HasMarked(1));
            Assert.IsFalse(flag.HasMarked(2));
            Assert.IsFalse(flag.HasMarked(3));
            Assert.IsFalse(flag.HasMarked(22));
            Assert.IsTrue(flag.HasMarked(23));
        }

        [Test]
        public void Test2()
        {
            var flag = new FieldFlag(32);
            flag.Mark(1);
            flag.Mark(23);
            flag.Mark(30);
            flag.Mark(31);
            Assert.IsFalse(flag.HasMarked(0));
            Assert.IsTrue(flag.HasMarked(1));
            Assert.IsFalse(flag.HasMarked(2));
            Assert.IsFalse(flag.HasMarked(3));
            Assert.IsFalse(flag.HasMarked(22));
            Assert.IsTrue(flag.HasMarked(23));
            Assert.IsFalse(flag.HasMarked(24));
            Assert.IsTrue(flag.HasMarked(30));
            Assert.IsTrue(flag.HasMarked(31));
        }

        [Test]
        public void Test3()
        {
            var flag = new FieldFlag(31);
            flag.Mark(1);
            flag.Mark(23);
            flag.Mark(30);
            Assert.IsFalse(flag.HasMarked(0));
            Assert.IsTrue(flag.HasMarked(1));
            Assert.IsFalse(flag.HasMarked(2));
            Assert.IsFalse(flag.HasMarked(3));
            Assert.IsFalse(flag.HasMarked(22));
            Assert.IsTrue(flag.HasMarked(23));
            Assert.IsFalse(flag.HasMarked(24));
            Assert.IsTrue(flag.HasMarked(30));
        }

        [Test]
        public void Test4()
        {
            var flag = new FieldFlag(64);
            flag.Mark(1);
            flag.Mark(23);
            flag.Mark(30);
            flag.Mark(32);
            flag.Mark(32);
            flag.Mark(33);
            flag.Mark(63);
            Assert.IsFalse(flag.HasMarked(0));
            Assert.IsTrue(flag.HasMarked(1));
            Assert.IsFalse(flag.HasMarked(2));
            Assert.IsFalse(flag.HasMarked(3));
            Assert.IsFalse(flag.HasMarked(22));
            Assert.IsTrue(flag.HasMarked(23));
            Assert.IsFalse(flag.HasMarked(24));
            Assert.IsTrue(flag.HasMarked(30));
            Assert.IsTrue(flag.HasMarked(32));
            Assert.IsTrue(flag.HasMarked(33));
            Assert.IsTrue(flag.HasMarked(63));
        }

        [Test]
        public void Test5()
        {
            var flag = new FieldFlag(65);
            flag.Mark(1);
            flag.Mark(23);
            flag.Mark(30);
            flag.Mark(32);
            flag.Mark(32);
            flag.Mark(33);
            flag.Mark(63);
            flag.Mark(64);
            Assert.IsFalse(flag.HasMarked(0));
            Assert.IsTrue(flag.HasMarked(1));
            Assert.IsFalse(flag.HasMarked(2));
            Assert.IsFalse(flag.HasMarked(3));
            Assert.IsFalse(flag.HasMarked(22));
            Assert.IsTrue(flag.HasMarked(23));
            Assert.IsFalse(flag.HasMarked(24));
            Assert.IsTrue(flag.HasMarked(30));
            Assert.IsTrue(flag.HasMarked(32));
            Assert.IsTrue(flag.HasMarked(33));
            Assert.IsTrue(flag.HasMarked(63));
            Assert.IsTrue(flag.HasMarked(64));
        }

        [Test]
        public void Test6()
        {
            var flag = new FieldFlag(96);
            flag.Mark(1);
            flag.Mark(23);
            flag.Mark(30);
            flag.Mark(32);
            flag.Mark(32);
            flag.Mark(33);
            flag.Mark(63);
            flag.Mark(65);
            flag.Mark(95);
            Assert.IsFalse(flag.HasMarked(0));
            Assert.IsTrue(flag.HasMarked(1));
            Assert.IsFalse(flag.HasMarked(2));
            Assert.IsFalse(flag.HasMarked(3));
            Assert.IsFalse(flag.HasMarked(22));
            Assert.IsTrue(flag.HasMarked(23));
            Assert.IsFalse(flag.HasMarked(24));
            Assert.IsTrue(flag.HasMarked(30));
            Assert.IsTrue(flag.HasMarked(32));
            Assert.IsTrue(flag.HasMarked(33));
            Assert.IsTrue(flag.HasMarked(63));
            Assert.IsFalse(flag.HasMarked(64));
            Assert.IsTrue(flag.HasMarked(65));
            Assert.IsTrue(flag.HasMarked(95));

        }

        [Test]
        public void Test7()
        {
            var flag = new FieldFlag(98);
            flag.Mark(1);
            flag.Mark(23);
            flag.Mark(30);
            flag.Mark(32);
            flag.Mark(32);
            flag.Mark(33);
            flag.Mark(63);
            flag.Mark(65);
            flag.Mark(95);
            flag.Mark(97);
            Assert.IsFalse(flag.HasMarked(0));
            Assert.IsTrue(flag.HasMarked(1));
            Assert.IsFalse(flag.HasMarked(2));
            Assert.IsFalse(flag.HasMarked(3));
            Assert.IsFalse(flag.HasMarked(22));
            Assert.IsTrue(flag.HasMarked(23));
            Assert.IsFalse(flag.HasMarked(24));
            Assert.IsTrue(flag.HasMarked(30));
            Assert.IsTrue(flag.HasMarked(32));
            Assert.IsTrue(flag.HasMarked(33));
            Assert.IsTrue(flag.HasMarked(63));
            Assert.IsFalse(flag.HasMarked(64));
            Assert.IsTrue(flag.HasMarked(65));
            Assert.IsTrue(flag.HasMarked(95));
            Assert.IsFalse(flag.HasMarked(96));
            Assert.IsTrue(flag.HasMarked(97));
        }
    }
}
