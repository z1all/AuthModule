using System.Net.Sockets;
using System.Text;
using AuthModule.Server.Helpers;
using AuthModule.Server.Store;
using AuthModule.Server.Stores;
using CryptoModule;

namespace AuthModule.Server.Services
{
    internal class HandlerService
    {
        private readonly IKeysStore _fileKeysStore;
        private readonly IProfileStore _profileStore;
        private readonly ICryptoService _cryptoService;

        public HandlerService(IKeysStore fileKeysStore, IProfileStore profileStore, ICryptoService cryptoService)
        {
            _fileKeysStore = fileKeysStore;
            _profileStore = profileStore;
            _cryptoService = cryptoService;
        }

        public void RouteHandler(object? obj, int clientId)
        {
            Console.WriteLine($"Подключен новый клиент {clientId}");

            string? route = null;
            try
            {
                using TcpClient client = obj is not null ? (TcpClient)obj : throw new NullReferenceException();
                using NetworkStream stream = client.GetStream();

                byte[] data = new byte[64];
                int routLength = stream.Read(data, 0, data.Length);
                route = Encoding.UTF8.GetString(data, 0, routLength);

                switch (route)
                {
                    case "AddPublicKey":
                        AddPublicKeyHandler(stream);
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

        private void AddPublicKeyHandler(NetworkStream stream)
        {
            bool checkingUserNameAndPasswordResult = CheckUserNameAndPassword(stream);
            if (!checkingUserNameAndPasswordResult) return;

            bool CheckingPublicKeyResult = CheckPublicKey(stream);
            if (!CheckingPublicKeyResult) return;
        }
       
        private void GetSecretMessageHandler(NetworkStream stream)
        {
            bool keyExist = CheckKeyInStorage(stream, out string publicKey);
            if (!keyExist) return;

            bool userRight = CheckUserOnRandomMessage(stream, publicKey);
            if (!userRight) return;

            SendSecretMessage(stream);
        }

        private bool CheckUserNameAndPassword(NetworkStream stream)
        {
            byte[] data = new byte[1024];

            do
            {
                int userNameLength = stream.Read(data, 0, data.Length);
                string userName = Encoding.UTF8.GetString(data, 0, userNameLength);

                int passwordLength = stream.Read(data, 0, data.Length);
                string password = Encoding.UTF8.GetString(data, 0, passwordLength);

                bool rightUserNameAndPassword = _profileStore.CheckUserNameAndPassword(userName, password);

                if (rightUserNameAndPassword)
                {
                    SendSuccess(stream);
                    return true;
                }
                else
                {
                    SendFail(stream);
                }
            }
            while (true);
        }

        private bool CheckPublicKey(NetworkStream stream)
        {
            byte[] data = new byte[1024];

            do
            {
                int keyLength = stream.Read(data, 0, data.Length);
                string key = Encoding.UTF8.GetString(data, 0, keyLength);

                bool keyExist = _fileKeysStore.FindKey(key);
                if (!keyExist)
                {
                    _fileKeysStore.SaveKey(key);
                    SendSuccess(stream);
                    return true;
                }
                else
                {
                    SendFail(stream);
                }
            }
            while (true);
        }

        private bool CheckKeyInStorage(NetworkStream stream, out string publicKey)
        {
            byte[] data = new byte[1024];

            int publicKeyLength = stream.Read(data, 0, data.Length);
            publicKey = Encoding.UTF8.GetString(data, 0, publicKeyLength);

            bool keyExist = _fileKeysStore.FindKey(publicKey);
            if (!keyExist)
            {
                SendFail(stream);
                return false;
            }

            return true;
        }

        private bool CheckUserOnRandomMessage(NetworkStream stream, string publicKey)
        {
            byte[] data = new byte[1024];

            string randomMessage = TextHelper.GenerateRandomString(100);

            byte[] encryptMassage = _cryptoService.Encrypt(publicKey, Encoding.UTF8.GetBytes(randomMessage));
            stream.Write(encryptMassage, 0, encryptMassage.Length);

            int decryptRandomMessageLength = stream.Read(data, 0, data.Length);
            string decryptRandomMessage = Encoding.UTF8.GetString(data, 0, decryptRandomMessageLength);
            if (decryptRandomMessage != randomMessage)
            {
                SendFail(stream);
                return false;
            }

            return true;
        }

        private void SendSecretMessage(NetworkStream stream)
        {
            string secretMessage = "Secret message";
            byte[] SecretMessageBytes = Encoding.UTF8.GetBytes(secretMessage);
            stream.Write(SecretMessageBytes, 0, SecretMessageBytes.Length);
        }

        private void SendSuccess(NetworkStream stream) => stream.WriteByte(0);
        private void SendFail(NetworkStream stream) => stream.WriteByte(1);
    }
}
