using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Infrastructure.Persistence;
using Notification.Application.Infrastructure.Repositories;

namespace Notification.Application.Infrastructure;

public static class Extensions
{
    public static void MapInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.Decorate<IUserRepository, CacheUserRepository>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.Decorate<ITemplateRepository, CacheTemplateRepository>();
        services.AddScoped<ISettingRepository, SettingRepository>();
    }
}

