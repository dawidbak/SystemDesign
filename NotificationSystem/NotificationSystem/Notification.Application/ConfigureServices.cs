using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Common;
using Notification.Application.Infrastructure;

namespace Notification.Application;

public static class ConfigureServices
{
    public static void MapApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.MapInfrastructure(configuration);
        services.RegisterHandlers();
        services.AddValidatorsFromAssembly(typeof(ConfigureServices).Assembly);
    }
}