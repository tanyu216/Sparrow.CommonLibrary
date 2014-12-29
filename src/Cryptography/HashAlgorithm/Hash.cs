using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sparrow.CommonLibrary.Cryptography.HashAlgorithm
{
    public class Hash : IHash
    {

        public string AlgorithmName { get; set; }

        public Encoding Encoding { get; set; }

        public Hash()
            : this(CryptographySettings.HashFlag, CryptographySettings.Encoding)
        {
        }

        public Hash(string algorithmName)
            : this(algorithmName, CryptographySettings.Encoding)
        {
        }

        public Hash(string algorithmName, Encoding encoding)
        {
            Encoding = encoding;
            AlgorithmName = algorithmName;
        }

        public Hash(HashFlag flag)
            : this(flag, CryptographySettings.Encoding)
        {
        }

        public Hash(HashFlag flag, Encoding encoding)
        {
            Encoding = encoding;
            switch (flag)
            {
                case HashFlag.MD5:
                    AlgorithmName = "MD5";
                    break;
                case HashFlag.SHA1:
                    AlgorithmName = "SHA1";
                    break;
                case HashFlag.SHA256:
                    AlgorithmName = "SHA256";
                    break;
                case HashFlag.SHA384:
                    AlgorithmName = "SHA384";
                    break;
                case HashFlag.SHA512:
                    AlgorithmName = "SHA512";
                    break;
                case HashFlag.RIPEMD160:
                    AlgorithmName = "RIPEMD160";
                    break;
            }
        }

        public byte[] SignData(byte[] buffer)
        {
            using (var hash = Create())
            {
                var result = hash.ComputeHash(buffer);
                hash.Clear();
                return result;
            }
        }

        public byte[] SignData(string s)
        {
            using (var hash = Create())
            {
                var result = hash.ComputeHash(Encoding.GetBytes(s));
                hash.Clear();
                return result;
            }
        }

        public byte[] SignData(System.IO.Stream inputStream)
        {
            using (var hash = Create())
            {
                var result = hash.ComputeHash(inputStream);
                hash.Clear();
                return result;
            }
        }

        public bool VerifySign(byte[] buffer, byte[] sign)
        {
            return EqualsCompare(SignData(buffer), sign);
        }

        public bool VerifySign(string s, byte[] sign)
        {
            return EqualsCompare(SignData(s), sign);
        }

        public bool VerifySign(string s, string sign)
        {
            return EqualsCompare(SignData(s), Crypto.FromHexString(sign));
        }

        public bool VerifySign(System.IO.Stream inputStream, byte[] sign)
        {
            return EqualsCompare(SignData(inputStream), sign);
        }

        public bool VerifySign(System.IO.Stream inputStream, string sign)
        {
            return EqualsCompare(SignData(inputStream), Crypto.FromHexString(sign));
        }

        protected virtual System.Security.Cryptography.HashAlgorithm Create()
        {
            if (string.IsNullOrWhiteSpace(AlgorithmName))
                return System.Security.Cryptography.HashAlgorithm.Create();
            return System.Security.Cryptography.HashAlgorithm.Create(AlgorithmName);
        }

        /// <summary>
        /// byte数据比较
        /// </summary>
        /// <param name="b1">字节数组1</param>
        /// <param name="b2">字节数组2</param>
        /// <returns></returns>
        protected bool EqualsCompare(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
                return false;
            int i = 0;
            do
            {
                if (b1[i] != b2[i])
                    return false;
            } while (++i < b1.Length);
            return true;
        }

    }
}
