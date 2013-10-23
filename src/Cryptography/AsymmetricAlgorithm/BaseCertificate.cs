using System;
using System.Security.Cryptography.X509Certificates;

namespace Sparrow.CommonLibrary.Cryptography.AsymmetricAlgorithm
{
    public class BaseCertificate : AsymmetricCryptoBase
    {
        private readonly bool _fromFile;

        // fromfile=true
        private readonly string _fileName;
        private readonly string _password;
        // fromfile=false
        private readonly StoreName _storeName;
        private readonly StoreLocation _location;
        private readonly string _certName;

        public BaseCertificate(string fileName)
            : this(fileName, null)
        {
        }

        public BaseCertificate(string fileName, string password)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            if (System.IO.File.Exists(fileName) == false)
                throw new System.IO.FileNotFoundException("文件未找到。", fileName);
            _fileName = fileName;
            _password = password;
            _fromFile = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="location"></param>
        /// <param name="certName">CN=myCerName...</param>
        public BaseCertificate(StoreName storeName, StoreLocation location, string certName)
        {
            _storeName = storeName;
            _location = location;
            _certName = certName;
        }

        public override System.Security.Cryptography.AsymmetricAlgorithm AlgorithmByPublicKey()
        {
            X509Certificate2 x509Cer;
            if (_fromFile)
                x509Cer = string.IsNullOrEmpty(_password) ? new X509Certificate2(_fileName) : new X509Certificate2(_fileName, _password);
            else
                x509Cer = GetCertificate();
            return x509Cer.PublicKey.Key;
        }

        public override System.Security.Cryptography.AsymmetricAlgorithm AlgorithmByPrivateKey()
        {
            X509Certificate2 x509Cer;
            if (_fromFile)
                x509Cer = string.IsNullOrEmpty(_password) ? new X509Certificate2(_fileName) : new X509Certificate2(_fileName, _password);
            else
                x509Cer = GetCertificate();
            return x509Cer.PrivateKey;
        }

        protected X509Certificate2 GetCertificate()
        {
            var store = new X509Store(_storeName, _location);
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 cert in store.Certificates)
            {
                if (cert.SubjectName.Name == _certName)
                {
                    store.Close();
                    return cert;
                }
            }
            store.Close();
            return null;
        }
    }
}
