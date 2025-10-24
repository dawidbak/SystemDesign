using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Common;
using Notification.Application.Domain;

namespace Notification.Application.Features.Template.GetNewestTemplate;

public class Endpoint : IEndpoint
{
    public void RegisterEndpoint(INotificationRouteBuilder app)
        => app.Templates.MapGet("/newest", async (
                    ChannelType type,
                    [FromServices] IHttpQueryHandler<GetNewestTemplate> handler,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(new GetNewestTemplate(type), cancellationToken))
            .WithName("GetNewestTemplate")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
}



