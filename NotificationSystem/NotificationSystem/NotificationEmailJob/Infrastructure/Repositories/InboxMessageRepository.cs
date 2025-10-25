using Microsoft.EntityFrameworkCore;
using NotificationJob.Inbox;

namespace NotificationJob.Infrastructure.Repositories;

public interface IInboxMessageRepository
{
    Task AddAsync(Guid messageId, CancellationToken cancellationToken);
    Task<InboxMessage?> GetAsync(Guid messageId, CancellationToken cancellationToken);
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken);
}

public class InboxMessageRepository : IInboxMessageRepository
{
    private readonly NotificationEmailJobDbContext _dbContext;

    public InboxMessageRepository(NotificationEmailJobDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var inboxMessage = new InboxMessage
        {
            Id = messageId,
            OccurredOnUtc = TimeProvider.System.GetUtcNow().UtcDateTime
        };

        await _dbContext.InboxMessages.AddAsync(inboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<InboxMessage?> GetAsync(Guid messageId, CancellationToken cancellationToken)
    {
        return await _dbContext.InboxMessages
            .FirstOrDefaultAsync(im => im.Id == messageId, cancellationToken);
    }

    public Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken)
    {
        return _dbContext.InboxMessages
            .Where(im => im.Id == messageId)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(im => im.ProcessedOnUtc, TimeProvider.System.GetUtcNow().UtcDateTime),
                cancellationToken);
    }
}