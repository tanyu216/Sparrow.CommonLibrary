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
            return Compare(SignData(buffer), sign) == 0;
        }

        public bool VerifySign(string s, byte[] sign)
        {
            return Compare(SignData(s), sign) == 0;
        }

        public bool VerifySign(System.IO.Stream inputStream, byte[] sign)
        {
            return Compare(SignData(inputStream), sign) == 0;
        }

        protected virtual System.Security.Cryptography.HashAlgorithm Create()
        {
            if (string.IsNullOrWhiteSpace(AlgorithmName))
                return System.Security.Cryptography.HashAlgorithm.Create();
            return System.Security.Cryptography.HashAlgorithm.Create(AlgorithmName);
        }

        public string ToHexString(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            var sb = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
                sb.Append(data[i].ToString("x2"));
            return sb.ToString();
        }

        public byte[] FromHexString(string s)
        {
            if (s == null) throw new ArgumentNullException("s");
            if (s.Length % 2 != 0) throw new ArgumentException("数据不合格。");
            var bt = new byte[s.Length / 2];
            for (var i = 0; i < s.Length; i += 2)
                bt[i / 2] = Convert.ToByte("0x" + s.Substring(i, 2));
            return bt;
        }

        [DllImport("msvcrt.dll")]
        private static extern IntPtr memcmp(byte[] b1, byte[] b2, IntPtr count);

        /// <summary>
        /// byte数据比较
        /// </summary>
        /// <param name="b1">字节数组1</param>
        /// <param name="b2">字节数组2</param>
        /// <returns>如果两个数组相同，返回0；如果数组1小于数组2，返回小于0的值；如果数组1大于数组2，返回大于0的值。</returns>
        protected int Compare(byte[] b1, byte[] b2)
        {
            return memcmp(b1, b2, new IntPtr(b1.Length)).ToInt32();
        }
    }
}
