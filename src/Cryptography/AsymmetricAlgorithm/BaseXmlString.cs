using System;
using System.Security.Cryptography;

namespace Sparrow.CommonLibrary.Cryptography.AsymmetricAlgorithm
{
    public class BaseXmlString : AsymmetricCryptoBase
    {
        private readonly string _xmlString;
        private readonly AsymmetricFlag _flag;
        public BaseXmlString(string xmlString)
            : this(xmlString, CryptographySettings.AsymmetricFlag)
        {
        }
        public BaseXmlString(string xmlString, AsymmetricFlag flag)
        {
            if (string.IsNullOrEmpty(xmlString))
                throw new ArgumentNullException("xmlString");
            _xmlString = xmlString;
            _flag = flag;
        }

        public override System.Security.Cryptography.AsymmetricAlgorithm AlgorithmByPublicKey()
        {
            if (_flag == AsymmetricFlag.RSA)
            {
                var rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(_xmlString);
                return rsa;
            }
            var dsa = new DSACryptoServiceProvider();
            dsa.FromXmlString(_xmlString);
            return dsa;
        }

        public override System.Security.Cryptography.AsymmetricAlgorithm AlgorithmByPrivateKey()
        {
            if (_flag == AsymmetricFlag.RSA)
            {
                var rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(_xmlString);
                return rsa;
            }
            var dsa = new DSACryptoServiceProvider();
            dsa.FromXmlString(_xmlString);
            return dsa;
        }
    }
}
