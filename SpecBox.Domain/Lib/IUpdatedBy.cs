using SpecBox.Domain.Model.Users;

namespace SpecBox.Domain.Lib;

public interface IUpdatedBy
{
    public Guid UpdatedById { get; set; }
    public User UpdatedBy { get; set; }
}
