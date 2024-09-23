using Microsoft.EntityFrameworkCore;
using SpecBox.Domain;
using SpecBox.Domain.Lib;

namespace SpecBox.WebApi.Services;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, CurrentUserService currentUserService) : SpecBoxDbContext(options)
{
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyAuditInformation();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public void SoftDelete<TEntity>(TEntity entity) where TEntity : IDeletedAt
    {
        if (entity.DeletedAt.HasValue)
        {
            return;
        }
        if (entity is IDeletedBy)
        {
            var userId = currentUserService.GetUserId();
            ((IDeletedBy)entity).DeletedById = userId;
        }
        entity.DeletedAt = DateTime.UtcNow;
    }

    public void ApplyAuditInformation()
    {
        var createdEntries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added);
        foreach (var entry in createdEntries)
        {
            var entity = entry.Entity;
            if (entity is ICreatedAt createdAtEntity && createdAtEntity.CreatedAt == default)
            {
                createdAtEntity.CreatedAt = DateTime.UtcNow;
            }
            if (entity is ICreatedBy  createdByEntity && createdByEntity.CreatedBy == null)
            {
                createdByEntity.CreatedById = currentUserService.GetUserId();
            }
            if (entity is IUpdatedAt updatedAtEntity)
            {
                updatedAtEntity.UpdatedAt = DateTime.UtcNow;
            }
            if (entity is IUpdatedBy updatedByEntity)
            {
                updatedByEntity.UpdatedById = currentUserService.GetUserId();
            }
        }
        var updatedEntries = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
        foreach (var entry in updatedEntries)
        {
            var entity = entry.Entity;
            
            if (entity is IUpdatedAt updatedAtEntity)
            {
                updatedAtEntity.UpdatedAt = DateTime.UtcNow;
            }
            if (entity is IUpdatedBy updatedByEntity)
            {
                updatedByEntity.UpdatedById = currentUserService.GetUserId();
            }
        }
    }
}
