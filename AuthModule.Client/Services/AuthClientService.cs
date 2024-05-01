using AuthModule.Client.Services.Interfaces;
using AuthModule.Client.Stores;
using CryptoModule;

namespace AuthModule.Client.Services
{
    internal class AuthClientService(
        IKeysStore keysStore, 
        ICryptoService cryptoService
    ) : IAuthClientService
    {
        private readonly IKeysStore _keysStore = keysStore;
        private readonly ICryptoService _cryptoService = cryptoService;

        public Keys GetNewKeys()
        {
            Keys newKeys = _cryptoService.MakeKeysPair();

            _keysStore.SaveKeys(newKeys);

            return newKeys;
        }

        public string GetCurrentPublicKey()
        {
            return _keysStore.GetKeys().PublicKey;
        }

        public byte[] DecryptRandomMessage(byte[] encryptMassageBytes)
        {
            Keys keys = _keysStore.GetKeys();

            byte[] decryptMassageBytes = _cryptoService.Decrypt(keys.PrivateKey, encryptMassageBytes);

            return decryptMassageBytes;
        }
    }
}
