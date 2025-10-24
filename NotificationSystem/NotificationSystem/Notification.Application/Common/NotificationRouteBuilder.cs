using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Notification.Application.Common;

public interface INotificationRouteBuilder
{
    public IEndpointRouteBuilder Notifications { get; }
    public IEndpointRouteBuilder Templates { get; }
    public IEndpointRouteBuilder Settings { get; }
}

public class NotificationRouteBuilder : INotificationRouteBuilder
{
    private readonly IEndpointRouteBuilder _endpointRouteBuilder;

    public NotificationRouteBuilder(IEndpointRouteBuilder endpointRouteBuilder)
    {
        _endpointRouteBuilder = endpointRouteBuilder;
    }

    public IEndpointRouteBuilder Notifications => _endpointRouteBuilder
        .MapGroup("api/notifications");
    public IEndpointRouteBuilder Templates => _endpointRouteBuilder
        .MapGroup("api/templates");
    public IEndpointRouteBuilder Settings => _endpointRouteBuilder
        .MapGroup("api/settings");
}

public static class GroceryStoreRouteBuilderExtensions
{
    public static INotificationRouteBuilder ToNotificationRouteBuilder(this IEndpointRouteBuilder endpoints) =>
        new NotificationRouteBuilder(endpoints);
}