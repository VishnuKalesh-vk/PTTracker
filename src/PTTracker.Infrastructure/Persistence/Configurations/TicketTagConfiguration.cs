using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PTTracker.Domain.Entities;

namespace PTTracker.Infrastructure.Persistence.Configurations;

public class TicketTagConfiguration : IEntityTypeConfiguration<TicketTag>
{
    public void Configure(EntityTypeBuilder<TicketTag> builder)
    {
        builder.HasKey(tt => new { tt.TicketId, tt.TagId });

        builder.HasOne(tt => tt.Ticket)
            .WithMany(t => t.TicketTags)
            .HasForeignKey(tt => tt.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tt => tt.Tag)
            .WithMany(t => t.TicketTags)
            .HasForeignKey(tt => tt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Reverse lookup: all tickets carrying a given tag.
        builder.HasIndex(tt => tt.TagId);
    }
}
