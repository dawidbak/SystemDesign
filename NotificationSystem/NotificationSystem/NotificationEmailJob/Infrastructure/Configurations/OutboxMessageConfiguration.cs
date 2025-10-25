using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationJob.Outbox;

namespace NotificationJob.Infrastructure.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .IsRequired();
        builder.Property(x => x.Content)
            .HasColumnType("JSONB")
            .IsRequired();
        builder.Property(x => x.OccurredOnUtc)
            .IsRequired();

        builder.Property(x => x.ProcessedOnUtc)
            .IsRequired(false);

        builder.Property(x => x.Error)
            .IsRequired(false);

        // Index for unprocessed messages
        builder.HasIndex(x => new { x.OccurredOnUtc, x.ProcessedOnUtc })
            .HasDatabaseName("idx_outbox_messages_unprocessed")
            .IncludeProperties(x => new { x.Id, x.Content })
            .HasFilter("\"ProcessedOnUtc\" IS NULL");
    }
}

