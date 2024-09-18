using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SpecBox.Domain;
using SpecBox.Domain.Lib;
using SpecBox.WebApi.Services;

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
            if (!userId.HasValue)
            {
                throw new Exception("User GUID not found in claims.");
            }
            ((IDeletedBy)entity).DeletedById = userId.Value;
        }
        entity.DeletedAt = DateTime.UtcNow;
    }

    public void ApplyAuditInformation()
    {
        var createdEntries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added);
        var userId = currentUserService.GetUserId();
        foreach (var entry in createdEntries)
        {
            var entity = entry.Entity;
            if (entity is ICreatedAt && ((ICreatedAt)entity).CreatedAt == default)
            {
                ((ICreatedAt)entity).CreatedAt = DateTime.UtcNow;
            }
            if (entity is ICreatedBy && ((ICreatedBy)entity).CreatedBy == null)
            {
                if (!userId.HasValue)
                {
                    throw new Exception("User GUID not found in claims.");
                }
                ((ICreatedBy)entity).CreatedById = userId.Value;
            }
            if (entity is IUpdatedAt)
            {
                ((IUpdatedAt)entity).UpdatedAt = DateTime.UtcNow;
            }
            if (entity is IUpdatedBy)
            {
                if (!userId.HasValue)
                {
                    throw new Exception("User GUID not found in claims.");
                }
                ((IUpdatedBy)entity).UpdatedById = userId.Value;
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
                if (!userId.HasValue)
                {
                    throw new Exception("User GUID not found in claims.");
                }
                ((IUpdatedBy)entity).UpdatedById = userId.Value;
            }
        }
    }
}