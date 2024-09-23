using SpecBox.Domain.Lib;

namespace SpecBox.WebApi.Test.Lib;

internal class FakeEntity
{
    public required string Id { get; set; }
    public required string Foo { get; set; }
    public required string Boo { get; set; }
    public required string Bar { get; set; }
}

internal class FakeConcurrencyControlEntity : IConcurrencyControl
{
    public required string Foo { get; set; }
    public Guid RowVersion { get; set; }
}

public class EntityUpdaterTests
{
    [Fact]
    public void ApplyChanges_SimpleModel_UpdatesModel()
    {
        var value = new
        {
            Foo = "John",
            Boo = "Doe"
        };
        var shouldNotChange = "ShouldNotChange";
        var result = new FakeEntity
        {
            Id = shouldNotChange,
            Foo = "ThisShouldChange",
            Boo = "ThisShouldChangeToo",
            Bar = shouldNotChange
        };

        Assert.True(EntityUpdater.ApplyChanges(result, value));

        Assert.Equal(shouldNotChange, result.Id);
        Assert.Equal(value.Foo, result.Foo);
        Assert.Equal(value.Boo, result.Boo);
        Assert.Equal(shouldNotChange, result.Bar);
    }

    [Fact]
    public void ApplyChanges_NoCHanges_ReturnsFalse()
    {
        var value = new
        {
            Foo = "John",
            Boo = "Doe"
        };
        var result = new FakeEntity
        {
            Id = "id",
            Foo = value.Foo,
            Boo = value.Boo,
            Bar = "bar"
        };

        Assert.False(EntityUpdater.ApplyChanges(result, value));
    }

    [Fact]
    public void ApplyChanges_ConcurrencyControl_NoChangesNoEffectOnRowVersion()
    {
        var value = new FakeConcurrencyControlEntity
        {
            Foo = "John",
            RowVersion = Guid.NewGuid()
        };
        var originalGuid = Guid.NewGuid();
        var result = new FakeConcurrencyControlEntity
        {
            Foo = value.Foo,
            RowVersion = originalGuid
        };

        Assert.False(EntityUpdater.ApplyChanges(result, value));
        Assert.Equal(originalGuid, result.RowVersion);
    }

    [Fact]
    public void ApplyChanges_ConcurrencyControl_HasChanges_CopiesRowVersion()
    {
        var newGuid = Guid.NewGuid();
        var value = new FakeConcurrencyControlEntity
        {
            Foo = "John",
            RowVersion = newGuid
        };
        var result = new FakeConcurrencyControlEntity
        {
            Foo = "Jane",
            RowVersion = Guid.NewGuid()
        };

        Assert.True(EntityUpdater.ApplyChanges(result, value));
        Assert.Equal(newGuid, result.RowVersion);
    }
}
