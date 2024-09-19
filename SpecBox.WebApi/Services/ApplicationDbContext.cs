using Microsoft.EntityFrameworkCore;
using SpecBox.Domain;
using SpecBox.Domain.Lib;

namespace SpecBox.WebApi.Services;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, CurrentUserService currentUserService) : SpecBoxDbContext(options)
{
    public override int SaveChanges()
    {
        ApplyAuditInformation();
        return base.SaveChanges();
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
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
            if (entity is ICreatedAt && ((ICreatedAt)entity).CreatedAt == default)
            {
                ((ICreatedAt)entity).CreatedAt = DateTime.UtcNow;
            }
            if (entity is ICreatedBy && ((ICreatedBy)entity).CreatedBy == null)
            {
                ((ICreatedBy)entity).CreatedById = currentUserService.GetUserId();
            }
            if (entity is IUpdatedAt)
            {
                ((IUpdatedAt)entity).UpdatedAt = DateTime.UtcNow;
            }
            if (entity is IUpdatedBy)
            {
                ((IUpdatedBy)entity).UpdatedById = currentUserService.GetUserId();
            }
        }
        var updatedEntries = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
        foreach (var entry in updatedEntries)
        {
            var entity = entry.Entity;
            if (entity is IDeletedAt && ((IDeletedAt)entity).DeletedAt.HasValue)
            {
                continue;
            }

            if (entity is IUpdatedAt)
            {
                ((IUpdatedAt)entity).UpdatedAt = DateTime.UtcNow;
            }
            if (entity is IUpdatedBy)
            {
                ((IUpdatedBy)entity).UpdatedById = currentUserService.GetUserId();
            }
        }
    }
}