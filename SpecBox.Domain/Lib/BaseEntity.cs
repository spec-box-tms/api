
using SpecBox.Domain.Model.Users;

namespace SpecBox.Domain.Lib;

public abstract class BaseEntity : ICreatedAt, ICreatedBy, IUpdatedAt, IUpdatedBy, IDeletedAt, IDeletedBy
{
    [IsUtc]
    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
    [IsUtc]
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedById { get; set; }
    public User UpdatedBy { get; set; } = null!;
    [IsUtc]
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedById { get; set; }
    public User DeletedBy { get; set; } = null!;
}
