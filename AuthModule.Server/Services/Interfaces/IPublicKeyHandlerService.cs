using System.Net.Sockets;

namespace AuthModule.Server.Services.Interfaces
{
    internal interface IPublicKeyHandlerService
    {
        void AddPublicKey(NetworkStream stream);
    }
}
