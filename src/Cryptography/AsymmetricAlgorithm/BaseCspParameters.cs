using System;
using System.Security.Cryptography;

namespace Sparrow.CommonLibrary.Cryptography.AsymmetricAlgorithm
{
    public class BaseCspParameters : AsymmetricCryptoBase
    {
        private readonly CspParameters _cspParameters;
        private readonly AsymmetricFlag _flag;

        public BaseCspParameters(CspParameters cspParameters)
        {
            if (cspParameters == null)
                throw new ArgumentNullException("cspParameters");
            _cspParameters = cspParameters;
            _flag = CryptographySettings.AsymmetricFlag;
        }

        public BaseCspParameters(CspParameters cspParameters, AsymmetricFlag flag)
        {
            if (cspParameters == null)
                throw new ArgumentNullException("cspParameters");
            _cspParameters = cspParameters;
            _flag = flag;
        }

        public override System.Security.Cryptography.AsymmetricAlgorithm AlgorithmByPublicKey()
        {
            if (_flag == AsymmetricFlag.RSA)
            {
                var rsa = new RSACryptoServiceProvider(_cspParameters);
                return rsa;
            }
            var dsa = new DSACryptoServiceProvider(_cspParameters);
            return dsa;
        }

        public override System.Security.Cryptography.AsymmetricAlgorithm AlgorithmByPrivateKey()
        {
            if (_flag == AsymmetricFlag.RSA)
            {
                var rsa = new RSACryptoServiceProvider(_cspParameters);
                return rsa;
            }
            var dsa = new DSACryptoServiceProvider(_cspParameters);
            return dsa;
        }
    }
}
