using System.Reflection;
using Application.Common.Options;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common;

public static class EndpointsExtensions
{
    public static void RegisterHandlers(this IServiceCollection services)
    {
        var assembly = Assembly.GetAssembly(typeof(EndpointsExtensions))!;
        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IHttpQueryHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IHttpCommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

    public static void RegisterOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ShortenerOptions>(
            configuration.GetSection(ShortenerOptions.Shortener));
    }

    public static void RegisterEndpoints(this IEndpointRouteBuilder app)
    {
        var assembly = Assembly.GetAssembly(typeof(EndpointsExtensions));
        var endpoints = assembly!
            .GetTypes()
            .Where(x => typeof(IEndpoint).IsAssignableFrom(x) && x.IsClass)
            .OrderBy(x => x.Name)
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>()
            .ToList();

        endpoints.ForEach(x => x.RegisterEndpoint(app));
    }
}