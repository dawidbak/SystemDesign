using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Notification.Application.Domain;
using Notification.Application.Infrastructure.Persistence;

namespace Notification.Application.Infrastructure.Repositories;

public interface ISettingRepository
{
    Task<List<Setting>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Setting?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(Setting setting, CancellationToken cancellationToken);
}

public class SettingRepository : ISettingRepository
{
    private readonly NotificationDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public SettingRepository(NotificationDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<List<Setting>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Settings
            .Where(s => s.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public Task<Setting?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Settings
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Setting setting, CancellationToken cancellationToken)
    {
        _dbContext.Settings.Update(setting);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _cache.InvalidateUserCacheAsync(setting.UserId, cancellationToken);
    }
}