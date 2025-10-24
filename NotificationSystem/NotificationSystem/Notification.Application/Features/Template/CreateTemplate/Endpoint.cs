using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Common;

namespace Notification.Application.Features.Template.CreateTemplate;

public class Endpoint : IEndpoint
{
    public void RegisterEndpoint(INotificationRouteBuilder app)
        => app.Templates.MapPost("", async (
                    CreateTemplate createTemplate,
                    [FromServices] IHttpCommandHandler<CreateTemplate> handler,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(createTemplate, cancellationToken))
            .WithName("CreateTemplate")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
}