namespace Sparrow.CommonLibrary.Cryptography
{
    /// <summary>
    /// 算法标识
    /// </summary>
    public enum AsymmetricFlag
    {
        /// <summary>
        /// RSA算法
        /// </summary>
        RSA,
        /// <summary>
        /// DSA算法（只支持数据签名，不支持加密）
        /// </summary>
        DSA
    }
}
