using AuthModule.Client.Services;
using CryptoModule;

namespace AuthModule.Client
{
    internal class Client
    {
        private readonly HandlerClientService _handlerService;

        private bool _work = true;

        public Client(HandlerClientService handlerService)
        {
            _handlerService = handlerService;
        }

        public void Start()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Клиент запущен...");

            while (_work)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(">> ");

                    string? command = Console.ReadLine();

                    switch(command)
                    {
                        case "create ks":
                        case "create keys":
                            Keys keys = _handlerService.CreateKeysHandler();
                            DisplayKeys(keys);
                            break;
                        case "send pk":
                        case "send public_key":
                            _handlerService.AddPublicKeyHandler();
                            break;
                        case "get sm":
                        case "get secret_message":
                            _handlerService.GetSecretMessageHandler();
                            break;
                        case "help":
                            DisplayHelps();
                            break;
                        case null: break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Неизвестная команда '{command}'. Введите help для получения списка команд.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void DisplayKeys(Keys keys)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Privat key: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(keys.PrivateKey);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Public key: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(keys.PublicKey);
        }

        private void DisplayHelps()
        {
            Console.WriteLine("create keys (сокр. create ks) \t\t- Создать новую пару ключей");
            Console.WriteLine("send public_key (сокр. send pk) \t- Отправить свой текущий публичный ключ на сервер");
            Console.WriteLine("get secret_message (сокр. get sm) \t- Получить секретное сообщение с сервера (Перед получение идет процесс аутентификации)");
        }
    }
}
