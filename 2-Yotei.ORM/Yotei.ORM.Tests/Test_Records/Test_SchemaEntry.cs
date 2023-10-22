using TItems = System.Collections.Generic.Dictionary<string, object?>;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_SchemaEntry
{
    static object? GetValue(this SchemaEntry entry, string tag, out bool error)
    {
        if (entry.TryGetValue(tag, out var value))
        {
            error = false;
            return value;
        }
        error = true;
        return null;
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
        Assert.Equal(0, entry.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Identifier()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine, "one");
        Assert.Equal(1, entry.Count);
        Assert.Equal("[one]", entry.Identifier.Value);
        Assert.Equal("one", entry.GetValue(engine.KnownTags.IdentifierTags[^1], out _));

        entry = new SchemaEntry(engine, "..one");
        Assert.Equal(1, entry.Count);
        Assert.Equal("[one]", entry.Identifier.Value);
        Assert.Equal("one", entry.GetValue(engine.KnownTags.IdentifierTags[^1], out _));

        entry = new SchemaEntry(engine, "two.one");
        Assert.Equal(2, entry.Count);
        Assert.Equal("[two].[one]", entry.Identifier.Value);
        Assert.Equal("two", entry.GetValue(engine.KnownTags.IdentifierTags[^2], out _));
        Assert.Equal("one", entry.GetValue(engine.KnownTags.IdentifierTags[^1], out _));

        entry = new SchemaEntry(engine, "three.two.one");
        Assert.Equal(3, entry.Count);
        Assert.Equal("[three].[two].[one]", entry.Identifier.Value);
        Assert.Equal("three", entry.GetValue(engine.KnownTags.IdentifierTags[^3], out _));
        Assert.Equal("two", entry.GetValue(engine.KnownTags.IdentifierTags[^2], out _));
        Assert.Equal("one", entry.GetValue(engine.KnownTags.IdentifierTags[^1], out _));

        entry = new SchemaEntry(engine, "a.b.c.d");
        Assert.Equal(3, entry.Count);
        Assert.Equal("[a].[b].[c].[d]", entry.Identifier.Value);
        Assert.Equal("b", entry.GetValue(engine.KnownTags.IdentifierTags[^3], out _));
        Assert.Equal("c", entry.GetValue(engine.KnownTags.IdentifierTags[^2], out _));
        Assert.Equal("d", entry.GetValue(engine.KnownTags.IdentifierTags[^1], out _));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Metadata()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine, "one", isPrimaryKey: true);
        Assert.Equal(2, entry.Count);
        Assert.Equal("[one]", entry.Identifier.Value);
        Assert.Equal("one", entry.GetValue(engine.KnownTags.IdentifierTags[^1], out _));
        Assert.True(entry.IsPrimaryKey);
        Assert.Equal(true, entry.GetValue(engine.KnownTags.PrimaryKeyTag!, out _));

        var meta = new TItems() {
            { engine.KnownTags.IdentifierTags[^1], "two" },
            { engine.KnownTags.UniqueValuedTag!, true },
        };
        entry = new SchemaEntry(engine, "one", metadata: meta);
        Assert.Equal(2, entry.Count);
        Assert.Equal("[two]", entry.Identifier.Value);
        Assert.Equal("two", entry.GetValue(engine.KnownTags.IdentifierTags[^1], out _));
        Assert.True(entry.IsUniqueValued);
        Assert.Equal(true, entry.GetValue(engine.KnownTags.UniqueValuedTag!, out _));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "one", isPrimaryKey: true);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Equal(source.Identifier.Value, target.Identifier.Value);
        Assert.Equal(source.IsPrimaryKey, target.IsPrimaryKey);
        Assert.Equal(source.IsUniqueValued, target.IsUniqueValued);
        Assert.Equal(source.IsReadOnly, target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "one", isPrimaryKey: true);
        Assert.Equal("[one]", source.Identifier.Value);
        Assert.True(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.False(source.IsReadOnly);

        var target = source.WithIdentifier(Identifier.Create(engine, "one.two"));
        Assert.NotSame(source, target);
        Assert.Equal("[one].[two]", target.Identifier.Value);
        Assert.True(target.IsPrimaryKey);

        target = source.WithIsPrimaryKey(false);
        Assert.NotSame(source, target);
        Assert.Equal(source.Identifier.Value, target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);

        target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.Equal(source.Identifier.Value, target.Identifier.Value);
        Assert.True(target.IsUniqueValued);

        target = source.WithIsReadOnly(true);
        Assert.NotSame(source, target);
        Assert.Equal(source.Identifier.Value, target.Identifier.Value);
        Assert.True(target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "one.two", isPrimaryKey: true);
        Assert.Equal("[one].[two]", source.Identifier.Value);
        Assert.True(source.IsPrimaryKey);
        Assert.False(source.IsUniqueValued);
        Assert.False(source.IsReadOnly);

        var target = source.Remove(engine.KnownTags.IdentifierTags[^2]);
        Assert.Equal("[two]", target.Identifier.Value);

        try { _ = source.Remove(engine.KnownTags.IdentifierTags[^1]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "one.two", isPrimaryKey: true);

        var target = source.RemoveRange([
            engine.KnownTags.IdentifierTags[^2],
            engine.KnownTags.PrimaryKeyTag!,
        ]);
        Assert.Equal("[two]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "one.two", isPrimaryKey: true);
        var target = source.Clear();

        Assert.Null(target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
    }
}