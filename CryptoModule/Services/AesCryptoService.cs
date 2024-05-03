using System.Security.Cryptography;
using CryptoModule.Interfaces;

namespace CryptoModule.Services
{
    public class AesCryptoService : ISymmetricCryptoService
    {
        public string Encrypt(string privateKey, string? passphrase)
        {
            if(passphrase is null)
            {
                return privateKey;
            }
            
            using (Aes aesAlg = Aes.Create())
            { 
                byte[] salt = GenerateRandomSalt();

                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(passphrase, salt, 10000, HashAlgorithmName.SHA256);
                aesAlg.Key = pdb.GetBytes(32);
                aesAlg.IV = pdb.GetBytes(16);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(salt, 0, salt.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV), CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(privateKey);
                        }
                    }
                    byte[] encrypted = msEncrypt.ToArray();

                    string encryptedPrivateKey = Convert.ToBase64String(encrypted);

                    return encryptedPrivateKey;
                }
            }
        }

        public bool TryDecrypt(string encryptedPrivateKey, string? passphrase, out string decryptedPrivateKey)
        {
            if (passphrase is null)
            {
                decryptedPrivateKey = encryptedPrivateKey;
                return true;
            }

            try
            {
                byte[] encryptedPrivateKeyBytes = Convert.FromBase64String(encryptedPrivateKey);

                using (Aes aesAlg = Aes.Create())
                {
                    byte[] salt = new byte[16];
                    Array.Copy(encryptedPrivateKeyBytes, 0, salt, 0, 16);

                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(passphrase, salt, 10000, HashAlgorithmName.SHA256);
                    aesAlg.Key = pdb.GetBytes(32);
                    aesAlg.IV = pdb.GetBytes(16);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream msDecrypt = new MemoryStream(encryptedPrivateKeyBytes, 16, encryptedPrivateKeyBytes.Length - 16))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                decryptedPrivateKey = srDecrypt.ReadToEnd();

                                return true;
                            }
                        }
                    }
                }
            }
            catch(CryptographicException)
            {
                decryptedPrivateKey = "";
                return false;
            }
        }

        private static byte[] GenerateRandomSalt()
        {
            byte[] salt = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
