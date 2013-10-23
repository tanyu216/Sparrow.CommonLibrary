namespace Sparrow.CommonLibrary.Cryptography
{
    /// <summary>
    /// 对称加密和解密算法
    /// </summary>
    public interface ISymmetricCrypto : ICrypto
    {
        void SetKey(string key, byte[] iv);
    }
}
