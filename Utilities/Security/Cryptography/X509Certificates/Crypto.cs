using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Diagnostics;
using Utilities.Common.ExtensionMethods;

namespace Utilities.Security.Cryptography.X509Certificates
{
    public static class Crypto
    {
        /// <summary>
        /// Converts RsaParameters to byte array using ASN.1 format.
        /// </summary>
        /// <param name="rsaParameters">RSAParameters of private key.</param>
        /// <returns>Private key is encoded to a byte array.</returns>
        public static Byte[] EncodeRsaPrivateKey(RSAParameters rsaParameters)
        {
            Byte[] privateKey;

            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                // data is little endian since the proper logical seq is 0x30 0x82 advance 2 bytes
                binaryWriter.Write(new Byte[] { 0x30, 0x82 });

                //length last package body
                binaryWriter.Write(new Byte[] { 0x00, 0x00 });

                // version number 0x0102
                binaryWriter.Write(new Byte[] { 0x02, 0x01 });

                binaryWriter.Write((Byte)0x00); // 0x00

                // write module info
                binaryWriter.Write((Byte)0x02); // integer
                binaryWriter.Write((Byte)0x81); // data size is the following Byte
                binaryWriter.Write((Byte)(rsaParameters.Modulus.Length + 1)); // module length
                binaryWriter.Write((Byte)0x00); // null Byte
                binaryWriter.Write(rsaParameters.Modulus); // modules

                // write exponent info
                binaryWriter.Write((Byte)0x02); // integer
                binaryWriter.Write((Byte)rsaParameters.Exponent.Length); //exponent length
                binaryWriter.Write(rsaParameters.Exponent);

                // write D info
                binaryWriter.Write((Byte)0x02);
                binaryWriter.Write((Byte)0x81);
                binaryWriter.Write((Byte)(rsaParameters.D.Length + 1)); // D length
                binaryWriter.Write((Byte)0x00); // null Byte
                binaryWriter.Write(rsaParameters.D); // D data

                // write P info
                binaryWriter.Write((Byte)0x02); // integer
                binaryWriter.Write((Byte)(rsaParameters.P.Length + 1)); // P length
                binaryWriter.Write((Byte)0x00); // null Byte
                binaryWriter.Write(rsaParameters.P); // P data

                // write Q info
                binaryWriter.Write((Byte)0x02); // integer
                binaryWriter.Write((Byte)(rsaParameters.Q.Length + 1)); // Q length
                binaryWriter.Write((Byte)0x00); // null Byte
                binaryWriter.Write(rsaParameters.Q); // Q data

                // write DP info
                binaryWriter.Write((Byte)0x02); // integer
                binaryWriter.Write((Byte)rsaParameters.DP.Length); // DP length
                binaryWriter.Write(rsaParameters.DP); // DP data

                // write DQ info
                binaryWriter.Write((Byte)0x02); // integer
                binaryWriter.Write((Byte)(rsaParameters.DQ.Length + 1)); // DQ length
                binaryWriter.Write((Byte)0x00); // null Byte
                binaryWriter.Write(rsaParameters.DQ); // DQ data

                // write InverseQ info
                binaryWriter.Write((Byte)0x02); // integer
                binaryWriter.Write((Byte)(rsaParameters.InverseQ.Length + 1)); // InverseQ length
                binaryWriter.Write((Byte)0x00); // null Byte
                binaryWriter.Write(rsaParameters.InverseQ); // InverseQ data             

                privateKey = memoryStream.ToArray();
            }

            Int32 offset = 4;
            privateKey[2] = (Byte)((privateKey.Length - offset) / 256);
            privateKey[3] = (Byte)((privateKey.Length - offset) % 256);
            return privateKey;
        }

