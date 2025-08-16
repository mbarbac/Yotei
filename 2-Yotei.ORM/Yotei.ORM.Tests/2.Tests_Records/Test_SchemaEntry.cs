using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Pair = Yotei.ORM.Records.Code.MetadataEntry;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_SchemaEntry
{
    /// <summary>
    /// Validates that the given collection contains a pair with the given name and value.
    /// </summary>
    static bool Contains(this SchemaEntry entry, string name, object? value)
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
        var entry = new SchemaEntry(engine);

        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(3, entry.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine, "column");
        Assert.Equal("[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(4, entry.Count);

        entry = new SchemaEntry(engine, "table.column", isReadOnly: true);
        Assert.Equal("[table].[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.Equal(5, entry.Count);

        entry = new SchemaEntry(engine, "schema..column", isReadOnly: true, range: [new Pair("Age", 50)]);
        Assert.Equal("[schema]..[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.Equal(7, entry.Count);
        Assert.NotNull(entry.Find("Age"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine, [
            new Pair("TableTag", "table"),
            new Pair("ColumnTag", "column"),
            new Pair("ReadOnlyTag", true),
            new Pair("Age", 50),]);
        Assert.Equal("[table].[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);
        Assert.Equal(6, entry.Count);
        Assert.NotNull(entry.Find("Age"));

        entry = new SchemaEntry(engine, [
            new Pair("SchemaTag", "schema"),
            new Pair("ColumnTag", "column"),
            new Pair("UniqueTag", true),
            new Pair("Age", 50),]);

        var id = entry.Identifier;
        Assert.Equal("[schema]..[column]", id.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(7, entry.Count);
        Assert.NotNull(entry.Find("Age"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "schema..column", isPrimaryKey: true, range: [new Pair("Age", 50)]);
        var target = source.Clone();
        
        Assert.NotSame(source, target);        
        Assert.Equal("[schema]..[column]", target.Identifier.Value);
        Assert.True(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.Equal(7, target.Count);
        Assert.NotNull(target.Find("Age"));
    }
}