using System.Text;
using System.IO;

namespace Sparrow.CommonLibrary.Cryptography
{
    /// <summary>
    /// 基于密钥的加密和解密算法
    /// </summary>
    public interface ICrypto
    {
        /// <summary>
        /// 算法名称
        /// </summary>
        string AlgorithmName { get; }
        /// <summary>
        /// 字符编码方式。
        /// </summary>
        Encoding Encoding { get; set; }
        /// <summary>
        /// 加密指定字节数组。
        /// </summary>
        /// <param name="buffer">明文输入。</param>
        /// <returns>加密后的输出。</returns>
        byte[] Encrypt(byte[] buffer);
        /// <summary>
        /// 加密指定对象<paramref name="s"/>。
        /// </summary>
        /// <param name="s">明文输入。</param>
        /// <returns>加密后的输出。</returns>
        byte[] Encrypt(string s);
        /// <summary>
        /// 加密指定对象<paramref name="inputStream"/>。
        /// </summary>
        /// <param name="inputStream">明文输入。</param>
        /// <returns>加密后的输出。</returns>
        byte[] Encrypt(Stream inputStream);
        /// <summary>
        /// 加密指定对象<paramref name="inputStream"/>。
        /// </summary>
        /// <param name="inputStream">明文输入。</param>
        /// <param name="outputStream">加密后的输出。</param>
        void Encrypt(Stream inputStream, Stream outputStream);
        /// <summary>
        /// 解密指定字节数组。
        /// </summary>
        /// <param name="buffer">密文输入。</param>
        /// <returns>解密后的输入。</returns>
        byte[] Decrypt(byte[] buffer);
        /// <summary>
        /// 解密指定对象<paramref name="inputStream"/>
        /// </summary>
        /// <param name="inputStream">密文输入。</param>
        /// <param name="outputStream">解密后的输入。</param>
        void Decrypt(Stream inputStream, Stream outputStream);
    }
}
