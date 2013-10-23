using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Sparrow.CommonLibrary.Cryptography.SymmetricAlgorithm
{
    public class SymmetricCrypto : ISymmetricCrypto, ICrypto
    {
        public SymmetricCrypto(string key)
            : this(key, GetDefaultIV(CryptographySettings.SymmetricFlag), CryptographySettings.SymmetricFlag)
        {
        }

        public SymmetricCrypto(string key, SymmetricFlag flag)
            : this(key, GetDefaultIV(flag), flag)
        {
        }

        public SymmetricCrypto(string key, byte[] iv)
            : this(key, iv, CryptographySettings.SymmetricFlag)
        {
        }

        public SymmetricCrypto(string key, byte[] iv, string algorithmName)
            : this(key, iv, algorithmName, CryptographySettings.Encoding)
        {
        }

        public SymmetricCrypto(string key, byte[] iv, string algorithmName, Encoding encoding)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (iv == null)
                throw new ArgumentNullException("iv");
            Key = key;
            IV = iv;
            AlgorithmName = algorithmName;
            Encoding = encoding;
        }

        public SymmetricCrypto(string key, byte[] iv, SymmetricFlag flag)
            : this(key, iv, flag, CryptographySettings.Encoding)
        {
        }

        public SymmetricCrypto(string key, byte[] iv, SymmetricFlag flag, Encoding encoding)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (iv == null)
                throw new ArgumentNullException("iv");
            Key = key;
            IV = iv;
            Encoding = encoding;
            switch (flag)
            {
                case SymmetricFlag.Rijndael:
                    AlgorithmName = "Rijndael";
                    break;
                case SymmetricFlag.TripleDES:
                    AlgorithmName = "TripleDES";
                    break;
                case SymmetricFlag.RC2:
                    AlgorithmName = "RC2";
                    break;
                case SymmetricFlag.DES:
                    AlgorithmName = "DES";
                    break;
            }
        }

        private static byte[] GetDefaultIV(SymmetricFlag flag)
        {
            switch (flag)
            {
                case SymmetricFlag.Rijndael:
                    return new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef, 0xef, 0xcd, 0xab, 0x90, 0x78, 0x56, 0x34, 0x12 };
                case SymmetricFlag.TripleDES:
                    return new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
                case SymmetricFlag.RC2:
                    return new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
                case SymmetricFlag.DES:
                    return new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
            }
            throw new NotSupportedException(string.Format("不支持{0}自动匹配向量。", flag));
        }

        #region ISymmetricCrypto

        public string AlgorithmName { get; set; }

        private string Key;
        private byte[] IV;

        public void SetKey(string key, byte[] iv)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (iv == null)
                throw new ArgumentNullException("iv");
            this.Key = key;
            this.IV = iv;
        }

        #endregion

        #region ICrypto

        public Encoding Encoding { get; set; }

        public byte[] Encrypt(byte[] buffer)
        {
            using (var ms = new MemoryStream(1024))
            {
                Crypt(alg => alg.CreateEncryptor(), ms, cryptoStream => cryptoStream.Write(buffer, 0, buffer.Length));
                return ms.ToArray();
            }
        }

        public byte[] Encrypt(string s)
        {
            return Encrypt(Encoding.GetBytes(s));
        }

        public byte[] Encrypt(Stream inputStream)
        {
            using (var ms = new MemoryStream(1024))
            {
                Crypt(alg => alg.CreateEncryptor(), ms, cryptoStream => StreamWriter(inputStream, cryptoStream));
                return ms.ToArray();
            }
        }

        public void Encrypt(Stream inputStream, Stream outputStream)
        {
            Crypt(alg => alg.CreateEncryptor(), outputStream, cryptoStream => StreamWriter(inputStream, cryptoStream));
        }

        public byte[] Decrypt(byte[] buffer)
        {
            using (var ms = new MemoryStream(1024))
            {
                Crypt(alg => alg.CreateDecryptor(), ms, cryptoStream => cryptoStream.Write(buffer, 0, buffer.Length));
                return ms.ToArray();
            }
        }

        public void Decrypt(Stream inputStream, Stream outputStream)
        {
            Crypt(alg => alg.CreateDecryptor(), outputStream, cryptoStream => StreamWriter(inputStream, cryptoStream));
        }

        #endregion

        protected void Crypt(Func<System.Security.Cryptography.SymmetricAlgorithm, ICryptoTransform> transform, Stream outputStream, Action<CryptoStream> cryptoStream)
        {
            using (var algorithm = Create())
            {
                using (var cs = new CryptoStream(outputStream, transform(algorithm), CryptoStreamMode.Write))
                {
                    cryptoStream(cs);
                    cs.FlushFinalBlock();
                    //
                    cs.Clear();
                    algorithm.Clear();
                }
            }
        }

        protected void StreamWriter(Stream inputStream, Stream outputStream)
        {
            const int blockSize = 1024 * 1024 * 16;//16MB
            if (inputStream.Length < blockSize)
            {
                var buffer = new byte[inputStream.Length];
                var count = inputStream.Read(buffer, 0, buffer.Length);
                if (count < 1)
                    return;
                outputStream.Write(buffer, 0, count);
            }
            else
            {
                var buffer = new byte[blockSize];
                int count;
                while (0 < (count = inputStream.Read(buffer, 0, buffer.Length)))
                {
                    outputStream.Write(buffer, 0, count);
                }
            }
        }

        protected virtual System.Security.Cryptography.SymmetricAlgorithm Create()
        {
            System.Security.Cryptography.SymmetricAlgorithm crypto;
            if (string.IsNullOrWhiteSpace(AlgorithmName))
                crypto = System.Security.Cryptography.SymmetricAlgorithm.Create();
            else
                crypto = System.Security.Cryptography.SymmetricAlgorithm.Create(AlgorithmName);
            //
            crypto.Key = Encoding.GetBytes(Key);
            crypto.IV = IV;
            return crypto;
        }
    }
}
