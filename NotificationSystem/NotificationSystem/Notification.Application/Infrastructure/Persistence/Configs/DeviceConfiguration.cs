using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Application.Domain;

namespace Notification.Application.Infrastructure.Persistence.Configs;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.DeviceToken).IsRequired().HasMaxLength(512);
        builder.Property(u => u.Type).IsRequired().HasConversion<string>();
        builder.Property(u => u.LastLoggedInAt);
        builder.Property(u => u.UserId).IsRequired();
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}