using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PTTracker.Domain.Entities;
using PTTracker.Domain.Enums;

namespace PTTracker.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.TicketNumber)
            .HasDefaultValueSql("nextval('ticket_number_seq')")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Subject).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(t => t.Priority).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(t => t.Source)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(SourceProvider.Manual)
            .IsRequired();
        builder.Property(t => t.ThreadKey).HasMaxLength(200);
        builder.Property(t => t.JiraId).HasMaxLength(50);

        builder.HasOne(t => t.Customer)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.AssignedUser)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(t => t.TicketNumber).IsUnique();
        builder.HasIndex(t => t.CustomerId);
        builder.HasIndex(t => t.AssignedUserId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => new { t.Status, t.Priority });
        builder.HasIndex(t => t.CreatedAt);
        builder.HasIndex(t => new { t.CustomerId, t.ThreadKey });

        builder.UseXminConcurrencyToken();
    }
}
