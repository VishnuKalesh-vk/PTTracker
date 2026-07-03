using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PTTracker.Domain.Entities;

namespace PTTracker.Infrastructure.Persistence.Configurations;

public class StatusHistoryConfiguration : IEntityTypeConfiguration<StatusHistory>
{
    public void Configure(EntityTypeBuilder<StatusHistory> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.FromStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.ToStatus).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(s => s.Reason).HasMaxLength(500);

        builder.HasOne(s => s.Ticket)
            .WithMany(t => t.StatusHistory)
            .HasForeignKey(s => s.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.ChangedByUser)
            .WithMany()
            .HasForeignKey(s => s.ChangedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => new { s.TicketId, s.ChangedAt });
    }
}
