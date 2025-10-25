using System.Text.Json;
using NotificationJob.Infrastructure;
using NotificationJob.Infrastructure.Repositories;
using NotificationJob.Services;
using Shared.Contracts;

namespace NotificationJob.Outbox;

public class OutboxProcessorJob : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));

    const int BatchSize = 10;

    public OutboxProcessorJob(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested && await _timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var fakeEmailService = scope.ServiceProvider.GetRequiredService<IFakeEmailService>();
            var outboxMessageRepository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var outboxMessages = await outboxMessageRepository.GetUnprocessedMessagesAsync(BatchSize, stoppingToken);
            foreach (var outboxMessage in outboxMessages)
            {
                await using var transaction = await unitOfWork.BeginTransactionAsync(stoppingToken);
                try
                {
                    var @event = JsonSerializer.Deserialize<EmailNotificationRequestedEvent>(outboxMessage.Content);

                    await fakeEmailService.SendEmail(@event!.RecipientEmail, @event.Subject, @event.Body);
                    await outboxMessageRepository.MarkAsProcessedAsync(outboxMessage.Id, stoppingToken);

                    await transaction.CommitAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(stoppingToken);
                    await outboxMessageRepository.MarkAsFailedAsync(outboxMessage.Id, ex.Message, stoppingToken);
                }
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Dispose();
        Console.WriteLine("Stopping outbox processor");
        return Task.CompletedTask;
    }
}