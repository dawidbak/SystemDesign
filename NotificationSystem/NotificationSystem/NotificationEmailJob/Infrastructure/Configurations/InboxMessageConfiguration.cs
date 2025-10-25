using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationJob.Inbox;

namespace NotificationJob.Infrastructure.Configurations;

public class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("InboxMessages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .IsRequired();
        builder.Property(x => x.OccurredOnUtc)
            .IsRequired();
        builder.Property(x => x.ProcessedOnUtc)
            .IsRequired(false);
    }
}