        /// <summary>
        /// Converts RsaParameters to byte array using ASN.1 format.
        /// </summary>
        /// <param name="rsaParameters">RSAParameters of public key.</param>
        /// <returns>Public key is encoded to a byte array.</returns>
        public static Byte[] EncodeRsaPublicKey(RSAParameters rsaParameters)
        {
            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(new Byte[] { 0x30, 0x82 });
                binaryWriter.Write(new Byte[] { 0x00, 0x00 });
                binaryWriter.Write(new Byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 });
                binaryWriter.Write(new Byte[] { 0x03, 0x82 });
                binaryWriter.Write(new Byte[] { 0x00, 0x00 });
                binaryWriter.Write((Byte)0x00);
                binaryWriter.Write(new Byte[] { 0x30, 0x82 });
                binaryWriter.Write(new Byte[] { 0x00, 0x00 });

                // write module info
                binaryWriter.Write((Byte)0x02); // integer
                binaryWriter.Write((Byte)0x81); // data size is the following Byte
                binaryWriter.Write((Byte)(rsaParameters.Modulus.Length + 1)); // module length
                binaryWriter.Write((Byte)0x00); // null Byte
                binaryWriter.Write(rsaParameters.Modulus); // modules

                // write exponent info
                binaryWriter.Write((Byte)0x02); // integer
                binaryWriter.Write((Byte)rsaParameters.Exponent.Length); //exponent length
                binaryWriter.Write(rsaParameters.Exponent);

