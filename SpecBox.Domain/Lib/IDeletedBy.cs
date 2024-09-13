using SpecBox.Domain.Model.Users;

namespace SpecBox.Domain.Lib;

public interface IDeletedBy
{
    public Guid? DeletedById { get; set; }
    public User DeletedBy { get; set; }
}
