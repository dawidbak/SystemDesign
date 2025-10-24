using Microsoft.AspNetCore.Routing;

namespace Notification.Application.Common;

public interface IEndpoint
{
     void RegisterEndpoint(INotificationRouteBuilder app);
}