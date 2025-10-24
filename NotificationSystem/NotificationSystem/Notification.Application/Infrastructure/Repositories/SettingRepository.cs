using Microsoft.EntityFrameworkCore;
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

    public SettingRepository(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
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
    }
}