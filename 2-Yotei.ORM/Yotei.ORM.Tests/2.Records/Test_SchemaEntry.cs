using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using Pair = Yotei.ORM.Records.Code.MetadataEntry;
using Tag = Yotei.ORM.Records.Code.MetadataTag;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_SchemaEntry
{
    static bool Contains(this ISchemaEntry entry, string name, object? value)
    {
        return entry.TryGet(name, out var item) && item.Value.EqualsEx(value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine);

        Assert.Empty(entry);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(0, entry.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine,
            "table.column",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        Assert.NotEmpty(entry);
        Assert.Equal("[table].[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);

        Assert.True(entry.Contains("TableTag", "table"));
        Assert.True(entry.Contains("ColumnTag", "column"));
        Assert.True(entry.Contains("ReadOnlyTag", true));
        Assert.True(entry.Contains("age", 50));
        Assert.True(entry.Contains("years", 50));

        Assert.False(entry.Contains("SchemaTag"));
        Assert.False(entry.Contains("PrimaryTag"));
        Assert.False(entry.Contains("UniqueTag"));
        Assert.False(entry.Contains("SchemaTag"));

        entry = new SchemaEntry(engine, "schema..column");
        Assert.NotEmpty(entry);
        Assert.Equal("[schema]..[column]", entry.Identifier.Value);
        Assert.True(entry.Contains("SchemaTag", "schema"));
        Assert.False(entry.Contains("TableTag"));
        Assert.True(entry.Contains("ColumnTag", "column"));

        entry = new SchemaEntry(engine, "schema.table.");
        Assert.NotEmpty(entry);
        Assert.Equal("[schema].[table].", entry.Identifier.Value);
        Assert.True(entry.Contains("SchemaTag", "schema"));
        Assert.True(entry.Contains("TableTag", "table"));
        Assert.False(entry.Contains("ColumnTag"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine, [
            new Pair(new Tag(false, "TableTag"), "table"),
            new Pair(new Tag(false, "ColumnTag"), "[column]"),
            new Pair(new Tag(false, "ReadOnlyTag"), true),
            new Pair(new Tag(false, ["age", "years"]), 50)]);

        Assert.NotEmpty(entry);
        Assert.Equal("[table].[column]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);

        Assert.True(entry.Contains("TableTag", "table"));
        Assert.True(entry.Contains("ColumnTag", "column"));
        Assert.True(entry.Contains("ReadOnlyTag", true));
        Assert.True(entry.Contains("age", 50));
        Assert.True(entry.Contains("years", 50));

        Assert.False(entry.Contains("SchemaTag"));
        Assert.False(entry.Contains("PrimaryTag"));
        Assert.False(entry.Contains("UniqueTag"));
        Assert.False(entry.Contains("SchemaTag"));
    }

    //[Enforced]
    //[Fact]
    //public static void Test_()
    //{
    //}
}