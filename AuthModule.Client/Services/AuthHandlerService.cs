using System.Net.Sockets;
using System.Net.Sockets.Extension;
using AuthModule.Client.Services.Interfaces;

namespace AuthModule.Client.Services
{
    internal class AuthHandlerService(
        IAuthClientService authClientService
    ) : IAuthHandlerService
    {
        private readonly IAuthClientService _authClientService = authClientService;

        public bool Auth(NetworkStream stream)
        {
            string publicKey = _authClientService.GetCurrentPublicKey();
            stream.WriteString(publicKey);

            int sendKeyStatus = stream.ReadByte();
            if (sendKeyStatus == 1)
            {
                Console.WriteLine("Аутентификация провалилась. На сервере отсутствует текущий публичный ключ.");
                return false;
            }

            byte[] encryptMassageBytes = stream.ReadBytes(1024);
            byte[] decryptMassageBytes = _authClientService.DecryptRandomMessage(encryptMassageBytes);
            stream.WriteBytes(decryptMassageBytes);

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
    }
}