                var publicKey = memoryStream.ToArray();
                return publicKey;
            }
        }

        /// <summary>
        /// Converts RsaParameters of private key to byte array using ASN.1 format and encodes the array to PEM string.
        /// </summary>
        /// <param name="rsaParameters">RSAParameters of private key.</param>
        /// <returns>Private key is encoded to PEM string.</returns>
        public static String EncodeRsaPrivateKeyToPemString(RSAParameters rsaParameters)
        {
            var privateKeyBytes = EncodeRsaPrivateKey(rsaParameters);
            var privateKeyString = GetPemStringFromBytes(privateKeyBytes, PemStringType.RsaPrivateKey);
            return privateKeyString;
        }

        /// <summary>
        /// Converts RsaParameters of public key to byte array using ASN.1 format and encodes the array to PEM string.
        /// </summary>
        /// <param name="rsaParameters">RSAParameters of public key.</param>
        /// <returns>Public key is encoded to PEM string.</returns>
        public static String EncodeRsaPublicKeyToPemString(RSAParameters rsaParameters)
        {
            var publicKeyBytes = EncodeRsaPublicKey(rsaParameters);
            var publicKeyString = GetPemStringFromBytes(publicKeyBytes, PemStringType.PublicKey);
            return publicKeyString;
        }

        /// <summary>
        /// Parses an RSA private key using the ASN.1 format.
        /// </summary>
        /// <param name="privateKey">Byte array containing PEM string of private key.</param>
        /// <returns>An instance of <see cref="RSACryptoServiceProvider"/> representing the requested private key.</returns>
        public static RSACryptoServiceProvider DecodeRsaPrivateKey(Byte[] privateKey)
        {
            using (var memoryStream = new MemoryStream(privateKey))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                UInt16 shortValue = binaryReader.ReadUInt16();
                switch (shortValue)
                {
                    // If true, data is little endian since the proper logical seq is 0x30 0x81
                    case 0x8130: binaryReader.ReadByte(); break;
                    case 0x8230: binaryReader.ReadInt16(); break;
                    default: throw new CryptographicException("Improper ASN.1 format");
                }

                // (version number)
                shortValue = binaryReader.ReadUInt16();
                if (shortValue != 0x0102) throw new CryptographicException("Improper ASN.1 format, unexpected version number");

                Byte byteValue = binaryReader.ReadByte();
                if (byteValue != 0x00) throw new CryptographicException("Improper ASN.1 format");

                // The data following the version will be the ASN.1 data itself, which in our case
                // are a sequence of integers.
                // In order to solve a problem with instancing RSACryptoServiceProvider
                // via default constructor on .net 4.0 this is a hack
                var @params = new CspParameters();
                @params.Flags = CspProviderFlags.NoFlags;
                @params.KeyContainerName = Guid.NewGuid().ToString().ToUpperInvariant();
                @params.ProviderType = ((Environment.OSVersion.Version.Major > 5) || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1))) ? 0x18 : 1;

                var rsaProvider = new RSACryptoServiceProvider(@params);
                var rsaParams = new RSAParameters();
                rsaParams.Modulus = binaryReader.ReadBytes(DecodeIntegerSize(binaryReader));

                // Argh, this is a pain.  From emperical testing it appears to be that RSAParameters doesn't like Byte buffers that
                // have their leading zeros removed.  The RFC doesn't address this area that I can see, so it's hard to say that this
                // is a bug, but it sure would be helpful if it allowed that. So, there's some extra code here that knows what the
                // sizes of the various components are supposed to be.  Using these sizes we can ensure the buffer sizes are exactly
                // what the RSAParameters expect.  Thanks, Microsoft.
                var traits = new RSAParameterTraits(rsaParams.Modulus.Length * 8);
                rsaParams.Modulus = rsaParams.Modulus.PadLeft(traits.SizeMod, 0x00);
                rsaParams.Exponent = binaryReader.ReadBytes(DecodeIntegerSize(binaryReader)).PadLeft(traits.SizeExp, 0x00);
                rsaParams.D = binaryReader.ReadBytes(DecodeIntegerSize(binaryReader)).PadLeft(traits.SizeD, 0x00);
                rsaParams.P = binaryReader.ReadBytes(DecodeIntegerSize(binaryReader)).PadLeft(traits.SizeP, 0x00);
                rsaParams.Q = binaryReader.ReadBytes(DecodeIntegerSize(binaryReader)).PadLeft(traits.SizeQ, 0x00);
                rsaParams.DP = binaryReader.ReadBytes(DecodeIntegerSize(binaryReader)).PadLeft( traits.SizeDP, 0x00);
                rsaParams.DQ = binaryReader.ReadBytes(DecodeIntegerSize(binaryReader)).PadLeft(traits.SizeDQ, 0x00);
                rsaParams.InverseQ = binaryReader.ReadBytes(DecodeIntegerSize(binaryReader)).PadLeft(traits.SizeInvQ, 0x00);

                rsaProvider.ImportParameters(rsaParams);
                return rsaProvider;
            }
        }

        /// <summary>
        /// Parses an RSA public key from a byte array.
        /// </summary>
        /// <param name="privateKeyBytes">Byte array containing PEM string of public key.</param>
        /// <returns>An instance of <see cref="RSACryptoServiceProvider"/> representing the requested public key.</returns>
        public static RSACryptoServiceProvider DecodeRsaPublicKey(Byte[] publicKey)
        {
            using (var memoryStream = new MemoryStream(publicKey))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                var exceptionMessage = "Error of parsing public key.";
                var byteValue = (Byte)0;
                var shortValue = (UInt16)0;

                shortValue = binaryReader.ReadUInt16();
                switch (shortValue)
                {
                    case 0x8130: binaryReader.ReadByte(); break;
                    case 0x8230: binaryReader.ReadInt16(); break;
                    default: throw new CryptographicException(exceptionMessage);
                }

                Byte[] sequence = binaryReader.ReadBytes(15);
                if (!sequence.IsEqual(new Byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 }))
                {
                    throw new CryptographicException("Sequence for OID is not correct.");
                }

                shortValue = binaryReader.ReadUInt16();
                switch (shortValue)
                {
                    case 0x8103: binaryReader.ReadByte(); break;
                    case 0x8203: binaryReader.ReadInt16(); break;
                    default: throw new CryptographicException(exceptionMessage);
                }

                byteValue = binaryReader.ReadByte();
                if (byteValue != 0x00)
                {
                    throw new CryptographicException(exceptionMessage);
                }

                shortValue = binaryReader.ReadUInt16();
                switch (shortValue)
                {
                    case 0x8130: binaryReader.ReadByte(); break;
                    case 0x8230: binaryReader.ReadInt16(); break;
                    default: throw new CryptographicException(exceptionMessage);
                }

                Byte lowByte = 0x00;
                Byte highByte = 0x00;
                shortValue = binaryReader.ReadUInt16();
                switch (shortValue)
                {
                    case 0x8102:
                        lowByte = binaryReader.ReadByte();
                        break;
                    case 0x8202:
                        highByte = binaryReader.ReadByte();
                        lowByte = binaryReader.ReadByte();
                        break;
                    default: throw new CryptographicException(exceptionMessage);
                }

                Byte[] modInt = { lowByte, highByte, 0x00, 0x00 };
                Int32 modSize = BitConverter.ToInt32(modInt, 0);
                Byte firstByte = binaryReader.ReadByte();
                binaryReader.BaseStream.Seek(-1, SeekOrigin.Current);

                if (firstByte == 0x00)
                {
                    binaryReader.ReadByte();
                    modSize -= 1;
                }

                Byte[] modulus = binaryReader.ReadBytes(modSize);
                if (binaryReader.ReadByte() != 0x02)
                {
                    throw new CryptographicException("Expect an Integer for the exponent data.");
                }

                var expBytes = (Int32)binaryReader.ReadByte();
                var exponent = binaryReader.ReadBytes(expBytes);
                var rsaProvider = new RSACryptoServiceProvider();
                var rsaParams = new RSAParameters();
                rsaParams.Modulus = modulus;
                rsaParams.Exponent = exponent;
                rsaProvider.ImportParameters(rsaParams);
                return rsaProvider;
            }
        }

        /// <summary>
        /// Parses an RSA private key from a PEM string.
        /// </summary>
        /// <param name="privateKeyString">PEM string of private key.</param>
        /// <returns>An instance of <see cref="RSACryptoServiceProvider"/> representing the requested private key.</returns>
        public static RSACryptoServiceProvider DecodeRsaPrivateKey(String privateKeyString)
        {
            var data = GetBytesFromPemString(privateKeyString, PemStringType.RsaPrivateKey);
            return DecodeRsaPrivateKey(data);
        }

        /// <summary>
        /// Parses an RSA public key from a PEM string.
        /// </summary>
        /// <param name="publicKeyString">PEM string of public key.</param>
        /// <returns>An instance of <see cref="RSACryptoServiceProvider"/> representing the requested public key.</returns>
        public static RSACryptoServiceProvider DecodeRsaPublicKey(String publicKeyString)
        {
            var data = GetBytesFromPemString(publicKeyString, PemStringType.PublicKey);
            return DecodeRsaPublicKey(data);
        }

        /// <summary>
        /// Parses an RSA private key from a file using the ASN.1 format
        /// </summary>
        /// <param name="privateKeyFile">File containing PEM string of private key.</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        /// <returns>An instance of <see cref="RSACryptoServiceProvider"/> representing the requested private key.</returns>
        public static RSACryptoServiceProvider DecodeRsaPrivateKey(String privateKeyFile, Encoding encoding)
        {
            var privateKey = File.ReadAllText(privateKeyFile, encoding);
            return DecodeRsaPrivateKey(privateKey);
        }

        /// <summary>
        /// Parses an RSA public key from a file.
        /// </summary>
        /// <param name="publicKeyFile">File containing PEM string of public key.</param>
        /// <param name="encoding">The encoding applied to the contents of the file.</param>
        /// <returns>An instance of <see cref="RSACryptoServiceProvider"/> representing the requested public key.</returns>
        public static RSACryptoServiceProvider DecodeRsaPublicKey(String publicKeyFile, Encoding encoding)
        {
            var publicKey = File.ReadAllText(publicKeyFile, encoding);
            return DecodeRsaPublicKey(publicKey);
        }

        /// <summary>
        /// Creates X509 certificate
        /// </summary>
        /// <param name="publicCertificate">PEM string of public certificate.</param>
        /// <param name="privateKey">PEM string of private key.</param>
        /// <param name="password">Password for certificate.</param>
        /// <returns>An instance of <see cref="X509Certificate2"/> rapresenting the X509 certificate.</returns>
        public static X509Certificate2 GetCertificateFromPEMstring(String publicCertificate, String privateKey, String password)
        {
            Byte[] certBuffer = GetBytesFromPemString(publicCertificate, PemStringType.Certificate);
            Byte[] keyBuffer = GetBytesFromPemString(privateKey, PemStringType.RsaPrivateKey);
            X509Certificate2 certificate = new X509Certificate2(certBuffer, password);
            RSACryptoServiceProvider prov = Crypto.DecodeRsaPrivateKey(keyBuffer);
            certificate.PrivateKey = prov;
            return certificate;
        }

        private static Int32 DecodeIntegerSize(BinaryReader rd)
        {
            Int32 count;

            Byte byteValue = rd.ReadByte();
            if (byteValue != 0x02) return 0;

            byteValue = rd.ReadByte();
            switch (byteValue)
            {
                case 0x81:
                    count = rd.ReadByte();
                    break;
                case 0x82:
                    Byte highByte = rd.ReadByte();
                    Byte lowByte = rd.ReadByte();
                    count = BitConverter.ToUInt16(new[] { lowByte, highByte }, 0);
                    break;
                default:
                    count = byteValue;
                    break;
            }

            while (rd.ReadByte() == 0x00)
            {
                count -= 1;
            }
            rd.BaseStream.Seek(-1, SeekOrigin.Current);

            return count;
        }

        private static Byte[] GetBytesFromPemString(String pemString, PemStringType type)
        {
            String header, footer;

            switch (type)
            {
                case PemStringType.Certificate:
                    header = "-----BEGIN CERTIFICATE-----";
                    footer = "-----END CERTIFICATE-----";
                    break;
                case PemStringType.RsaPrivateKey:
                    header = "-----BEGIN RSA PRIVATE KEY-----";
                    footer = "-----END RSA PRIVATE KEY-----";
                    break;
                case PemStringType.PublicKey:
                    header = "-----BEGIN PUBLIC KEY-----";
                    footer = "-----END PUBLIC KEY-----";
                    break;
                default:
                    return null;
            }

            Int32 start = pemString.IndexOf(header) + header.Length;
            Int32 end = pemString.IndexOf(footer, start) - start;
            return Convert.FromBase64String(pemString.Substring(start, end));
        }

        private static String GetPemStringFromBytes(Byte[] key, PemStringType type)
        {
            String header, footer;

            switch (type)
            {
                case PemStringType.Certificate:
                    header = "-----BEGIN CERTIFICATE-----";
                    footer = "-----END CERTIFICATE-----";
                    break;
                case PemStringType.RsaPrivateKey:
                    header = "-----BEGIN RSA PRIVATE KEY-----";
                    footer = "-----END RSA PRIVATE KEY-----";
                    break;
                case PemStringType.PublicKey:
                    header = "-----BEGIN PUBLIC KEY-----";
                    footer = "-----END PUBLIC KEY-----";
                    break;
                default: return null;
            }

            var textBase64 = Convert.ToBase64String(key);
            var text = new StringBuilder(textBase64.Length);
            for (Int32 i = 0; i < textBase64.Length; i++)
            {
                text.Append(textBase64[i]);
                if (i != 0 && (i + 1) % 64 == 0)
                {
                    text.Append(Environment.NewLine);
                }
            }
            var result = String.Format("{0}{1}{2}", header, text, footer);
            return result;
        }

        private enum PemStringType
        {
            Certificate,
            RsaPrivateKey,
            PublicKey
        }
    }
}