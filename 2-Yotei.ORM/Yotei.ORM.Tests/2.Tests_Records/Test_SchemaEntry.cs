using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Entry = Yotei.ORM.Records.Code.SchemaEntry;
using Item = Yotei.ORM.Records.Code.MetadataItem;

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
    public static void Test_Create_Empty_WithKnowns()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine);

        Assert.Equal(3, entry.Count);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values_NoKnowns()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new Entry(engine, "column", isReadOnly: true, range: [new Item("Age", 50)]);

        Assert.Equal(1, entry.Count);
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values_WithKnowns()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine, "column", isReadOnly: true);

        Assert.Equal(4, entry.Count);
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);

        entry = new Entry(engine, "column", isUniqueValued: true);

        Assert.Equal(4, entry.Count);
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);

        entry = new Entry(engine, "[table].", range: [new Item("Age", 50)]);

        Assert.Equal(6, entry.Count);
        Assert.Equal("[table].", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));

        entry = new Entry(engine, "[schema]..", isPrimaryKey: true, range: [new Item("Age", 50)]);

        Assert.Equal(7, entry.Count);
        Assert.Equal("[schema]..", entry.Identifier.Value);
        Assert.True(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));

        try { _ = new Entry(engine, "one.two.three.four"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata_NoKnowns()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new Entry(engine, [new Item("Age", 50)]);

        Assert.Equal(1, entry.Count);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));

        entry = new Entry(engine, "table.", range: [new Item("Age", 50)]);

        Assert.Equal(1, entry.Count);
        Assert.Equal("[table].", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata_WithKnowns()
    {
        var engine = new FakeEngine();
        var entry = new Entry(engine, [new Item("Age", 50)]);

        Assert.Equal(4, entry.Count);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));

        entry = new Entry(engine, [
            new Item("TableTag2", "table"),
            new Item("PrimaryTag3", true),
            new Item("Age", 50)]);

        Assert.Equal(6, entry.Count);
        Assert.Equal("[table].", entry.Identifier.Value);
        Assert.True(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.Contains("Age", 50));
    }

    // ----------------------------------------------------
}