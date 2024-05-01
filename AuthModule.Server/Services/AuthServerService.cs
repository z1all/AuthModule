using AuthModule.Server.Helpers;
using AuthModule.Server.Services.Interfaces;
using AuthModule.Server.Stores;
using CryptoModule;
using System.Text;

namespace AuthModule.Server.Services
{
    internal class AuthServerService : IAuthServerService
    {
        private readonly IKeysStore _fileKeysStore;
        private readonly IProfileStore _profileStore;
        private readonly ICryptoService _cryptoService;

        public AuthServerService(IKeysStore fileKeysStore, IProfileStore profileStore, ICryptoService cryptoService)
        {
            _fileKeysStore = fileKeysStore;
            _profileStore = profileStore;
            _cryptoService = cryptoService;
        }

        public bool AuthByUserNameAndPassword(string userName, string password)
        {
            bool rightUserNameAndPassword = _profileStore.CheckUserNameAndPassword(userName, password);
            return rightUserNameAndPassword;
        }

        public bool TrySavePublicKey(string publicKey)
        {
            bool keyExist = _fileKeysStore.FindKey(publicKey);
            if (!keyExist)
            {
                _fileKeysStore.SaveKey(publicKey);
                return true;
            }
            return false;
        }

        public bool CheckKnownKey(string publicKey)
        {
            bool keyExist = _fileKeysStore.FindKey(publicKey);
            return keyExist;
        }

        public byte[] GetEncryptRandomMassage(string publicKey, out string randomMessage)
        {
            randomMessage = TextHelper.GenerateRandomString(100);
            byte[] encryptRandomMassage = _cryptoService.Encrypt(publicKey, Encoding.UTF8.GetBytes(randomMessage));
            return encryptRandomMassage;
        }

        public bool EqualRandomMessage(string decryptRandomMassage, string randomMessage)
        {
            return decryptRandomMassage == randomMessage;
        }
    }
}
