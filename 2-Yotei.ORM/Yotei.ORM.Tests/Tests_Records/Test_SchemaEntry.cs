using TPair = System.Collections.Generic.KeyValuePair<string, object?>;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_SchemaEntry
{
    static object? GetValue(this ISchemaEntry entry, string tag, out bool error)
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

        Assert.Equal(4, entry.Count);
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.True(entry.Contains(engine.KnownTags.PrimaryKeyTag!));
        Assert.True(entry.Contains(engine.KnownTags.UniqueValuedTag!));
        Assert.True(entry.Contains(engine.KnownTags.ReadOnlyTag!));

        Assert.False(entry.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.False(entry.Contains(engine.KnownTags.IdentifierTags[^3]));

        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new SchemaEntry(engine);

        Assert.Equal(0, entry.Count);
        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Metadata()
    {
        var engine = new FakeEngine();
        var metadata = new TPair[] {
            new(engine.KnownTags.IdentifierTags[^3], "one"),
            new(engine.KnownTags.IdentifierTags[^1], "three"),
            new(engine.KnownTags.PrimaryKeyTag!, true),
            new("age", 50),
        };

        var entry = new SchemaEntry(engine, metadata);
        Assert.Equal(7, entry.Count);
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[^3]));
        Assert.True(entry.Contains(engine.KnownTags.PrimaryKeyTag!));
        Assert.True(entry.Contains(engine.KnownTags.UniqueValuedTag!));
        Assert.True(entry.Contains(engine.KnownTags.ReadOnlyTag!));
        Assert.True(entry.Contains("age"));

        Assert.Equal("[one]..[three]", entry.Identifier.Value);
        Assert.True(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(50, (int)entry.GetValue("AGE", out _)!);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Metadata_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var metadata = new TPair[] {
            new("age", 50),
        };

        var entry = new SchemaEntry(engine, metadata);
        Assert.Equal(1, entry.Count);
        Assert.True(entry.Contains("age"));

        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(50, (int)entry.GetValue("AGE", out _)!);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var entry = new SchemaEntry(engine, "one..three", isPrimaryKey: true, metadata: [pair]);

        Assert.Equal(7, entry.Count);
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[^3]));
        Assert.True(entry.Contains(engine.KnownTags.PrimaryKeyTag!));
        Assert.True(entry.Contains(engine.KnownTags.UniqueValuedTag!));
        Assert.True(entry.Contains(engine.KnownTags.ReadOnlyTag!));
        Assert.True(entry.Contains("age"));

        Assert.Equal("[one]..[three]", entry.Identifier.Value);
        Assert.True(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(50, (int)entry.GetValue("AGE", out _)!);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var entry = new SchemaEntry(engine, "one..three", isPrimaryKey: true, metadata: [pair]);

        Assert.Equal(1, entry.Count);
        Assert.True(entry.Contains("age"));

        Assert.Equal("[one]..[three]", entry.Identifier.Value);
        Assert.True(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(50, (int)entry.GetValue("AGE", out _)!);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isPrimaryKey: true, metadata: [pair]);
        Assert.Equal(7, source.Count);

        var target = source.Clone();
        Assert.NotSame(source, target);

        Assert.Equal(7, target.Count);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^3]));
        Assert.True(target.Contains(engine.KnownTags.PrimaryKeyTag!));
        Assert.True(target.Contains(engine.KnownTags.UniqueValuedTag!));
        Assert.True(target.Contains(engine.KnownTags.ReadOnlyTag!));
        Assert.True(target.Contains("age"));

        Assert.Equal("[one]..[three]", target.Identifier.Value);
        Assert.True(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.Equal(50, (int)target.GetValue("AGE", out _)!);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isPrimaryKey: true, metadata: [pair]);
        Assert.Equal(1, source.Count);

        var target = source.Clone();

        Assert.Equal(1, target.Count);
        Assert.True(target.Contains("age"));

        Assert.Equal("[one]..[three]", target.Identifier.Value);
        Assert.True(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.Equal(50, (int)target.GetValue("AGE", out _)!);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_Identifier()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", metadata: [pair]);
        Assert.Equal(7, source.Count);

        var target = source.WithIdentifier(Identifier.Create(engine, "x"));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^3]));
        Assert.Equal("[x]", target.Identifier.Value);

        target = source.WithIdentifier(Identifier.Create(engine));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^3]));
        Assert.Null(target.Identifier.Value);

        target = source.WithIdentifier(Identifier.Create(engine, "x."));
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^3]));
        Assert.Equal("[x].", target.Identifier.Value);

        try { source.WithIdentifier(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.WithIdentifier(Identifier.Create(new FakeEngine(), "any")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.WithIdentifier(Identifier.Create(engine, "x.a.b.c")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Identifier_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", metadata: [pair]);
        Assert.Equal(1, source.Count);

        var target = source.WithIdentifier(Identifier.Create(engine, "x"));
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[x]", target.Identifier.Value);

        target = source.WithIdentifier(Identifier.Create(engine));
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Null(target.Identifier.Value);

        target = source.WithIdentifier(Identifier.Create(engine, "x."));
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[x].", target.Identifier.Value);

        try { source.WithIdentifier(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.WithIdentifier(Identifier.Create(new FakeEngine(), "any")); Assert.Fail(); }
        catch (ArgumentException) { }

        target = source.WithIdentifier(Identifier.Create(engine, "x.a.b.c"));
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[x].[a].[b].[c]", target.Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_PrimaryKey()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", metadata: [pair]);
        Assert.Equal(7, source.Count);
        Assert.False(source.IsPrimaryKey);

        var target = source.WithIsPrimaryKey(true);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsPrimaryKey);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_PrimaryKey_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", metadata: [pair]);
        Assert.Equal(1, source.Count);
        Assert.False(source.IsPrimaryKey);

        var target = source.WithIsPrimaryKey(true);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.IsPrimaryKey);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_UniqueValued()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", metadata: [pair]);
        Assert.Equal(7, source.Count);
        Assert.False(source.IsUniqueValued);

        var target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsUniqueValued);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_UniqueValued_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", metadata: [pair]);
        Assert.Equal(1, source.Count);
        Assert.False(source.IsUniqueValued);

        var target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.IsUniqueValued);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_ReadOnly()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", metadata: [pair]);
        Assert.Equal(7, source.Count);
        Assert.False(source.IsReadOnly);

        var target = source.WithIsReadOnly(true);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_ReadOnly_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", metadata: [pair]);
        Assert.Equal(1, source.Count);
        Assert.False(source.IsReadOnly);

        var target = source.WithIsReadOnly(true);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.IsReadOnly);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isPrimaryKey: true, metadata: [pair]);
        Assert.Equal(7, source.Count);

        var tag = "any";
        var target = source.Replace(tag, "other");
        Assert.NotSame(source, target);
        Assert.Equal(8, target.Count);
        Assert.Equal("other", target.GetValue("ANY", out _));

        tag = engine.KnownTags.IdentifierTags[^3];
        target = source.Replace(tag, null);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^3]));
        Assert.Equal("[three]", target.Identifier.Value);

        tag = engine.KnownTags.IdentifierTags[^1];
        target = source.Replace(tag, null);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^3]));
        Assert.Equal("[one]..", target.Identifier.Value);

        tag = engine.KnownTags.PrimaryKeyTag!;
        target = source.Replace(tag, false);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.False(target.IsPrimaryKey);

        tag = engine.KnownTags.UniqueValuedTag!;
        target = source.Replace(tag, true);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsUniqueValued);

        tag = engine.KnownTags.ReadOnlyTag!;
        target = source.Replace(tag, true);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsReadOnly);

        try { source.Replace(null!, "any"); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Replace(engine.KnownTags.IdentifierTags[^1], 50); Assert.Fail(); }
        catch (UnExpectedException) { }

        try { source.Replace(engine.KnownTags.PrimaryKeyTag!, 50); Assert.Fail(); }
        catch (UnExpectedException) { }

        try { source.Replace(engine.KnownTags.UniqueValuedTag!, 50); Assert.Fail(); }
        catch (UnExpectedException) { }

        try { source.Replace(engine.KnownTags.ReadOnlyTag!, 50); Assert.Fail(); }
        catch (UnExpectedException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isPrimaryKey: true, metadata: [pair]);
        Assert.Equal(1, source.Count);

        var target = source.Replace("any", "other");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("other", target.GetValue("ANY", out _));

        target = source.Replace("AGE", 60);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal(60, (int)target.GetValue("AGE", out _)!);

        try { source.Replace(null!, "any"); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isPrimaryKey: true, metadata: [pair]);
        Assert.Equal(7, source.Count);

        var target = source.Add("any", "other");
        Assert.NotSame(source, target);
        Assert.Equal(8, target.Count);
        Assert.True(target.Contains("ANY"));
        Assert.Equal("other", target.GetValue("ANY", out _));

        try { source.Add(null!, "other"); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add("", "other"); Assert.Fail(); }
        catch (EmptyException) { }

        try { source.Add("AGE", "other"); Assert.Fail(); }
        catch (DuplicateException) { }

        var tag = engine.KnownTags.IdentifierTags[^1];
        target = source.Add(tag, "other");
        Assert.NotSame(source, tag);
        Assert.Equal(7, target.Count);
        Assert.Equal("[one]..[other]", target.Identifier.Value);

        tag = engine.KnownTags.IdentifierTags[^3];
        target = source.Add(tag, null);
        Assert.NotSame(source, tag);
        Assert.Equal(5, target.Count);
        Assert.Equal("[three]", target.Identifier.Value);

        tag = engine.KnownTags.PrimaryKeyTag!;
        target = source.Add(tag, false);
        Assert.NotSame(source, tag);
        Assert.Equal(7, target.Count);
        Assert.False(target.IsPrimaryKey);

        tag = engine.KnownTags.UniqueValuedTag!;
        target = source.Add(tag, true);
        Assert.NotSame(source, tag);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsUniqueValued);

        tag = engine.KnownTags.ReadOnlyTag!;
        target = source.Add(tag, true);
        Assert.NotSame(source, tag);
        Assert.Equal(7, target.Count);
        Assert.True(target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isPrimaryKey: true, metadata: [pair]);
        Assert.Equal(1, source.Count);

        var target = source.Add("any", "other");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("ANY"));
        Assert.Equal("other", target.GetValue("ANY", out _));

        try { source.Add(null!, "other"); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add("", "other"); Assert.Fail(); }
        catch (EmptyException) { }

        try { source.Add("AGE", "other"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isPrimaryKey: true, metadata: [pair]);
        Assert.Equal(7, source.Count);

        var tag = "any";
        var target = source.Remove(tag);
        Assert.Same(source, target);

        tag = engine.KnownTags.IdentifierTags[^3];
        target = source.Remove(tag);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[three]", target.Identifier.Value);

        tag = engine.KnownTags.IdentifierTags[^1];
        target = source.Remove(tag);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.Equal("[one]..", target.Identifier.Value);

        tag = "AGE";
        target = source.Remove(tag);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.False(target.Contains("AGE"));

        try { source.Remove(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Remove(""); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isPrimaryKey: true, metadata: [pair]);
        Assert.Equal(1, source.Count);

        var tag = "any";
        var target = source.Remove(tag);
        Assert.Same(source, target);

        tag = "AGE";
        target = source.Remove(tag);
        Assert.NotSame(source, target);
        Assert.Equal(0, target.Count);
        Assert.False(target.Contains("AGE"));

        try { source.Remove(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Remove(""); Assert.Fail(); }
        catch (EmptyException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isPrimaryKey: true, metadata: [pair]);
        Assert.Equal(7, source.Count);

        var target = source.Clear();
        Assert.NotSame(source, target);

        Assert.Equal(4, target.Count);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.True(target.Contains(engine.KnownTags.PrimaryKeyTag!));
        Assert.True(target.Contains(engine.KnownTags.UniqueValuedTag!));
        Assert.True(target.Contains(engine.KnownTags.ReadOnlyTag!));

        Assert.False(target.Contains("age"));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^3]));

        Assert.Null(target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isPrimaryKey: true, metadata: [pair]);
        Assert.Equal(1, source.Count);

        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Equal(0, target.Count);
        Assert.False(target.Contains("age"));

        Assert.Null(target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
    }
}