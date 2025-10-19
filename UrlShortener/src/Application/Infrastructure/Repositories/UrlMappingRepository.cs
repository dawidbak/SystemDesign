using Application.Domain;
using Application.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Infrastructure.Repositories;

public interface IUrlMappingRepository
{
    Task<UrlMapping?> GetByOriginalUrl(string originalUrl, CancellationToken cancellationToken = default);
    Task<UrlMapping?> GetByShortUrl(string shortUrl, CancellationToken cancellationToken = default);
    Task Create(UrlMapping urlMapping, CancellationToken cancellationToken = default);
}

public class UrlMappingRepository : IUrlMappingRepository
{
    private readonly UrlShortenerDbContext _dbContext;

    public UrlMappingRepository(UrlShortenerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UrlMapping?> GetByOriginalUrl(string originalUrl, CancellationToken cancellationToken)
    {
        return await _dbContext.UrlMappings
            .FirstOrDefaultAsync(x => x.OriginalUrl == originalUrl, cancellationToken);
    }

    public async Task<UrlMapping?> GetByShortUrl(string shortUrl, CancellationToken cancellationToken)
    {
        return await _dbContext.UrlMappings
            .FirstOrDefaultAsync(x => x.ShortUrl == shortUrl, cancellationToken);
    }

    public async Task Create(UrlMapping urlMapping, CancellationToken cancellationToken)
    {
        await _dbContext.UrlMappings.AddAsync(urlMapping, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

public class CacheUrlMappingRepository : IUrlMappingRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly UrlMappingRepository _urlMappingRepository;

    public CacheUrlMappingRepository(IMemoryCache memoryCache, UrlMappingRepository repository)
    {
        _memoryCache = memoryCache;
        _urlMappingRepository = repository;
    }

    public async Task<UrlMapping?> GetByOriginalUrl(string originalUrl, CancellationToken cancellationToken)
    {
        return await _memoryCache.GetOrCreateAsync(originalUrl, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);
            return await _urlMappingRepository.GetByOriginalUrl(originalUrl, cancellationToken);
        });
    }

    public async Task<UrlMapping?> GetByShortUrl(string shortUrl, CancellationToken cancellationToken)
    {
        return await _memoryCache.GetOrCreateAsync(shortUrl, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);
            return await _urlMappingRepository.GetByShortUrl(shortUrl, cancellationToken);
        });
    }

    public async Task Create(UrlMapping urlMapping, CancellationToken cancellationToken)
    {
        await _urlMappingRepository.Create(urlMapping, cancellationToken);
    }
}