using System.ComponentModel.DataAnnotations;
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
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var entry = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        Assert.Equal(7, entry.Count);
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.True(entry.Contains(engine.KnownTags.IdentifierTags[^3]));
        Assert.True(entry.Contains(engine.KnownTags.PrimaryKeyTag!));
        Assert.True(entry.Contains(engine.KnownTags.UniqueValuedTag!));
        Assert.True(entry.Contains(engine.KnownTags.ReadOnlyTag!));
        Assert.True(entry.Contains("AGE"));

        Assert.Equal("[one]..[three]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.Equal(50, (int)entry.GetValue("AGE", out _)!);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var entry = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        Assert.Equal(1, entry.Count);
        Assert.True(entry.Contains("AGE"));

        Assert.Equal("[one]..[three]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
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
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^3]));
        Assert.True(target.Contains(engine.KnownTags.PrimaryKeyTag!));
        Assert.True(target.Contains(engine.KnownTags.UniqueValuedTag!));
        Assert.True(target.Contains(engine.KnownTags.ReadOnlyTag!));
        Assert.True(target.Contains("AGE"));

        Assert.Equal("[one]..[three]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.True(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.Equal(50, (int)target.GetValue("AGE", out _)!);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.Contains("AGE"));

        Assert.Equal("[one]..[three]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.True(target.IsUniqueValued);
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
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        var target = source.WithIdentifier(Identifier.Create(engine, "alpha"));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[alpha]", target.Identifier.Value);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^3]));

        target = source.WithIdentifier(Identifier.Create(engine));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Null(target.Identifier.Value);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^3]));

        target = source.WithIdentifier(Identifier.Create(engine, "x."));
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("[x].", target.Identifier.Value);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^3]));

        try { source.WithIdentifier(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.WithIdentifier(Identifier.Create(new FakeEngine(), "any")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Identifier_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        var target = source.WithIdentifier(Identifier.Create(engine, "alpha"));
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[alpha]", target.Identifier.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_ReadOnly()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

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
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

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
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        var tag = engine.KnownTags.IdentifierTags[^3];
        var target = source.Replace(tag, null);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[three]", target.Identifier.Value);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.False(target.Contains(engine.KnownTags.IdentifierTags[^3]));

        target = source.Replace(engine.KnownTags.IdentifierTags[^1], null);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.Equal("[one]..", target.Identifier.Value);

        target = source.Replace("AGE", 60);
        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Equal(60, (int)target.GetValue("AGE", out _)!);

        try { source.Replace(null!, "any"); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        var target = source.Replace("AGE", 60);
        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Equal(60, (int)target.GetValue("AGE", out _)!);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "three", isUniqueValued: true, metadata: [pair]);

        var target = source.Add("any", "other");
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("other", target.GetValue("any", out _));

        try { source.Add("age", 60); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(null!, "any"); Assert.Fail(); }
        catch (ArgumentNullException) { }

        var tag = engine.KnownTags.IdentifierTags[^3];
        target = source.Add(tag, "one");
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.Equal("[one]..[three]", target.Identifier.Value);
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^1]));
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^2]));
        Assert.True(target.Contains(engine.KnownTags.IdentifierTags[^3]));

        tag = engine.KnownTags.PrimaryKeyTag;
        target = source.Add(tag!, true);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.True(target.IsPrimaryKey);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        var target = source.Add("any", "other");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("other", target.GetValue("any", out _));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        var tag = engine.KnownTags.IdentifierTags[^3];
        var target = source.Remove(tag);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[three]", target.Identifier.Value);

        tag = engine.KnownTags.UniqueValuedTag;
        target = source.Remove(tag!);
        Assert.NotSame(source, target);
        Assert.Equal(7, target.Count);
        Assert.False(target.IsUniqueValued);

        target = source.Remove("AGE");
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.False(target.Contains("AGE"));

        try { source.Remove(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        var target = source.Remove("AGE");
        Assert.NotSame(source, target);
        Assert.Equal(0, target.Count);
        Assert.False(target.Contains("AGE"));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Null(target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.False(target.Contains("age"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear_NoTags()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var pair = new TPair("age", 50);
        var source = new SchemaEntry(engine, "one..three", isUniqueValued: true, metadata: [pair]);

        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Equal(0, target.Count);
        Assert.Null(target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.False(target.Contains("age"));
    }
}