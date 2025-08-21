using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Entry = Yotei.ORM.Records.Code.MetadataEntry;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_SchemaEntry
{
    /// <summary>
    /// Validates that the given collection contains a pair with the given name and value.
    /// </summary>
    static bool Contains(this ISchemaEntry entry, string name, object? value)
    {
        var item = entry.Find(name);
        return item is not null && item.Value.EqualsEx(value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine);

        Assert.Null(source.Identifier.Value);
        Assert.False(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.False(source.IsReadOnly);
        Assert.Equal(3, source.Count);
        Assert.Equal(0, source.RawCount);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Special()
    {
        var engine = new Engine();
        var source = new SchemaEntry(engine);

        Assert.Null(source.Identifier.Value);
        Assert.False(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.False(source.IsReadOnly);
        Assert.Equal(0, source.Count);
        Assert.Equal(0, source.RawCount);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //    var engine = new FakeEngine();
    //    var source = new SchemaEntry(
    //        engine, "schema..column", isPrimaryKey: true, range: [new Entry("Age", 50)]);
    //}
}