using Microsoft.Extensions.DependencyInjection;
using CryptoModule.Interfaces;
using CryptoModule.Services;

namespace CryptoModule
{
    public static class CryptoServiceExtensions
    {
        public static void AddCryptoService(this IServiceCollection services)
        {
            services.AddSingleton<IAsymmetricCryptoService, RSACryptoService>();
            services.AddSingleton<ISymmetricCryptoService, AesCryptoService>();
        }
    }
}
