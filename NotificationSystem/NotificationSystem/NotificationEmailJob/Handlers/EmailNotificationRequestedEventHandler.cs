using MassTransit;
using NotificationJob.Infrastructure.Repositories;
using Shared.Contracts;

namespace NotificationJob.Handlers;

public class EmailNotificationRequestedEventHandler : IConsumer<EmailNotificationRequestedEvent>
{
    private readonly IInboxMessageRepository _inboxMessageRepository;
    private readonly IOutboxMessageRepository _outboxMessageRepository;

    public EmailNotificationRequestedEventHandler(IInboxMessageRepository inboxMessageRepository,
        IOutboxMessageRepository outboxMessageRepository)
    {
        _inboxMessageRepository = inboxMessageRepository;
        _outboxMessageRepository = outboxMessageRepository;
    }

    public async Task Consume(ConsumeContext<EmailNotificationRequestedEvent> context)
    {
        var message = context.Message;
        var messageId = context.MessageId;
        Console.WriteLine($"Received email notification request for {message.RecipientEmail}");

        var inboxMessage = await _inboxMessageRepository.GetAsync(messageId!.Value, context.CancellationToken);
        if (inboxMessage?.ProcessedOnUtc != null)
        {
            return;
        }

        await _inboxMessageRepository.AddAsync(messageId!.Value, context.CancellationToken);

        try
        {
            await _outboxMessageRepository.AddAsync(message, messageId!.Value, context.CancellationToken);
            await _inboxMessageRepository.MarkAsProcessedAsync(messageId!.Value, context.CancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing email notification request: {ex.Message}");
            throw;
        }
    }
}