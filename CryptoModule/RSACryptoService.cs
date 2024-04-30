using System.Security.Cryptography;

namespace CryptoModule
{
    public class RSACryptoService : ICryptoService
    {
        public byte[] Decrypt(string privateKey, byte[] data)
        {
            using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            
            rsa.ImportFromPem(privateKey);

            return rsa.Decrypt(data, false);
        }

        public byte[] Encrypt(string publicKey, byte[] data)
        {
            var keySize = 768;

            using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
           
            rsa.ImportFromPem(publicKey);
            return rsa.Encrypt(data, false);
        }

        public Keys MakeKeysPair()
        {
            using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            RSAParameters publicKey = rsa.ExportParameters(false);
            RSAParameters privateKey = rsa.ExportParameters(true);

            string privateKeyXml = rsa.ExportRSAPrivateKeyPem();
            string publicKeyXml = rsa.ExportRSAPublicKeyPem();

            return new()
            {
                PrivateKey = privateKeyXml,
                PublicKey = publicKeyXml
            };
        }
    }
}
