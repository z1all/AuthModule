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
                            DrawKeys(keys);
                            break;
                        case "send pk":
                        case "send public_key":
                            _handlerService.AddPublicKeyHandler();
                            break;
                        case "get sm":
                        case "get secret_message":
                            _handlerService.GetSecretMessageHandler();
                            break;
                        case null: break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Неизвестная команда '{command}'");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void DrawKeys(Keys keys)
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
    }
}
