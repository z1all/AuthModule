namespace CryptoModule.Interfaces
{
    public interface ISymmetricCryptoService
    {
        string Encrypt(string privateKey, string? passphrase);
        bool TryDecrypt(string encryptedPrivateKey, string? passphrase, out string decryptedPrivateKey);
    }
}
