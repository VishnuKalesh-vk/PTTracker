using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PTTracker.Domain.Entities;

namespace PTTracker.Infrastructure.Persistence.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Body).IsRequired();

        builder.HasOne(n => n.Ticket)
            .WithMany(t => t.Notes)
            .HasForeignKey(n => n.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.AuthorUser)
            .WithMany(u => u.Notes)
            .HasForeignKey(n => n.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(n => n.TicketId);

        builder.UseXminConcurrencyToken();
    }
}
