using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Common;

namespace Notification.Application.Features.Template.GetTemplates;

public class Endpoint : IEndpoint
{
    public void RegisterEndpoint(INotificationRouteBuilder app)
        => app.Templates.MapGet("", async (
                    [FromServices] IHttpQueryHandler<GetTemplatesQuery> handler,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(new GetTemplatesQuery(), cancellationToken))
            .WithName("GetTemplates")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
}

