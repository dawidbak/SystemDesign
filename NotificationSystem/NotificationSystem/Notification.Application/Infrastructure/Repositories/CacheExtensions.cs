using Microsoft.Extensions.Caching.Distributed;
using Notification.Application.Common;

namespace Notification.Application.Infrastructure.Repositories;

public static class CacheExtensions
{
    private static readonly DistributedCacheEntryOptions DefaultOptions = new()
    {
        SlidingExpiration = TimeSpan.FromHours(12),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
    };

    public static Task SetStringWithSlidingAsync(
        this IDistributedCache cache,
        string key,
        string value,
        CancellationToken token = default)
    {
        return cache.SetStringAsync(key, value, DefaultOptions, token);
    }

    public static Task InvalidateAsync(
        this IDistributedCache cache,
        string key,
        CancellationToken token = default)
    {
        return cache.RemoveAsync(key, token);
    }

    public static Task InvalidateUserCacheAsync(
        this IDistributedCache cache,
        Guid userId,
        CancellationToken token = default)
    {
        return cache.RemoveAsync(CacheKeys.User(userId), token);
    }
}