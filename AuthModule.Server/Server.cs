using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using AuthModule.Server.Configurations;
using AuthModule.Server.Services;

namespace AuthModule.Server
{
    internal class Server
    {
        private readonly TcpListener _server;
        private readonly HandlerServerService _handlerService;

        private bool _listening = true;

        public Server(IOptions<ServerOptions> options, HandlerServerService handlerService)
        {
            ServerOptions serverOptions = options.Value;
            _handlerService = handlerService;

            _server = new TcpListener(IPAddress.Parse(serverOptions.Host), serverOptions.Port);
        }

        public void Start()
        {
            _server.Start();

            Console.WriteLine("Сервер запущен...");

            try
            {
                int clientCount = 0;

                while (_listening)
                {
                    int clientId = clientCount;

                    TcpClient client = _server.AcceptTcpClient();
                    
                    Thread clientThread = new Thread(new ParameterizedThreadStart((obj) => _handlerService.RouteHandler(obj, clientId)));
                    clientThread.Start(client);

                    clientCount += 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _server.Stop();
            }
        }
    }
}
