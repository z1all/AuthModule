using System.Net.Sockets;

namespace AuthModule.Server.Services.Interfaces
{
    internal interface IAuthHandlerService
    {
        bool CheckAuthByKeys(NetworkStream stream);
    }
}
