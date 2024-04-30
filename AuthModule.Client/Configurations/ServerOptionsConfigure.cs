using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AuthModule.Client.Configurations
{
    internal class ServerOptionsConfigure(IConfiguration configuration) : IConfigureOptions<ServerOptions>
    {
        private readonly string valueKey = "server";
        private readonly IConfiguration _configuration = configuration;

        public void Configure(ServerOptions options) => _configuration.GetSection(valueKey).Bind(options);
    }
}
