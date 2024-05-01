using CryptoModule;
using System.Net.Sockets;

namespace AuthModule.Client.Services.Interfaces
{
    internal interface IPublicKeyHandlerService
    {
        Keys GetNewKeys();
        void SendPublicKey(NetworkStream stream);
    }
}
