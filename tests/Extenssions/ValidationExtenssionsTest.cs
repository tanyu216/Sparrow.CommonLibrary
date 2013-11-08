using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sparrow.CommonLibrary.Extenssions;

namespace Sparrow.CommonLibrary.Test.Extenssions
{
    [TestFixture]
    public class ValidationExtenssionsTest
    {
        [Test]
        public void IsDigitsTest()
        {
            Assert.IsTrue("1234".IsDigits());
            Assert.IsFalse("1234,".IsDigits());
            Assert.IsTrue("01234".IsDigits());
            Assert.IsFalse("1234.1".IsDigits());

        }

        [Test]
        public void IsNumberTest()
        {
            Assert.IsTrue("1234".IsNumber());
            Assert.IsFalse("1234,".IsNumber());
            Assert.IsTrue("01234".IsNumber());
            Assert.IsTrue("1234.1".IsNumber());

        }

        [Test]
        public void IsEmailTest()
        {
            Assert.IsTrue("tanyu216@hotmail.com".IsEmail());
            Assert.IsTrue("tanyu216@126.com".IsEmail());
            Assert.IsTrue("tanyu_216@126.com".IsEmail());
            Assert.IsTrue("tanyu-216@126.com".IsEmail());
            Assert.IsFalse("tanyu216@.com".IsEmail());
            Assert.IsFalse("tanyu216@".IsEmail());
            Assert.IsFalse("tanyu216".IsEmail());
        }

        [Test]
        public void IsFixPhoneCHNTest()
        {
            Assert.IsTrue("59356309".IsFixPhoneCHN());
            Assert.IsTrue("8659356309".IsFixPhoneCHN());
            Assert.IsTrue("+8659356309".IsFixPhoneCHN());
            Assert.IsTrue("01059356309".IsFixPhoneCHN());
            Assert.IsTrue("8601059356309".IsFixPhoneCHN());
            Assert.IsTrue("+8601059356309".IsFixPhoneCHN());
            Assert.IsTrue("5935630".IsFixPhoneCHN());
            Assert.IsFalse("959356309".IsFixPhoneCHN());
        }

        [Test]
        public void IsMobilePhoneCHNTest()
        {
            Assert.IsTrue("15801121212".IsMobilePhoneCHN());
            Assert.IsTrue("+8615801121212".IsMobilePhoneCHN());
            Assert.IsFalse("015801121212".IsMobilePhoneCHN());
            Assert.IsFalse("1580112121".IsMobilePhoneCHN());
            Assert.IsFalse("01015801121212".IsMobilePhoneCHN());
            Assert.IsFalse("25801121212".IsMobilePhoneCHN());
        }

        [Test]
        public void IsUrlTest()
        {
            Assert.IsTrue("http://www.test.com".IsUrl());
            Assert.IsTrue("https://www.test.com".IsUrl());
            Assert.IsTrue("http://www.test.com?id=1".IsUrl());
            Assert.IsTrue("http://www.test.com?id=1&name=test".IsUrl());
            Assert.IsFalse("www.test.com".IsUrl());
            Assert.IsFalse("http://www.test.com---".IsUrl());
            Assert.IsFalse("http:/www.test.com".IsUrl());
            Assert.IsFalse("http://.test.com".IsUrl());
            Assert.IsFalse("http://www.test.com1111".IsUrl());
            Assert.IsFalse("http://www.test.com1111?".IsUrl());
            Assert.IsFalse("http://www.test.com&11111".IsUrl());
        }

        [Test]
        public void IsDateTest()
        {
            Assert.IsTrue("2013-1-1".IsDateTime());
            Assert.IsTrue("2013-1-1 2:30:50".IsDateTime());
            Assert.IsTrue("2013/1/1 2:30:50".IsDateTime());
            Assert.IsFalse("2013-1 2:30:50".IsDateTime());
            Assert.IsFalse("2013-13-1 2:30:50".IsDateTime());
        }

        [Test]
        public void IsDateTimeTest()
        {
            Assert.IsTrue("2013-1-1".IsDateISO());
            Assert.IsTrue("2013/1/1".IsDateISO());
            Assert.IsFalse("2013-1".IsDateISO());
            Assert.IsFalse("2013-13-1".IsDateISO());
        }

        [Test]
        public void IsCreditCardTest()
        {
            //Assert.IsTrue("5201****1159****".IsCreditCard());
            //Assert.IsTrue("5201-****-1159-****".IsCreditCard());
            //Assert.IsTrue("5201 **** 1159 ****".IsCreditCard());
            Assert.IsFalse("4408041234567890".IsCreditCard());
            Assert.IsFalse("4408-0412-3456-7890".IsCreditCard());
            Assert.IsFalse("4408 0412 3456 7890".IsCreditCard());
        }

        [Test]
        public void IsIDCardCHNTest()
        {
            //Assert.IsTrue("430211***".IsIDCardCHN());
            Assert.IsFalse("430211191202161811".IsIDCardCHN());
        }
    }
}
