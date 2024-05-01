using System.Net.Sockets;
using System.Net.Sockets.Extension;
using AuthModule.Client.Services.Interfaces;
using CryptoModule;

namespace AuthModule.Client.Services
{
    internal class PublicKeyHandlerService (
        IAuthClientService authClientService
    ) : IPublicKeyHandlerService
    {
        private readonly IAuthClientService _authClientService = authClientService;

        public Keys GetNewKeys()
        {
            Console.WriteLine("Введите passphrase для шифрования приватного ключа:");
            string? passphrase = Console.ReadLine();

            return _authClientService.GetNewKeys(passphrase);
        }

        public void SendPublicKey(NetworkStream stream)
        {
            UserNameAndPasswordAuthHandler(stream);
            SendPublicKeyHandler(stream);
        }

        private void UserNameAndPasswordAuthHandler(NetworkStream stream)
        {
            do
            {
                Console.WriteLine("Введите логин:");
                string userName = Console.ReadLine() ?? "";
                stream.WriteString(userName);

                Console.WriteLine("Введите пароль:");
                string password = Console.ReadLine() ?? "";
                stream.WriteString(password);

                int authStatus = stream.ReadByte();
                if (authStatus == 0)
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

        private void SendPublicKeyHandler(NetworkStream stream)
        {
            Console.WriteLine("Отправка публичного ключа...");
            string publicKey = _authClientService.GetCurrentPublicKey();
            stream.WriteString(publicKey);

            int saveStatus = stream.ReadByte();
            if (saveStatus == 0)
            {
                Console.WriteLine("Ключ сохранен");
            }
            else
            {
                Console.WriteLine("Такой ключ уже сохранен на сервере.");
            }
        }
    }
}
