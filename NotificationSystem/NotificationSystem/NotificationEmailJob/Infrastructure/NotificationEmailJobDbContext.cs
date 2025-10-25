using Microsoft.EntityFrameworkCore;
using NotificationJob.Inbox;
using NotificationJob.Outbox;

namespace NotificationJob.Infrastructure;

public class NotificationEmailJobDbContext : DbContext
{
    public NotificationEmailJobDbContext(DbContextOptions<NotificationEmailJobDbContext> options)
        : base(options)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationEmailJobDbContext).Assembly);

    }
}