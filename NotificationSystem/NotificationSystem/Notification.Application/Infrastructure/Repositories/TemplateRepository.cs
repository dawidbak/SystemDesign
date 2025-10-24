using Microsoft.EntityFrameworkCore;
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