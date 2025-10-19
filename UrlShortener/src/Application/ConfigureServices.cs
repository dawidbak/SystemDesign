using Application.Common;
using Application.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureServices
{
    public static void MapApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.MapInfrastructure(configuration);
        services.RegisterHandlers();
        services.RegisterOptions(configuration);
    }
}