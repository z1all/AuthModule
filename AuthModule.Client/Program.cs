using Microsoft.Extensions.DependencyInjection;
using AuthModule.Client;

var services = new ServiceCollection();
services.AddApplicationServices();

var serviceProvider = services.BuildServiceProvider();
serviceProvider.UseApplicationServices();