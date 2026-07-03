using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PTTracker.Domain.Entities;

namespace PTTracker.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.SenderType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(m => m.Body).IsRequired();
        builder.Property(m => m.MessageIdHeader).HasMaxLength(998);
        builder.Property(m => m.InReplyTo).HasMaxLength(998);
        builder.Property(m => m.ContentHash).HasMaxLength(64);

        // Nullable unique: Postgres treats NULLs as distinct, so manual messages (no header) are unconstrained.
        builder.HasIndex(m => m.MessageIdHeader).IsUnique();
        builder.HasIndex(m => m.ContentHash);

        builder.HasOne(m => m.Ticket)
            .WithMany(t => t.Messages)
            .HasForeignKey(m => m.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.AuthorUser)
            .WithMany()
            .HasForeignKey(m => m.AuthorUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(m => new { m.TicketId, m.CreatedAt });

        builder.UseXminConcurrencyToken();
    }
}
