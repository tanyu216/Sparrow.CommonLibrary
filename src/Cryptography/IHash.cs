using System.IO;
using System.Text;

namespace Sparrow.CommonLibrary.Cryptography
{
    /// <summary>
    /// 加密哈希算法
    /// </summary>
    public interface IHash
    {
        /// <summary>
        /// 算法名称
        /// </summary>
        string AlgorithmName { get; set; }
        /// <summary>
        /// 字符编码方式。
        /// </summary>
        Encoding Encoding { get; set; }
        /// <summary>
        /// 计算指定字节数组的哈希签名。
        /// </summary>
        /// <param name="buffer">要计算其哈希代码的输入。</param>
        /// <returns>计算所得的哈希代码。</returns>
        byte[] SignData(byte[] buffer);
        /// <summary>
        /// 计算指定对象<paramref name="s"/>的哈希签名。
        /// </summary>
        /// <param name="s">要计算其哈希代码的输入。</param>
        /// <returns>计算所得的哈希代码。</returns>
        byte[] SignData(string s);
        /// <summary>
        /// 计算指定对象<paramref name="inputStream"/>的哈希签名。
        /// </summary>
        /// <param name="inputStream">要计算其哈希代码的输入。</param>
        /// <returns>计算所得的哈希代码。</returns>
        byte[] SignData(Stream inputStream);
        /// <summary>
        /// 通过将指定的签名数据与为指定数据计算的签名进行比较来验证指定的签名数据。
        /// </summary>
        /// <param name="buffer">要计算签名的数据输入。</param>
        /// <param name="sign">要验证的数据签名。</param>
        /// <returns>签名验证结果。</returns>
        bool VerifySign(byte[] buffer, byte[] sign);
        /// <summary>
        /// 通过将指定的签名数据与为指定数据计算的签名进行比较来验证指定的签名数据
        /// </summary>
        /// <param name="s">要计算签名的数据输入。</param>
        /// <param name="sign">要验证的数据签名。</param>
        /// <returns>签名验证结果。</returns>
        bool VerifySign(string s, byte[] sign);
        /// <summary>
        /// 通过将指定的签名数据与为指定数据计算的签名进行比较来验证指定的签名数据
        /// </summary>
        /// <param name="s">要计算签名的数据输入。</param>
        /// <param name="sign">要验证的数据签名。</param>
        /// <returns>签名验证结果。</returns>
        bool VerifySign(string s, string sign);
        /// <summary>
        /// 通过将指定的签名数据与为指定数据计算的签名进行比较来验证指定的签名数据
        /// </summary>
        /// <param name="inputStream">要计算签名的数据输入。</param>
        /// <param name="sign">要验证的数据签名。</param>
        /// <returns>签名验证结果。</returns>
        bool VerifySign(Stream inputStream, byte[] sign);
        /// <summary>
        /// 通过将指定的签名数据与为指定数据计算的签名进行比较来验证指定的签名数据
        /// </summary>
        /// <param name="inputStream">要计算签名的数据输入。</param>
        /// <param name="sign">要验证的数据签名。</param>
        /// <returns>签名验证结果。</returns>
        bool VerifySign(Stream inputStream, string sign);

    }
}
