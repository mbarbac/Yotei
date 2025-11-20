using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Entry = Yotei.ORM.Records.Code.SchemaEntry;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_SchemaEntry
{
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
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new Entry(engine);

        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(0, entry.Count);

        engine = new FakeEngine();
        entry = new Entry(engine);

        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(3, entry.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new Entry(engine, "column", isReadonly: true);
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.Equal(0, entry.Count);

        engine = new FakeEngine();
        entry = new Entry(engine, "column", isUniqueValued: true);
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(4, entry.Count);

        entry = new Entry(engine, "schema..", isPrimaryKey: true);
        Assert.Equal("[schema]..", entry.Identifier.Value);
        Assert.True(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(6, entry.Count);
    }
}