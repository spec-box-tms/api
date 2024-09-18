using System.ComponentModel.DataAnnotations.Schema;
using SpecBox.Domain.Model.Users;

namespace SpecBox.Domain.Lib;

public abstract class BaseEntity : ICreatedAt, ICreatedBy, IUpdatedAt, IUpdatedBy, IDeletedAt, IDeletedBy
{
    [IsUtc]
    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    [IsUtc]
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedById { get; set; }
    [IsUtc]
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedById { get; set; }
    
    [ForeignKey(nameof(CreatedById))]
    public User CreatedBy { get; set; } = null!;

    [ForeignKey(nameof(UpdatedById))]
    public User UpdatedBy { get; set; } = null!;

    [ForeignKey(nameof(DeletedById))]
    public User DeletedBy { get; set; } = null!;
}
