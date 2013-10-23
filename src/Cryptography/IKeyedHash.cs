namespace Sparrow.CommonLibrary.Cryptography
{
    public interface IKeyedHash : IHash
    {
        /// <summary>
        /// 签名依据的Key
        /// </summary>
        void SetKey(string key);
    }
}
