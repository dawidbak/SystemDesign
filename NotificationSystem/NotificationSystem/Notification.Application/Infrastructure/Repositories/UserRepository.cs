using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Notification.Application.Common;
using Notification.Application.Domain;
using Notification.Application.Infrastructure.Persistence;

namespace Notification.Application.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
}

public class UserRepository : IUserRepository
{
    private readonly NotificationDbContext _dbContext;

    public UserRepository(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(d => d.Devices)
            .Include(s => s.Settings)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

public class CacheUserRepository : IUserRepository
{
    private readonly IUserRepository _repository;
    private readonly IDistributedCache _cache;

    public CacheUserRepository(IUserRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheUser = await _cache.GetStringAsync(CacheKeys.User(id), token: cancellationToken);
        if (cacheUser is not null)
        {
            return System.Text.Json.JsonSerializer.Deserialize<User>(cacheUser);
        }

        var user = await _repository.GetByIdAsync(id, cancellationToken);
        await _cache.SetStringWithSlidingAsync(
            CacheKeys.User(id),
            System.Text.Json.JsonSerializer.Serialize(user), cancellationToken);
        return user;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _repository.AddAsync(user, cancellationToken);
        await _cache.SetStringWithSlidingAsync(
            CacheKeys.User(user.Id),
            System.Text.Json.JsonSerializer.Serialize(user), cancellationToken);
    }
}