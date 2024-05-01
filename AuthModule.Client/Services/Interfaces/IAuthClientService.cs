using CryptoModule;

namespace AuthModule.Client.Services.Interfaces
{
    internal interface IAuthClientService
    {
        Keys GetNewKeys(string? passphrase);
        string GetCurrentPublicKey();
        bool TryGetCurrentPrivateKey(string? passphrase, out string privateKey);
        byte[] DecryptRandomMessage(string privateKey, byte[] encryptMassageBytes);
    }
}
