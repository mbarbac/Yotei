using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Pair = Yotei.ORM.Records.Code.MetadataEntry;

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

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine,
            "table.column",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        Assert.True(entry.Contains("TABLETAG", "table"));
        Assert.True(entry.Contains("COLUMNTAG", "column"));
        Assert.True(entry.Contains("READONLYTAG", true));
        Assert.True(entry.Contains("AGE", 50));
        Assert.True(entry.Contains("YEARS", 50));
        Assert.False(entry.Contains("OTHER"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine,
            "table.column",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.NotEmpty(target);
        Assert.Equal("[table].[column]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.True(target.IsReadOnly);

        Assert.True(target.Contains("TableTag", "table"));
        Assert.True(target.Contains("ColumnTag", "column"));
        Assert.True(target.Contains("ReadOnlyTag", true));
        Assert.True(target.Contains("age", 50));
        Assert.True(target.Contains("years", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine,
            "table.column",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        var target = source.WithIdentifier(Identifier.Create(engine, "one.two"));
        Assert.NotSame(source, target);
        Assert.Equal("[one].[two]", target.Identifier.Value);

        target = source.WithIsPrimaryKey(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsPrimaryKey);

        target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsUniqueValued);

        target = source.WithIsReadOnly(false);
        Assert.NotSame(source, target);
        Assert.False(target.IsReadOnly);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine,
            "table.column",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        var target = source.ReplaceValue("TableTag", "[other]");
        Assert.NotSame(source, target);
        Assert.Equal("[other].[column]", target.Identifier.Value);

        target = source.ReplaceValue("years", 60);
        Assert.NotSame(source, target);
        Assert.True(target.Contains("years", 60));
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_NotExisting_WellKnown()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine,
            "table.column",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        var target = source.ReplaceValue("SchemaTag", "[other]");
        Assert.NotSame(source, target);
        Assert.Equal("[other].[table].[column]", target.Identifier.Value);

        target = source.ReplaceValue("any", "other");
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceRange()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine,
            "table.column",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        var target = source.ReplaceValue(["any", "TABLETag"], "[other]");
        Assert.NotSame(source, target);
        Assert.Equal("[other].[column]", target.Identifier.Value);

        target = source.ReplaceValue(["any", "SchemaTag"], "[other]");
        Assert.NotSame(source, target);
        Assert.Equal("[other].[table].[column]", target.Identifier.Value);

        target = source.ReplaceValue(["any", "other"], "whatever");
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine,
            "table.column",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        var tag = new Tag(false, ["Other", "SchemaTag"]);
        var pair = new Pair(tag, "schema");
        var target = source.Add(pair);
        Assert.NotSame(source, tag);
        Assert.Equal("[schema].[table].[column]", target.Identifier.Value);

        try { source.Add(new Pair(new Tag(false, "readONLYtag"), false)); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(new Pair(new Tag(false, ["any", "tableTAG"]), "whatever")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(new Pair(new Tag(false, ["any", "SchemaTag"]), 0)); Assert.Fail(); }
        catch (InvalidCastException) { }

        try { source.Add(new Pair(new Tag(false, ["any", "primaryTAG"]), 0)); Assert.Fail(); }
        catch (InvalidCastException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_AddRange() { }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine,
            "table.column",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        var target = source.Remove("tableTAG");
        Assert.NotSame(source, target);
        Assert.Equal("[column]", target.Identifier.Value);

        target = source.Remove("years");
        Assert.False(target.Contains("age"));

        target = source.Remove("any");
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine,
            "table.column",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        var target = source.Remove(["any", "tableTAG"]);
        Assert.NotSame(source, target);
        Assert.Equal("[column]", target.Identifier.Value);

        target = source.Remove(["any", "years"]);
        Assert.False(target.Contains("age"));

        target = source.Remove(["any", "other"]);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine);

        var target = source.Clear();
        Assert.Same(source, target);

        source = new SchemaEntry(engine,
            "table.column",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        target = source.Clear();
        Assert.Empty(target);
        Assert.Null(target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.Equal(0, target.Count);
    }
}