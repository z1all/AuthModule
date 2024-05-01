using System.Net.Sockets;
using AuthModule.Server.Helpers;
using AuthModule.Server.Services.Interfaces;

namespace AuthModule.Server.Services
{
    internal class AuthHandlerService : IAuthHandlerService
    {
        private readonly IAuthServerService _authService;

        public AuthHandlerService(IAuthServerService authService)
        {
            _authService = authService;
        }

        public bool CheckAuthByKeys(NetworkStream stream)
        {
            bool keyExist = CheckKeyInStorage(stream, out string publicKey);
            if (!keyExist) return false;

            bool userRight = CheckUserOnRandomMessage(stream, publicKey);
            if (!userRight) return false;

            return true;
        }

        private bool CheckKeyInStorage(NetworkStream stream, out string publicKey)
        {
            publicKey = stream.ReadString(2048);

            if (_authService.CheckKnownKey(publicKey))
            {
                return stream.SendSuccess();
            }
            else
            {
                return stream.SendFail();
            }
        }

        private bool CheckUserOnRandomMessage(NetworkStream stream, string publicKey)
        {
            byte[] encryptMassage = _authService.GetEncryptRandomMassage(publicKey, out string randomMessage);
            stream.WriteBytes(encryptMassage);

            string decryptRandomMessage = stream.ReadString(1024);
            if (_authService.EqualRandomMessage(decryptRandomMessage, randomMessage))
            {
                return stream.SendSuccess();
            }
            else
            {
                return stream.SendFail();
            }
        }
    }
}
