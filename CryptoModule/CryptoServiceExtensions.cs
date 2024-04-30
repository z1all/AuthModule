using Microsoft.Extensions.DependencyInjection;

namespace CryptoModule
{
    public static class CryptoServiceExtensions
    {
        public static void AddCryptoService(this IServiceCollection services)
        {
            services.AddSingleton<ICryptoService, RSACryptoService>();
        }
    }
}
