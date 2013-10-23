using System;
using System.Security.Cryptography;

namespace Sparrow.CommonLibrary.Cryptography.HashAlgorithm
{
    public class HashMAC : Hash, IKeyedHash
    {
        public HashMAC(string key)
            : this(key, HMACName(CryptographySettings.HashFlag))
        {
        }

        public HashMAC(string key, HashFlag flag)
            : this(key, HMACName(flag))
        {
        }

        public HashMAC(string key, string algorithmName)
            : base(algorithmName)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            Key = key;
        }

        #region IKeyedHash

        private string Key;
        public void SetKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            this.Key = key;
        }

        #endregion

        private static string HMACName(HashFlag flag)
        {
            switch (flag)
            {
                case HashFlag.MD5:
                    return "HMACMD5";
                case HashFlag.RIPEMD160:
                    return "HMACRIPEMD160";
                case HashFlag.SHA1:
                    return "HMACSHA1";
                case HashFlag.SHA256:
                    return "HMACSHA256";
                case HashFlag.SHA384:
                    return "HMACSHA384";
                case HashFlag.SHA512:
                    return "HMACSHA512";
            }
            throw new NotSupportedException(string.Format("不支持的签名算法{0}", flag));
        }

        protected override System.Security.Cryptography.HashAlgorithm Create()
        {
            HMAC mac;
            if (string.IsNullOrWhiteSpace(AlgorithmName))
                mac = HMAC.Create();
            else
                mac = HMAC.Create(AlgorithmName);
            mac.Key = Encoding.GetBytes(Key);
            return mac;
        }

    }
}
