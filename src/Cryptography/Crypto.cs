using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Sparrow.CommonLibrary.Cryptography.AsymmetricAlgorithm;
using Sparrow.CommonLibrary.Cryptography.HashAlgorithm;
using Sparrow.CommonLibrary.Cryptography.SymmetricAlgorithm;

namespace Sparrow.CommonLibrary.Cryptography
{
    public static class Crypto
    {
        #region IHash

        /// <summary>
        /// Hash签名
        /// </summary>
        /// <returns></returns>
        /// <remarks>默认为采用SHA256算法完成哈希签名，如果想使用SHA1/MD5/SHA384/SHA512等，可采用配置方式实现</remarks>
        public static IHash Hash()
        {
            return new Hash();
        }

        /// <summary>
        /// Hash签名
        /// </summary>
        /// <param name="flag"> </param>
        /// <returns></returns>
        public static IHash Hash(HashFlag flag)
        {
            return new Hash(flag);
        }

        /// <summary>
        /// Hash签名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IHash HashFromConfig(string name)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region KeyedHash

        /// <summary>
        /// 基于Key的Hash签名
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks>默认为采用SHA256算法完成哈希签名</remarks>
        public static IHash Hash(string key)
        {
            return new HashMAC(key);
        }

        /// <summary>
        /// 基于Key的Hash签名
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flag"> </param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static IHash Hash(string key, HashFlag flag)
        {
            return new HashMAC(key, flag);
        }

        #endregion

        #region AsymmetricHash

        /// <summary>
        /// 非对称的Hash签名
        /// </summary>
        /// <param name="xmlString">xml描述的证书</param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static IHash Hash(string xmlString, AsymmetricFlag flag)
        {
            return new BaseXmlString(xmlString, flag);
        }

        /// <summary>
        /// 非对称的Hash签名
        /// </summary>
        /// <param name="fileName">完整的证书路径</param>
        /// <param name="certificate">始终为true</param>
        /// <returns></returns>
        public static IHash Hash(string fileName, bool certificate)
        {
            return new BaseCertificate(fileName);
        }

        /// <summary>
        /// 非对称的Hash签名
        /// </summary>
        /// <param name="fileName">完整的证书路径</param>
        /// <param name="password">证书密码</param>
        /// <returns></returns>
        public static IHash Hash(string fileName, string password)
        {
            return new BaseCertificate(fileName, password);
        }

        /// <summary>
        /// 非对称的Hash签名
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="location"></param>
        /// <param name="certName">CN=myCerName...</param>
        public static IHash Hash(StoreName storeName, StoreLocation location, string certName)
        {
            return new BaseCertificate(storeName, location, certName);
        }

        #endregion

        #region AsymmetricCrypto

        /// <summary>
        /// 非对称加密解密，通过容器获取密钥。
        /// </summary>
        /// <returns></returns>
        public static IAsymmetricCrypto AsymmetricCrypto(CspParameters csp)
        {
            return new BaseCspParameters(csp);
        }

        /// <summary>
        /// 非对称加密解密，通过文件获取密钥。
        /// </summary>
        /// <param name="xmlString">xml描述的证书</param>
        /// <returns></returns>
        public static IAsymmetricCrypto AsymmetricCrypto(string xmlString)
        {
            return new BaseXmlString(xmlString);
        }

        /// <summary>
        /// 非对称加密解密，通过文件获取密钥。
        /// </summary>
        /// <param name="fileName">完整的证书路径</param>
        /// <param name="certificate">始终为true</param>
        /// <returns></returns>
        public static IAsymmetricCrypto AsymmetricCrypto(string fileName, bool certificate)
        {
            return new BaseCertificate(fileName);
        }

        /// <summary>
        /// 非对称加密解密，通过文件获取密钥。
        /// </summary>
        /// <param name="fileName">完整的证书路径</param>
        /// <param name="password">证书密码</param>
        /// <returns></returns>
        public static IAsymmetricCrypto AsymmetricCrypto(string fileName, string password)
        {
            return new BaseCertificate(fileName, password);
        }

        /// <summary>
        /// 非对称加密解密，通过安装在系统中的证书获取密钥。
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="location"></param>
        /// <param name="certName">CN=myCerName...</param>
        public static IAsymmetricCrypto AsymmetricCrypto(StoreName storeName, StoreLocation location, string certName)
        {
            return new BaseCertificate(storeName, location, certName);
        }

        /// <summary>
        /// 非对称加密解密
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IAsymmetricCrypto AsymmetricCryptoFromConfig(string name)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SymmetricCrypto

        /// <summary>
        /// 对称加密
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks>默认采用Rijndael（AES）加密技术。</remarks>
        public static ISymmetricCrypto SymmetricCrypto(string key)
        {
            return new SymmetricCrypto(key);
        }

        /// <summary>
        /// 对称加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flag"> </param>
        /// <returns></returns>
        public static ISymmetricCrypto SymmetricCrypto(string key, SymmetricFlag flag)
        {
            return new SymmetricCrypto(key, flag);
        }

        /// <summary>
        /// 对称加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        /// <remarks>默认采用Rijndael（AES）加密技术。</remarks>
        public static ISymmetricCrypto SymmetricCrypto(string key, byte[] iv)
        {
            return new SymmetricCrypto(key, iv);
        }

        /// <summary>
        /// 对称加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static ISymmetricCrypto SymmetricCrypto(string key, byte[] iv, SymmetricFlag flag)
        {
            return new SymmetricCrypto(key, iv, flag);
        }

        /// <summary>
        /// 对称加密
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ISymmetricCrypto SymmetricCryptoFromConfig(string name)
        {
            return null;
        }

        #endregion
    }
}
