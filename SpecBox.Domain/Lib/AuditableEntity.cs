using SpecBox.Domain.Model.Users;

namespace SpecBox.Domain.Lib;

public abstract class AuditableEntity : ICreatedAt, ICreatedBy, IUpdatedAt, IUpdatedBy, IDeletedAt, IDeletedBy
{
    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedById { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedById { get; set; }
    
    public User CreatedBy { get; set; } = null!;

    public User UpdatedBy { get; set; } = null!;

    public User DeletedBy { get; set; } = null!;
}
