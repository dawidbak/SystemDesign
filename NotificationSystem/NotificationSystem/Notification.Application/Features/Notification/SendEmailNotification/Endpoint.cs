using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Common;

namespace Notification.Application.Features.Notification.SendEmailNotification;

public class Endpoint : IEndpoint
{
    public void RegisterEndpoint(INotificationRouteBuilder app)
        => app.Notifications.MapPost("/sendEmail", async (
                    SendEmailNotification sendEmailNotification,
                    [FromServices] IHttpCommandHandler<SendEmailNotification> handler,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(sendEmailNotification, cancellationToken))
            .WithName("SendEmailNotification")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
}