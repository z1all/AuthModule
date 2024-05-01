using System.Net.Sockets;
using System.Net.Sockets.Extension;
using AuthModule.Server.Services.Interfaces;

namespace AuthModule.Server.Services
{
    internal class HandlerServerService(
        IPublicKeyHandlerService publicKeyHandlerService,
        IAuthHandlerService authHandlerService
    )
    {
        private readonly IPublicKeyHandlerService _publicKeyHandlerService = publicKeyHandlerService;
        private readonly IAuthHandlerService _authHandlerService = authHandlerService;

        public void RouteHandler(object? obj, int clientId)
        {
            Console.WriteLine($"Подключен новый клиент {clientId}");

            string? route = null;
            try
            {
                using TcpClient client = obj is not null ? (TcpClient)obj : throw new NullReferenceException();
                using NetworkStream stream = client.GetStream();

                route = stream.ReadString(64);

                switch (route)
                {
                    case "AddPublicKey":
                        _publicKeyHandlerService.AddPublicKey(stream);
                        break;
                    case "GetSecretMessage":
                        GetSecretMessageHandler(stream);
                        break;
                }
            }
            catch (IOException) { }
            finally
            {
                Console.WriteLine($"Клиент {clientId} отключился. (Route: '{route}')");
            }
        }

        private void GetSecretMessageHandler(NetworkStream stream)
        {
            bool isAuthenticated = _authHandlerService.CheckAuthByKeys(stream);
            if (!isAuthenticated) return;

            SendSecretMessage(stream);
        }

        private void SendSecretMessage(NetworkStream stream)
        {
            string secretMessage = "Secret message";
            stream.WriteString(secretMessage);
        }
    }
}
