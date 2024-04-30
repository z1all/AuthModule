using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthModule.Server.Configurations;
using AuthModule.Server.Services;
using AuthModule.Server.Store;
using AuthModule.Server.Stores;
using CryptoModule;

namespace AuthModule.Server
{
    internal static class ServiceConfigureExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddServices();
            services.AddConfigurations();
        }

        public static void UseApplicationServices(this IServiceProvider services)
        {
            var server = services.GetRequiredService<Server>();
            server.Start();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<Server>();
            services.AddSingleton<HandlerService>();
            services.AddSingleton<IKeysStore, FileKeysStore>();
            services.AddSingleton<IProfileStore, FileProfileStore>();
            services.AddCryptoService();
        }

        private static void AddConfigurations(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();
            services.AddSingleton<IConfiguration>(configuration);

            services.ConfigureOptions<ServerOptionsConfigure>();
        }
    }
}
