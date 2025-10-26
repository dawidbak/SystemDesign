using MassTransit;
using Notification.Application.Domain;
using Notification.Application.Infrastructure.Repositories;
using Shared.Contracts;

namespace Notification.Application.Features.Notification.SendPaymentReminderEmailNotification;

public class PaymentReminderCreatedEventHandler : IConsumer<PaymentReminderCreatedEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IPublishEndpoint _bus;
    private readonly ITemplateRepository _templateRepository;

    public PaymentReminderCreatedEventHandler(IUserRepository userRepository, IPublishEndpoint bus,
        ITemplateRepository templateRepository)
    {
        _userRepository = userRepository;
        _bus = bus;
        _templateRepository = templateRepository;
    }

    public async Task Consume(ConsumeContext<PaymentReminderCreatedEvent> context)
    {
        var message = context.Message;
        var user = await _userRepository.GetByIdAsync(message.UserId, context.CancellationToken);
        if (user == null)
            throw new Exception($"User with ID {message.UserId} not found.");

        if (!CanSendEmail(user))
            throw new Exception("User has not opted in for email notifications.");


        // For simulation purposes, we assume there's a latest email template for payment reminders
        var template = await _templateRepository.GetLatestByTypeAsync(ChannelType.Email, context.CancellationToken);
        if (template == null)
            throw new Exception("No email template found.");

        var @event = new EmailNotificationRequestedEvent
        (
            user.Email,
            template.Subject,
            template.Content
        );

        await _bus.Publish(@event, context.CancellationToken);
    }

    private static bool CanSendEmail(Domain.User user)
    {
        return user.Settings.First(x => x.Channel == ChannelType.Email).OptIn;
    }
}