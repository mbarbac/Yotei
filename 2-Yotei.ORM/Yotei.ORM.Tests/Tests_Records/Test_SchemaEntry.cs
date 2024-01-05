using TPair = System.Collections.Generic.KeyValuePair<string, object?>;
using Identifier = Yotei.ORM.Code.Identifier;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_SchemaEntry
{
    [SuppressMessage("", "CA1859")]
    static bool Contains(this ISchemaEntry entry, string name, object? value)
    {
        if (!entry.TryGetValue(name, out var temp)) return false;

        return
            (value is null && temp is null) ||
            (value is not null && value.Equals(temp));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var entry = new FakeSchemaEntry(engine);

        Assert.Empty(entry);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var entry = new FakeSchemaEntry(
            engine, "one.two", isReadOnly: true, metadata: [new TPair("age", 50)]);

        Assert.Equal("[one].[two]", entry.Identifier.Value);
        Assert.True(entry.Contains("TableTag", "one"));
        Assert.True(entry.Contains("ColumnTag", "two"));
        Assert.True(entry.IsReadOnly);
        Assert.True(entry.Contains("ReadOnlyTag", true));
        Assert.True(entry.Contains("age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Metadata()
    {
        var engine = new FakeEngine();
        var entry = new FakeSchemaEntry(engine, [
            new TPair("TableTag", "one"),
            new TPair("ColumnTag", "[two]"),
            new TPair("PrimaryTag", true),
            new TPair("age", 50),
        ]);

        Assert.Equal("[one].[two]", entry.Identifier.Value);
        Assert.True(entry.Contains("TableTag", "one"));
        Assert.True(entry.Contains("ColumnTag", "two"));
        Assert.True(entry.IsPrimaryKey);
        Assert.True(entry.Contains("PrimaryTag", true));
        Assert.True(entry.Contains("age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var entry = new FakeSchemaEntry(
            engine, "one.two", isReadOnly: true, metadata: [new TPair("age", 50)]);

        Assert.True(entry.TryGetValue<string?>("ColumnTag", out var str));
        Assert.Equal("two", str);
        Assert.False(entry.Contains("SchemaTag"));

        try { entry.TryGetValue<int>("ColumnTag", out var ent); Assert.Fail(); }
        catch (InvalidCastException) { }

        var names = entry.Names.ToList();
        Assert.Equal(4, names.Count);
        Assert.Contains("TableTag", names);
        Assert.Contains("ColumnTag", names);
        Assert.Contains("ReadOnlyTag", names);
        Assert.Contains("age", names);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new FakeSchemaEntry(
            engine, "one.two", isReadOnly: true, metadata: [new TPair("age", 50)]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Equal("[one].[two]", target.Identifier.Value);
        Assert.True(target.Contains("TableTag", "one"));
        Assert.True(target.Contains("ColumnTag", "two"));
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("ReadOnlyTag", true));
        Assert.True(target.Contains("age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Identifier()
    {
        var engine = new FakeEngine();
        var source = new FakeSchemaEntry(engine, "one..three");

        var target = source.WithIdentifier(new Identifier(engine));
        Assert.NotSame(source, target);
        Assert.Null(target.Identifier.Value);
        Assert.False(target.Contains("SchemaTag"));
        Assert.False(target.Contains("TableTag"));
        Assert.False(target.Contains("ColumnTag"));
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Primary()
    {
        var engine = new FakeEngine();
        var source = new FakeSchemaEntry(engine, "one");

        var target = source.WithIsPrimaryKey(false);
        Assert.Same(source, target);

        target = source.WithIsPrimaryKey(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsPrimaryKey);
        Assert.True(target.Contains("PrimaryTag", true));
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Unique()
    {
        var engine = new FakeEngine();
        var source = new FakeSchemaEntry(engine, "one");

        var target = source.WithIsUniqueValued(false);
        Assert.Same(source, target);

        target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsUniqueValued);
        Assert.True(target.Contains("UniqueTag", true));
    }

    //[Enforced]
    [Fact]
    public static void Test_With_ReadOnly()
    {
        var engine = new FakeEngine();
        var source = new FakeSchemaEntry(engine, "one");

        var target = source.WithIsReadOnly(false);
        Assert.Same(source, target);

        target = source.WithIsReadOnly(true);
        Assert.NotSame(source, target);
        Assert.True(target.IsReadOnly);
        Assert.True(target.Contains("ReadOnlyTag", true));
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new FakeSchemaEntry(engine, "one", isPrimaryKey: true);

        var target = source.Replace("ColumnTag", "one");
        Assert.Same(source, target);

        target = source.Replace("ColumnTag", "other");
        Assert.NotSame(source, target);
        Assert.Equal("[other]", target.Identifier.Value);

        target = source.Replace("TableTag", "table");
        Assert.NotSame(source, target);
        Assert.Equal("[table].[one]", target.Identifier.Value);

        source = new FakeSchemaEntry(engine, "one.two", isPrimaryKey: true);
        target = source.Replace("ColumnTag", null);
        Assert.NotSame(source, target);
        Assert.Equal("[one].", target.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new FakeSchemaEntry(engine, "one", metadata: [new TPair("age", 50)]);

        var target = source.Add("ColumnTag", "one");
        Assert.Same(source, target);

        target = source.Add("ColumnTag", "two");
        Assert.NotSame(source, target);
        Assert.Equal("[two]", target.Identifier.Value);

        try { source.Add("age", 60); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add("TableTag", 0); Assert.Fail(); }
        catch (InvalidCastException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine();
        var source = new FakeSchemaEntry(
            engine, "one.two", isReadOnly: true, metadata: [new TPair("age", 50)]);

        var target = source.Remove("other");
        Assert.Same(source, target);

        target = source.Remove("TableTag");
        Assert.NotSame(source, target);
        Assert.Equal("[two]", target.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new FakeSchemaEntry(engine);

        var target = source.Clear();
        Assert.Same(source, target);

        source = new FakeSchemaEntry(engine, "one", metadata: [new TPair("age", 50)]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}