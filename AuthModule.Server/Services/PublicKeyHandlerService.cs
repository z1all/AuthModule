using AuthModule.Server.Helpers;
using AuthModule.Server.Services.Interfaces;
using System.Net.Sockets;

namespace AuthModule.Server.Services
{
    internal class PublicKeyHandlerService : IPublicKeyHandlerService
    {
        private readonly IAuthServerService _authService;

        public PublicKeyHandlerService(IAuthServerService authService)
        {
            _authService = authService;
        }

        public void AddPublicKey(NetworkStream stream)
        {
            bool isAuthenticated = CheckAuthByUserNameAndPassword(stream);
            if (!isAuthenticated) return;

            bool existSameKey = SavePublicKey(stream);
            if (!existSameKey) return;
        }

        private bool CheckAuthByUserNameAndPassword(NetworkStream stream)
        {
            do
            {
                string userName = stream.ReadString(64);
                string password = stream.ReadString(64);

                if (_authService.AuthByUserNameAndPassword(userName, password))
                {
                    return stream.SendSuccess();
                }
                else
                {
                    stream.SendFail();
                }
            }
            while (true);
        }

        private bool SavePublicKey(NetworkStream stream)
        {
            do
            {
                string publicKey = stream.ReadString(2048);

                if (_authService.TrySavePublicKey(publicKey))
                {
                    return stream.SendSuccess();
                }
                else
                {
                    stream.SendFail();
                }
            }
            while (true);
        }
    }
}
