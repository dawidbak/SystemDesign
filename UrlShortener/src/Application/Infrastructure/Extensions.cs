using Application.Common.Options;
using Application.Infrastructure.Persistence;
using Application.Infrastructure.Repositories;
using Application.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Infrastructure;

public static class Extensions
{
    public static void MapInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UrlShortenerDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<UrlMappingRepository>();
        services.AddScoped<IUrlMappingRepository, CacheUrlMappingRepository>();
        services.AddMemoryCache();
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));
        services.AddScoped<ISnowflakeService, SnowflakeService>();
    }
}