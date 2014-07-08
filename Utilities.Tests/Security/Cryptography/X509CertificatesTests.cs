using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Common.ExtensionMethods;
using Utilities.Security.Cryptography.X509Certificates;

namespace Utilities.Tests.Security.Cryptography
{
    [TestClass]
    public class X509CertificatesTests
    {
        private readonly Byte[] _clearData = new Byte[] {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 
            0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 
            0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 
            0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 
            0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
            0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 
            0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 
            0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
        };

        private String GetFileContent(String fileName)
        {
            var path = String.Format("Utilities.Tests.Security.Cryptography.{0}", fileName);
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            using (var streamReader = new StreamReader(stream))
            {
                var fileContent = streamReader.ReadToEnd();
                return fileContent;
            }
        }

        [TestMethod]
        public void SignDataAndVerifyData()
        {
            var privateKeyString = GetFileContent("PrivateKey.pem");
            var publicKeyString = GetFileContent("PublicKey.pem");
            var rsaPrivateKey = Crypto.DecodeRsaPrivateKey(privateKeyString);
            var rsaPublicKey = Crypto.DecodeRsaPublicKey(publicKeyString);
            var signedData = rsaPrivateKey.SignData(_clearData, new SHA1CryptoServiceProvider());
            var isVerified = rsaPublicKey.VerifyData(_clearData, "SHA1", signedData);
            
            Assert.IsTrue(isVerified);
        }

        [TestMethod]
        public void EncodeDecodePrivateKeyAndSignDataAndVerifyData()
        {
            var privateKeyString = GetFileContent("PrivateKey.pem");
            var rsaPrivateKey = Crypto.DecodeRsaPrivateKey(privateKeyString);
            privateKeyString = Crypto.EncodeRsaPrivateKeyToPemString(rsaPrivateKey.ExportParameters(true));
            rsaPrivateKey = Crypto.DecodeRsaPrivateKey(privateKeyString);

            var publicKeyString = GetFileContent("PublicKey.pem");
            var rsaPublicKey = Crypto.DecodeRsaPublicKey(publicKeyString);
            var signedData = rsaPrivateKey.SignData(_clearData, new SHA1CryptoServiceProvider());
            var isVerified = rsaPublicKey.VerifyData(_clearData, "SHA1", signedData);

            Assert.IsTrue(isVerified);
        }

        [TestMethod]
        public void EncodeDecodePublicKeyAndSignDataAndVerifyData()
        {
            var publicKeyString = GetFileContent("PublicKey.pem");
            var rsaPublicKey = Crypto.DecodeRsaPublicKey(publicKeyString);
            publicKeyString = Crypto.EncodeRsaPublicKeyToPemString(rsaPublicKey.ExportParameters(false));
            rsaPublicKey = Crypto.DecodeRsaPublicKey(publicKeyString);

            var privateKeyString = GetFileContent("PrivateKey.pem");
            var rsaPrivateKey = Crypto.DecodeRsaPrivateKey(privateKeyString);
            var signedData = rsaPrivateKey.SignData(_clearData, new SHA1CryptoServiceProvider());
            var isVerified = rsaPublicKey.VerifyData(_clearData, "SHA1", signedData);

            Assert.IsTrue(isVerified);
        }
    }
}