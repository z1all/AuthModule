namespace AuthModule.Server.Services.Interfaces
{
    internal interface IAuthServerService
    {
        bool AuthByUserNameAndPassword(string userName, string password);
        bool TrySavePublicKey(string publicKey);
        bool CheckKnownKey(string publicKey);
        byte[] GetEncryptRandomMassage(string publicKey, out string randomMessage);
        bool EqualRandomMessage(string decryptRandomMassage, string randomMessage);
    }
}
