using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Net.Sockets.Extension;
using AuthModule.Client.Configurations;
using AuthModule.Client.Services.Interfaces;
using CryptoModule;

namespace AuthModule.Client.Services
{
    internal class HandlerClientService(
        IAuthHandlerService authHandlerService,
        IPublicKeyHandlerService publicKeyHandlerService,
        IOptions<ServerOptions> options
    )
    {
        private readonly IAuthHandlerService _authHandlerService = authHandlerService;
        private readonly IPublicKeyHandlerService _publicKeyHandlerService = publicKeyHandlerService;

        private readonly ServerOptions _serverOptions = options.Value;
  
        public Keys CreateKeysHandler()
        {
            return _publicKeyHandlerService.GetNewKeys();
        }   

        public void AddPublicKeyHandler()
        {
            TcpHandler((stream) =>
            {
                _publicKeyHandlerService.SendPublicKey(stream);
            }, "AddPublicKey");
        }

        public void GetSecretMessageHandler()
        {
            TcpHandler((stream) =>
            {
                bool isAuthenticated = _authHandlerService.Auth(stream);
                if (!isAuthenticated) return;

                GetSecretMessage(stream);
            }, "GetSecretMessage");
        }

        private void GetSecretMessage(NetworkStream stream)
        {
            string secretMessage = stream.ReadString(1024);

            Console.WriteLine($"Секретное сообщение: '{secretMessage}'.");
        }

        private void TcpHandler(Action<NetworkStream> action, string route)
        {
            Console.WriteLine("Клиент подключен к серверу");

            try
            {
                using TcpClient client = new TcpClient(_serverOptions.Host, _serverOptions.Port);
                using NetworkStream stream = client.GetStream();

                SendRoute(stream, route);

                action(stream);
            }
            finally
            {
                Console.WriteLine("Клиент отключен от сервера");
            }
        }

        private void SendRoute(NetworkStream stream, string route)
        {
            stream.WriteString(route);

            Thread.Sleep(100);
        }
    }
}
