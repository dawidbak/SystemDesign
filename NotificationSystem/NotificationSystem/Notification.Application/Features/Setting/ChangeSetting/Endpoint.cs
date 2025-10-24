using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Common;

namespace Notification.Application.Features.Setting.ChangeSetting;

public class Endpoint : IEndpoint
{
    public void RegisterEndpoint(INotificationRouteBuilder app)
        => app.Settings.MapPatch("", async (
                    ChangeSetting changeSetting,
                    [FromServices] IHttpCommandHandler<ChangeSetting> handler,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(changeSetting, cancellationToken))
            .WithName("ChangeSetting")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
}