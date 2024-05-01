using AuthModule.Client.Services.Interfaces;
using AuthModule.Client.Stores;
using CryptoModule;
using CryptoModule.Interfaces;

namespace AuthModule.Client.Services
{
    internal class AuthClientService(
        IKeysStore keysStore, 
        IAsymmetricCryptoService cryptoService,
        ISymmetricCryptoService symmetricCryptoService
    ) : IAuthClientService
    {
        private readonly IKeysStore _keysStore = keysStore;
        private readonly IAsymmetricCryptoService _cryptoService = cryptoService;
        private readonly ISymmetricCryptoService _symmetricCryptoService = symmetricCryptoService;

        public Keys GetNewKeys(string? passphrase)
        {
            Keys newKeys = _cryptoService.MakeKeysPair();

            newKeys.PrivateKey = _symmetricCryptoService.Encrypt(newKeys.PrivateKey, passphrase);

            _keysStore.SaveKeys(newKeys);

            return newKeys;
        }

        public string GetCurrentPublicKey()
        {
            return _keysStore.GetKeys().PublicKey;
        }

        public bool TryGetCurrentPrivateKey(string? passphrase, out string privateKey)
        {
            return _symmetricCryptoService.TryDecrypt(_keysStore.GetKeys().PrivateKey, passphrase, out privateKey);
        }

        public byte[] DecryptRandomMessage(string privateKey, byte[] encryptMassageBytes)
        {
            byte[] decryptMassageBytes = _cryptoService.Decrypt(privateKey, encryptMassageBytes);

            return decryptMassageBytes;
        }
    }
}
