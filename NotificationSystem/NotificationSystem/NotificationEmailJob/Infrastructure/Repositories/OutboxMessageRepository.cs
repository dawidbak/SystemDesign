using Microsoft.EntityFrameworkCore;
using NotificationJob.Outbox;
using Shared.Contracts;

namespace NotificationJob.Infrastructure.Repositories;

public interface IOutboxMessageRepository
{
    Task AddAsync(EmailNotificationRequestedEvent message, Guid messageId, CancellationToken cancellationToken);
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken);
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken);
    Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken);
}

public class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly NotificationEmailJobDbContext _dbContext;

    public OutboxMessageRepository(NotificationEmailJobDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(EmailNotificationRequestedEvent message, Guid messageId,
        CancellationToken cancellationToken)
    {
        var outboxMessage = new OutboxMessage
        {
            Id = messageId,
            Content = System.Text.Json.JsonSerializer.Serialize(message),
            OccurredOnUtc = TimeProvider.System.GetUtcNow().UtcDateTime,
            ProcessedOnUtc = null,
            Error = null
        };

        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken)
    {
        return _dbContext.OutboxMessages
            .Where(om => om.ProcessedOnUtc == null)
            .OrderBy(om => om.OccurredOnUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken)
    {
        await _dbContext.OutboxMessages
            .Where(om => om.Id == messageId)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(om => om.ProcessedOnUtc, TimeProvider.System.GetUtcNow().UtcDateTime),
                cancellationToken);
    }

    public async Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken)
    {
        await _dbContext.OutboxMessages
            .Where(om => om.Id == messageId)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(om => om.Error, error)
                    .SetProperty(om => om.ProcessedOnUtc, TimeProvider.System.GetUtcNow().UtcDateTime),
                cancellationToken);
    }
}