using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Common;

namespace Notification.Application.Features.Setting.GetSettings;

public class Endpoint : IEndpoint
{
    public void RegisterEndpoint(INotificationRouteBuilder app)
        => app.Settings.MapGet("", async (
                    Guid userId,
                    [FromServices] IHttpQueryHandler<GetSettings> handler,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(new GetSettings(userId), cancellationToken))
            .WithName("GetSettings")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
}