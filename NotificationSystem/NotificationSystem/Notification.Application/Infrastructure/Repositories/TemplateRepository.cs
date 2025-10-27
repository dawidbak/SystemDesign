using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Notification.Application.Common;
using Notification.Application.Domain;
using Notification.Application.Infrastructure.Persistence;

namespace Notification.Application.Infrastructure.Repositories;

public interface ITemplateRepository
{
    Task<Template?> GetByIdAsync(int templateId, CancellationToken cancellationToken);
    Task<List<Template>> GetAllAsync(CancellationToken cancellationToken);
    Task<Template?> GetLatestByTypeAsync(ChannelType type, CancellationToken cancellationToken);
    Task<Template> CreateAsync(Template template, CancellationToken cancellationToken);
}

public class TemplateRepository : ITemplateRepository
{
    private readonly NotificationDbContext _dbContext;

    public TemplateRepository(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Template?> GetByIdAsync(int templateId, CancellationToken cancellationToken)
    {
        return await _dbContext.Templates
            .FirstOrDefaultAsync(x => x.Id == templateId, cancellationToken);
    }

    public async Task<List<Template>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Templates
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Template?> GetLatestByTypeAsync(ChannelType type, CancellationToken cancellationToken)
    {
        return await _dbContext.Templates
            .Where(t => t.Type == type)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Template> CreateAsync(Template template, CancellationToken cancellationToken)
    {
        await _dbContext.Templates.AddAsync(template, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return template;
    }
}

public class CacheTemplateRepository : ITemplateRepository
{
    private readonly ITemplateRepository _repository;
    private readonly IDistributedCache _cache;

    public CacheTemplateRepository(ITemplateRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<Template?> GetByIdAsync(int templateId, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Template(templateId);
        var cachedTemplate = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedTemplate != null)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Template>(cachedTemplate);
        }

        var template = await _repository.GetByIdAsync(templateId, cancellationToken);
        await _cache.SetStringWithSlidingAsync(cacheKey,
            System.Text.Json.JsonSerializer.Serialize(template), cancellationToken);
        return template;
    }

    public async Task<List<Template>> GetAllAsync(CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.TemplateAll();
        var cachedTemplates = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedTemplates != null)
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<Template>>(cachedTemplates) ?? [];
        }

        var templates = await _repository.GetAllAsync(cancellationToken);
        await _cache.SetStringWithSlidingAsync(cacheKey,
            System.Text.Json.JsonSerializer.Serialize(templates), cancellationToken);
        return templates;
    }

    public async Task<Template?> GetLatestByTypeAsync(ChannelType type, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.TemplateLatestByType(type);
        var cachedTemplate = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedTemplate != null)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Template>(cachedTemplate);
        }

        var template = await _repository.GetLatestByTypeAsync(type, cancellationToken);
        await _cache.SetStringWithSlidingAsync(cacheKey,
            System.Text.Json.JsonSerializer.Serialize(template), cancellationToken);
        return template;
    }

    public async Task<Template> CreateAsync(Template template, CancellationToken cancellationToken)
    {
        var createdTemplate = await _repository.CreateAsync(template, cancellationToken);
        var cacheKeyById = CacheKeys.Template(createdTemplate.Id);

        await _cache.SetStringAsync(cacheKeyById,
            System.Text.Json.JsonSerializer.Serialize(createdTemplate), cancellationToken);

        return createdTemplate;
    }
}