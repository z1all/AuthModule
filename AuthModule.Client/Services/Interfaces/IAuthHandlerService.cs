using System.Net.Sockets;

namespace AuthModule.Client.Services.Interfaces
{
    internal interface IAuthHandlerService
    {
        bool Auth(NetworkStream stream);
    }
}
