using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Sparrow.CommonLibrary.Cryptography;
using Sparrow.CommonLibrary.Cryptography.HashAlgorithm;

namespace Sparrow.CommonLibrary.Test.Cryptography
{
    [TestFixture]
    public class HashTest
    {
        [Test]
        public void HashTest1()
        {
            var result1 = Crypto.Hash().SignData("signdata");
            Assert.IsNotNull(result1);
            var result2 = Crypto.Hash().SignData(Encoding.UTF8.GetBytes("signdata"));
            Assert.AreEqual(result1, result2);
            using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes("signdata")))
            {
                var result3 = Crypto.Hash().SignData(ms);
                Assert.AreEqual(result1, result3);
            }
        }

        private void HashTest_2(HashFlag flag)
        {
            var result1 = Crypto.Hash(flag).SignData("signdata");
            Assert.IsNotNull(result1);
            var result2 = Crypto.Hash(flag).SignData(Encoding.UTF8.GetBytes("signdata"));
            Assert.AreEqual(result1, result2);
            using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes("signdata")))
            {
                var result3 = Crypto.Hash(flag).SignData(ms);
                Assert.AreEqual(result1, result3);
            }
        }

        [Test]
        public void HashTest2()
        {
            HashTest_2(HashFlag.MD5);
            HashTest_2(HashFlag.RIPEMD160);
            HashTest_2(HashFlag.SHA1);
            HashTest_2(HashFlag.SHA256);
            HashTest_2(HashFlag.SHA384);
            HashTest_2(HashFlag.SHA512);
        }

        private void HashTest_3(string key, HashFlag flag)
        {
            var result1 = Crypto.Hash(key, flag).SignData("signdata");
            Assert.IsNotNull(result1);
            var result2 = Crypto.Hash(key, flag).SignData(Encoding.UTF8.GetBytes("signdata"));
            Assert.AreEqual(result1, result2);
            using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes("signdata")))
            {
                var result3 = Crypto.Hash(key, flag).SignData(ms);
                Assert.AreEqual(result1, result3);
            }
        }

        [Test]
        public void HashTest3()
        {
            HashTest_3("hkey", HashFlag.MD5);
            HashTest_3("hkey", HashFlag.RIPEMD160);
            HashTest_3("hkey", HashFlag.SHA1);
            HashTest_3("hkey", HashFlag.SHA256);
            HashTest_3("hkey", HashFlag.SHA384);
            HashTest_3("hkey", HashFlag.SHA512);
        }

        [Test]
        public void HashTest4()
        {
        }
        [Test]
        public void HashTest5()
        {
        }
        [Test]
        public void HashTest6()
        {
        }
        [Test]
        public void HashTest7()
        {
        }
        [Test]
        public void HashTest8()
        {
        }
    }
}
