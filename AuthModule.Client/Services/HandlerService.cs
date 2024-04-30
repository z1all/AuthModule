using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Text;
using AuthModule.Client.Configurations;
using AuthModule.Client.Stores;
using CryptoModule;

namespace AuthModule.Client.Services
{
    internal class HandlerService
    {
        private readonly IKeysStore _keysStore;
        private readonly ICryptoService _cryptoService;

        private readonly ServerOptions _serverOptions;

        public HandlerService(IKeysStore keysStore, ICryptoService cryptoService, IOptions<ServerOptions> options)
        {
            _keysStore = keysStore;
            _cryptoService = cryptoService;
            _serverOptions = options.Value; 
        }

        public Keys CreateKeysHandler()
        {
            Keys newKeys = _cryptoService.MakeKeysPair();

            _keysStore.SaveKeys(newKeys);

            return newKeys;
        }   

        public void AddPublicKeyHandler()
        {
            TcpHandler((stream) =>
            {
                UserNameAndPasswordAuth(stream);
                SendPublicKey(stream);
            }, "AddPublicKey");
        }

        public void GetSecretMessageHandler()
        {
            TcpHandler((stream) =>
            {
                bool isAuthenticated = Auth(stream);
                if (!isAuthenticated) return;

                GetSecretMessage(stream);
            }, "GetSecretMessage");
        }

        private bool Auth(NetworkStream stream)
        {
            Keys keys = _keysStore.GetKeys();

            byte[] publicKeyBytes = Encoding.UTF8.GetBytes(keys.PublicKey);
            stream.Write(publicKeyBytes, 0, publicKeyBytes.Length);

            int sendKeyStatus = stream.ReadByte();
            if (sendKeyStatus == 1)
            {
                Console.WriteLine("Аутентификация провалилась. На сервере отсутствует текущий публичный ключ.");
                return false;
            }

            byte[] data = new byte[1024];
            int encryptMassageLength = stream.Read(data, 0, data.Length);
            byte[] encryptMassageBytes = new byte[encryptMassageLength];
            Array.Copy(data, encryptMassageBytes, encryptMassageBytes.Length);

            byte[] decryptMassageBytes = _cryptoService.Decrypt(keys.PrivateKey, encryptMassageBytes);
            stream.Write(decryptMassageBytes, 0, decryptMassageBytes.Length);

            int authStatus = stream.ReadByte();
            if (authStatus == 1)
            {
                Console.WriteLine("Аутентификация провалилась. Неверный закрытый ключ.");
                return false;
            }
            else
            {
                Console.WriteLine("Успешная аутентификация.");
                return true;
            }
        }

        private void GetSecretMessage(NetworkStream stream)
        {
            byte[] data = new byte[1024];
            int dataLength = stream.Read(data, 0, data.Length);
            string secretMessage = Encoding.UTF8.GetString(data, 0, dataLength);

            Console.WriteLine($"Секретное сообщение: '{secretMessage}'.");
        }

        private void UserNameAndPasswordAuth(NetworkStream stream)
        {
            do
            {
                Console.WriteLine("Введите логин:");
                string userName = Console.ReadLine() ?? "";
                byte[] responseUserName = System.Text.Encoding.UTF8.GetBytes(userName);
                stream.Write(responseUserName, 0, responseUserName.Length);

                Console.WriteLine("Введите пароль:");
                string password = Console.ReadLine() ?? "";
                byte[] responsePassword = System.Text.Encoding.UTF8.GetBytes(password);
                stream.Write(responsePassword, 0, responsePassword.Length);

                byte[] data = new byte[1];
                stream.Read(data, 0, data.Length);

                if (data[0] == 0)
                {
                    Console.WriteLine("Авторизован");
                    break;
                }
                else
                {
                    Console.WriteLine("Неверный логин или пароль");
                }
            }
            while (true);
        }

        private void SendPublicKey(NetworkStream stream)
        {
            Console.WriteLine("Отправка публичного ключа...");
            string key = _keysStore.GetKeys().PublicKey;
            byte[] responseKey = Encoding.UTF8.GetBytes(key);
            stream.Write(responseKey, 0, responseKey.Length);

            byte[] data = new byte[1];
            stream.Read(data, 0, data.Length);

            if (data[0] == 0)
            {
                Console.WriteLine("Ключ сохранен");
            }
            else
            {
                Console.WriteLine("Такой ключ уже сохранен на сервере.");
            }
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
            byte[] routeBytes = Encoding.UTF8.GetBytes(string.Concat(route));
            stream.Write(routeBytes, 0, routeBytes.Length);
            stream.Flush();

            Thread.Sleep(100);
        }
    }
}
