namespace Yotei.ORM.Core.Tests;

// ========================================================
[Enforced]
public static class Test_SchemaEntry
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine);
        Assert.Equal(0, entry.Count);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Identifier()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine, Identifier.Create(engine, "one"));
        Assert.Equal(1, entry.Count);
        Assert.Equal("[one]", entry.Identifier.Value);
        Assert.Equal("one", entry.GetValue(engine.KnownTags.IdentifierTags[^1], out _));

        entry = new SchemaEntry(engine, Identifier.Create(engine, "two.one"));
        Assert.Equal(2, entry.Count);
        Assert.Equal("[two].[one]", entry.Identifier.Value);
        Assert.Equal("two", entry.GetValue(engine.KnownTags.IdentifierTags[^2], out _));
        Assert.Equal("one", entry.GetValue(engine.KnownTags.IdentifierTags[^1], out _));

        entry = new SchemaEntry(engine, Identifier.Create(engine, "three.two.one"));
        Assert.Equal(3, entry.Count);
        Assert.Equal("[three].[two].[one]", entry.Identifier.Value);
        Assert.Equal("three", entry.GetValue(engine.KnownTags.IdentifierTags[^3], out _));
        Assert.Equal("two", entry.GetValue(engine.KnownTags.IdentifierTags[^2], out _));
        Assert.Equal("one", entry.GetValue(engine.KnownTags.IdentifierTags[^1], out _));

        try { _ = new SchemaEntry(engine, Identifier.Create(engine, "a.b.c.d")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Known_Tags()
    {
        var engine = new FakeEngine().WithKnownTags(new KnownTags(false));
        var entry = new SchemaEntry(engine, Identifier.Create(engine, "one"));
        Assert.Equal(0, entry.Count);
        Assert.Equal("[one]", entry.Identifier.Value);

        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);

        entry = entry.WithIsPrimaryKey(true); Assert.True(entry.IsPrimaryKey);
        entry = entry.WithIsUniqueValued(true); Assert.True(entry.IsUniqueValued);
        entry = entry.WithIsReadOnly(true); Assert.True(entry.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Identifier()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, Identifier.Create(engine, "three.two.one"));
        var target = source.WithIdentifier(Identifier.Create(engine, "one"));
        Assert.Equal(1, target.Count);
        Assert.Equal("[one]", target.Identifier.Value);
        Assert.Equal("one", target.GetValue(engine.KnownTags.IdentifierTags[^1], out _));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^3]));
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Or_Add()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, Identifier.Create(engine, "three.two.one"));

        var target = source.ReplaceOrAdd(engine.KnownTags.PrimaryKeyTag!, true);
        Assert.True(target.IsPrimaryKey);

        target = source.ReplaceOrAdd(engine.KnownTags.IdentifierTags[^1], "x");
        Assert.Equal("[three].[two].[x]", target.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, Identifier.Create(engine, "three.two.one"));
        var target = source.Remove(engine.KnownTags.IdentifierTags[^2]);
        Assert.Equal("[three]..[one]", target.Identifier.Value);
    }
}