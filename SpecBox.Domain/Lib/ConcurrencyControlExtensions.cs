using Microsoft.EntityFrameworkCore;

namespace SpecBox.Domain.Lib;

public static class ConcurrencyControlExtensions
{
    public static void ApplyConcurrencyControl(this ModelBuilder builder)
    {
        var concurrencyControlledEntities = builder.Model.GetEntityTypes()
            .Where(e => typeof(IConcurrencyControl).IsAssignableFrom(e.ClrType))
            .ToList();

        foreach (var entityType in concurrencyControlledEntities)
        {
            builder.Entity(entityType.ClrType)
                .Property(nameof(IConcurrencyControl.RowVersion))
                .IsConcurrencyToken();
        }
    }

    public static void UpdateConcurrencyControl(this DbContext db)
    {
        var entries = db.ChangeTracker.Entries().Where(e =>
            e.State == EntityState.Added ||
            e.State == EntityState.Modified ||
            e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            if (entity is IConcurrencyControl concurrencyEntity)
            {
                entry.Property(nameof(IConcurrencyControl.RowVersion)).OriginalValue = concurrencyEntity.RowVersion;
                concurrencyEntity.RowVersion = Guid.NewGuid();
            }
        }
    }
}
