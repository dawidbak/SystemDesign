using MassTransit;
using Microsoft.AspNetCore.Http;
using Notification.Application.Common;
using Notification.Application.Domain;
using Notification.Application.Infrastructure.Repositories;
using Shared.Contracts;
using Wolverine;

namespace Notification.Application.Features.Notification.SendEmailNotification;

public class SendEmailNotificationHandler : IHttpCommandHandler<SendEmailNotification>
{
    private readonly ITemplateRepository _templateRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPublishEndpoint _bus;

    public SendEmailNotificationHandler(ITemplateRepository templateRepository, IUserRepository userRepository,
        IPublishEndpoint bus)
    {
        _templateRepository = templateRepository;
        _userRepository = userRepository;
        _bus = bus;
    }

    public async Task<IResult> HandleAsync(SendEmailNotification command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user == null)
        {
            return Results.NotFound($"User with ID {command.UserId} not found.");
        }

        if (!CanSendEmail(user))
        {
            return Results.BadRequest("User has not opted in for email notifications.");
        }

        var template = await _templateRepository.GetByIdAsync(command.TemplateId, cancellationToken);
        if (template == null)
        {
            return Results.NotFound($"Template with ID {command.TemplateId} is invalid");
        }

        var @event = new EmailNotificationRequestedEvent
        (
            user.Email,
            template.Subject,
            template.Content
        );

        await _bus.Publish(@event, cancellationToken);
        return Results.Ok(new { Message = "Email notification requested successfully." });
    }

    private static bool CanSendEmail(Domain.User user)
    {
        return user.Settings.First(x => x.Channel == ChannelType.Email).OptIn;
    }
}