using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Sparrow.CommonLibrary.Cryptography.AsymmetricAlgorithm
{
    public abstract class AsymmetricCryptoBase : IAsymmetricCrypto, IHash
    {
        public string AlgorithmName
        {
            get { return null; }
            set { throw new InvalidOperationException("设置属性AlgorithmName无效。"); }
        }

        public Encoding Encoding { get; set; }

        protected AsymmetricCryptoBase()
        {
            Encoding = CryptographySettings.Encoding;
        }

        #region IHash

        public byte[] SignData(byte[] buffer)
        {
            using (var hash = AlgorithmByPrivateKey())
            {
                byte[] result = null;
                if (Adapter<RSACryptoServiceProvider>(hash, x => result = x.SignData(buffer, SHA256.Create())))
                {
                    hash.Clear();
                    return result;
                }
                if (Adapter<DSACryptoServiceProvider>(hash, x => result = x.SignData(buffer)))
                {
                    hash.Clear();
                    return result;
                }
                //
                throw new NotSupportedException(string.Format("{0}不支持签名算法{1}。", GetType().FullName, hash.GetType().FullName));
            }
        }

        public byte[] SignData(string s)
        {
            return SignData(Encoding.GetBytes(s));
        }

        public byte[] SignData(Stream inputStream)
        {
            return SignData(ReadStream(inputStream));
        }

        public bool VerifySign(byte[] buffer, byte[] sign)
        {
            using (var hash = AlgorithmByPublicKey())
            {
                bool result = false;
                if (Adapter<RSACryptoServiceProvider>(hash, x => result = x.VerifyData(buffer, SHA256.Create(), sign)))
                {
                    hash.Clear();
                    return result;
                }
                if (Adapter<DSACryptoServiceProvider>(hash, x => result = x.VerifyData(buffer, sign)))
                {
                    hash.Clear();
                    return result;
                }
                //
                throw new NotSupportedException(string.Format("{0}不支持签名算法{1}。", GetType().FullName, hash.GetType().FullName));
            }
        }

        public bool VerifySign(string s, byte[] sign)
        {
            return VerifySign(Encoding.GetBytes(s), sign);
        }

        public bool VerifySign(string s, string sign)
        {
            return VerifySign(Encoding.GetBytes(s), Crypto.FromHexString(sign));
        }

        public bool VerifySign(Stream inputStream, byte[] sign)
        {
            return VerifySign(ReadStream(inputStream), sign);
        }

        public bool VerifySign(System.IO.Stream inputStream, string sign)
        {
            return VerifySign(inputStream, Crypto.FromHexString(sign));
        }

        #endregion

        #region IAsymmetricCrypto

        public byte[] Encrypt(byte[] buffer)
        {
            using (var hash = AlgorithmByPublicKey())
            {
                byte[] result = null;
                if (Adapter<RSACryptoServiceProvider>(hash, x => result = x.Encrypt(buffer, false)))
                {
                    hash.Clear();
                    return result;
                }
                //
                throw new NotSupportedException(string.Format("{0}不支持加密算法{1}。", GetType().FullName, hash.GetType().FullName));
            }
        }

        public byte[] Encrypt(string s)
        {
            return Encrypt(Encoding.GetBytes(s));
        }

        public byte[] Encrypt(Stream inputStream)
        {
            return Encrypt(ReadStream(inputStream));
        }

        public void Encrypt(Stream inputStream, Stream outputStream)
        {
            var result = Encrypt(inputStream);
            StreamWriter(result, outputStream);
        }

        public byte[] Decrypt(byte[] buffer)
        {
            using (var hash = AlgorithmByPrivateKey())
            {
                byte[] result = null;
                if (Adapter<RSACryptoServiceProvider>(hash, x => result = x.Decrypt(buffer, false)))
                {
                    hash.Clear();
                    return result;
                }
                //
                throw new NotSupportedException(string.Format("{0}不支持解密算法{1}。", GetType().FullName, hash.GetType().FullName));
            }
        }

        public void Decrypt(Stream inputStream, Stream outputStream)
        {
            var result = Decrypt(ReadStream(inputStream));
            StreamWriter(result, outputStream);
        }

        #endregion

        protected virtual byte[] ReadStream(Stream sm)
        {
            const int maxMb = 1024 * 1024 * 8; //8MB
            if (sm.Length > maxMb)
                throw new OverflowException(string.Format("文件超过{0}个字节。", maxMb));
            var buffer = new byte[sm.Length];
            var count = sm.Read(buffer, 0, (int)sm.Length);
            if (buffer.Length != count)
                return buffer.Take(count).ToArray();
            return buffer;
        }

        private static void StreamWriter(byte[] data, Stream outputStream)
        {
            outputStream.Write(data, 0, data.Length);
        }

        private static bool Adapter<T>(System.Security.Cryptography.AsymmetricAlgorithm algorithm, Action<T> action) where T : System.Security.Cryptography.AsymmetricAlgorithm
        {
            if (algorithm is T)
            {
                action((T)algorithm);
                return true;
            }
            return false;
        }

        public abstract System.Security.Cryptography.AsymmetricAlgorithm AlgorithmByPublicKey();

        public abstract System.Security.Cryptography.AsymmetricAlgorithm AlgorithmByPrivateKey();

    }
}
