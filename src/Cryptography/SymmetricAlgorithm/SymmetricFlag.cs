using System;
namespace Sparrow.CommonLibrary.Cryptography.SymmetricAlgorithm
{
    public enum SymmetricFlag
    {
        /// <summary>
        /// Rijndael是AES（高级加密标准）的一个实现，速度快，安全级别高。
        /// </summary>
        /// <remarks>此算法支持 128、192 或 256 位的密钥长度。</remarks>
        Rijndael,
        /// <summary>
        /// 3DES是基于DES，对一块数据用三个不同的密钥进行三次加密，强度更高。
        /// </summary>
        /// <remarks>此算法支持从 128 位到 192 位（以 64 位递增）的密钥长度。</remarks>
        TripleDES,
        /// <summary>
        /// RC2用变长密钥对大量数据进行加密，比DES快。
        /// </summary>
        /// <remarks>此算法支持从 40 位到 1024 位（以 8 位递增）的密钥长度。</remarks>
        RC2,
        /// <summary>
        /// 数据加密标准，速度较快，安全级别低，易被破解。
        /// </summary>
        /// <remarks>此算法支持长度为 64 位的密钥。</remarks>
        [Obsolete("安全级别低，易被破解，不建议使用。")]
        DES
    }
}
