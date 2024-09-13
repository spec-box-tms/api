using SpecBox.Domain.Model.Users;

namespace SpecBox.Domain.Lib;

public interface ICreatedBy
{
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; }
}
