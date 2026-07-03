using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PTTracker.Infrastructure.Persistence;

internal static class ConcurrencyConfigurationExtensions
{
    /// <summary>
    /// Maps PostgreSQL's <c>xmin</c> system column as an optimistic-concurrency token.
    /// This is the non-obsolete equivalent of Npgsql's <c>UseXminAsConcurrencyToken()</c>:
    /// it configures the identical shadow property, so it requires no schema change.
    /// </summary>
    public static EntityTypeBuilder<T> UseXminConcurrencyToken<T>(this EntityTypeBuilder<T> builder)
        where T : class
    {
        builder.Property<uint>("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        return builder;
    }
}
