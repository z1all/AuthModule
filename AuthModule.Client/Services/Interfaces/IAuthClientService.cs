using CryptoModule;

namespace AuthModule.Client.Services.Interfaces
{
    internal interface IAuthClientService
    {
        Keys GetNewKeys();
        string GetCurrentPublicKey();
        byte[] DecryptRandomMessage(byte[] encryptMassageBytes);
    }
}
