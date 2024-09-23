using Microsoft.EntityFrameworkCore;
using SpecBox.Domain.Model.Users;

namespace SpecBox.Domain.Lib;

public static class AuditableExtensions
{
    public static void ApplyAuditableEntity(this ModelBuilder builder)
    {
        ApplyCreatedBy(builder);
        ApplyUpdatedBy(builder);
        ApplyDeletedBy(builder);
    }

    public static void ApplyCreatedBy(this ModelBuilder builder)
    {
        var entities = builder.Model.GetEntityTypes()
            .Where(e => typeof(ICreatedBy).IsAssignableFrom(e.ClrType))
            .ToList();

        foreach (var entityType in entities)
        {
            builder.Entity(entityType.ClrType)
                .HasOne(typeof(User), nameof(ICreatedBy.CreatedBy))
                .WithMany()
                .HasForeignKey(nameof(ICreatedBy.CreatedById));
        }
    }

    public static void ApplyUpdatedBy(this ModelBuilder builder)
    {
        var entities = builder.Model.GetEntityTypes()
            .Where(e => typeof(IUpdatedBy).IsAssignableFrom(e.ClrType))
            .ToList();

        foreach (var entityType in entities)
        {
            builder.Entity(entityType.ClrType)
                .HasOne(typeof(User), nameof(IUpdatedBy.UpdatedBy))
                .WithMany()
                .HasForeignKey(nameof(IUpdatedBy.UpdatedById));
        }
    }

    public static void ApplyDeletedBy(this ModelBuilder builder)
    {
        var entities = builder.Model.GetEntityTypes()
             .Where(e => typeof(IDeletedBy).IsAssignableFrom(e.ClrType))
             .ToList();

        foreach (var entityType in entities)
        {
            builder.Entity(entityType.ClrType)
                .HasOne(typeof(User), nameof(IDeletedBy.DeletedBy))
                .WithMany()
                .HasForeignKey(nameof(IDeletedBy.DeletedById));
        }
    }
}
