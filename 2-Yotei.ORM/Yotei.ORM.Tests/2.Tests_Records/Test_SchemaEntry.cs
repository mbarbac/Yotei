using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Entry = Yotei.ORM.Records.Code.SchemaEntry;
using Pair = Yotei.ORM.Records.Code.MetadataEntry;

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
    public static void Test_Create_Empty_NoKnowns()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new Entry(engine);

        Assert.Equal(0, entry.Count);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine);

        Assert.Equal(3, entry.Count);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values_NoKnowns()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new Entry(engine, "column", isReadonly: true, range: [new Pair("Age", 50)]);

        Assert.Equal(1, entry.Count);
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine, "column", isReadonly: true);

        Assert.Equal(4, entry.Count);
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);

        entry = new Entry(engine, "..column", isUniqueValued: true);

        Assert.Equal(4, entry.Count);
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);

        entry = new Entry(engine, "[schema]..", isPrimaryKey: true, range: [new Pair("Age", 50)]);

        Assert.Equal(7, entry.Count);
        Assert.Equal("[schema]..", entry.Identifier.Value);
        Assert.True(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));

        try { _ = new Entry(engine, "one.two.three.four"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine, [
            new Pair("SchemaTag", "[schema]"),
            new Pair("TableTag", "table"),
            new Pair("ReadOnlyTag", true),
            new Pair("Age", 50)]);

        Assert.Equal(7, entry.Count);
        Assert.Equal("[schema].[table].", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));
    }
}