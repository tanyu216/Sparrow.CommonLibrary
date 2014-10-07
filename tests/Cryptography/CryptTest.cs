using NUnit.Framework;
using Sparrow.CommonLibrary.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Test.Cryptography
{
    [TestFixture]
    class CryptTest
    {
        [Test]
        public void FromHexStringTest1()
        {
            var result = Crypto.FromHexString("0e5ef9ca0f30376f04b45eecc471d3c3");
        }
        [Test]
        public void ToHexStringTest1()
        {
            var data = "0e5ef9ca0f30376f04b45eecc471d3c3";
            var bt = Crypto.FromHexString(data);
            var result = Crypto.ToHexString(bt);
            Assert.AreEqual(data, result);
        }
        [Test]
        public void Test1()
        {

            var cryptKey = "!@#$%^&*!@#$%^&*";
            var symmetricCrypto = Crypto.SymmetricCrypto(cryptKey,SymmetricFlag.Rijndael);
            var ctyptValue = symmetricCrypto.Encrypt("我的密文");


        }
    }
}
