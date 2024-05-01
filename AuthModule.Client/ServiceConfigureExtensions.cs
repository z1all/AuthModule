using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthModule.Client.Configurations;
using AuthModule.Client.Services;
using AuthModule.Client.Stores;
using AuthModule.Client.Services.Interfaces;
using CryptoModule;

namespace AuthModule.Client
{
    public static class ServiceConfigureExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddServices();
            services.AddConfigurations();
        }

        public static void UseApplicationServices(this IServiceProvider services)
        {
            var client = services.GetRequiredService<Client>();
            client.Start();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<Client>();
            services.AddSingleton<HandlerClientService>();
            services.AddSingleton<IAuthHandlerService, AuthHandlerService>();
            services.AddSingleton<IPublicKeyHandlerService, PublicKeyHandlerService>();

            services.AddSingleton<IAuthClientService, AuthClientService>();
            services.AddSingleton<IKeysStore, FileKeysStore>();

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
