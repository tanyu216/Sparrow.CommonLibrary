using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sparrow.CommonLibrary.Cryptography
{
    /// <summary>
    /// 加密默认设置
    /// </summary>
    public static class CryptographySettings
    {
        /// <summary>
        /// 默认使用的Hash算法
        /// </summary>
        public static HashAlgorithm.HashFlag HashFlag { get { return HashAlgorithm.HashFlag.SHA256; } }

        /// <summary>
        /// 默认使用的非对称算法
        /// </summary>
        public static AsymmetricAlgorithm.AsymmetricFlag AsymmetricFlag { get { return AsymmetricAlgorithm.AsymmetricFlag.RSA; } }

        /// <summary>
        /// 默认使用的对称算法
        /// </summary>
        public static SymmetricAlgorithm.SymmetricFlag SymmetricFlag { get { return SymmetricAlgorithm.SymmetricFlag.Rijndael; } }

        /// <summary>
        /// 默认编码
        /// </summary>
        public static Encoding Encoding { get { return Encoding.UTF8; } }


    }
}
