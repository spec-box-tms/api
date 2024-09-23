namespace SpecBox.Domain.Lib;

public interface IConcurrencyControl
{
    public Guid RowVersion { get; set; }
}
