using Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Infrastructure.Persistence;

public class UrlShortenerDbContext : DbContext
{
    public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<UrlMapping> UrlMappings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UrlMapping>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ShortUrl).IsRequired().HasMaxLength(11);
            entity.Property(e => e.OriginalUrl).IsRequired().HasMaxLength(2000);
            entity.HasIndex(e => e.ShortUrl).IsUnique();
            entity.HasIndex(e => e.OriginalUrl).IsUnique();
        });
    }
}