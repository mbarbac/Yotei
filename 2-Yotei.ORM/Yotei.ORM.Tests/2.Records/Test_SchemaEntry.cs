#pragma warning disable CA1859

using Pair = Yotei.ORM.Code.MetadataEntry;
using Tag = Yotei.ORM.Code.MetadataTag;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_SchemaEntry
{
    static bool Contains(this ISchemaEntry entry, string name, object? value)
    {
        if (!entry.TryGet(name, out var item)) return false;
        return item.Value.EquivalentTo(value);
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
            "one.two",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        Assert.NotEmpty(entry);
        Assert.Equal("[one].[two]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);

        Assert.True(entry.Contains("TableTag", "one"));
        Assert.True(entry.Contains("ColumnTag", "two"));
        Assert.True(entry.Contains("ReadonlyTag", true));
        Assert.True(entry.Contains("age", 50));
        Assert.True(entry.Contains("years", 50));

        Assert.False(entry.Contains("PrimaryTag"));
        Assert.False(entry.Contains("UniqueTag"));
        Assert.False(entry.Contains("SchemaTag"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Metadata()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine, [
            new Pair(new Tag(false, "TableTag"), "[one]"),
            new Pair(new Tag(false, "ColumnTag"), "[two]"),
            new Pair(new Tag(false, "ReadonlyTag"), true),
            new Pair(new Tag(false, ["age", "years"]), 50)]);

        Assert.NotEmpty(entry);
        Assert.Equal("[one].[two]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.True(entry.IsReadOnly);

        Assert.True(entry.Contains("TableTag", "one"));
        Assert.True(entry.Contains("ColumnTag", "two"));
        Assert.True(entry.Contains("ReadonlyTag", true));
        Assert.True(entry.Contains("age", 50));
        Assert.True(entry.Contains("years", 50));

        Assert.False(entry.Contains("PrimaryTag"));
        Assert.False(entry.Contains("UniqueTag"));
        Assert.False(entry.Contains("SchemaTag"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var entry = new SchemaEntry(engine,
            "one.two",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        Assert.Equal(4, entry.Count);
        Assert.Equal("one", entry["TABLETAG"].Value);
        Assert.Equal("two", entry["COLUMNTAG"].Value);
        Assert.True((bool)entry["READONLYTAG"].Value!);
        Assert.Equal(50, (int)entry["AGE"].Value!);
        Assert.Equal(50, (int)entry["YEARS"].Value!);

        try { _ = entry["other"]; Assert.Fail(); }
        catch (NotFoundException) { }

        Assert.True(entry.TryGet("years", out var item));
        Assert.Equal(50, (int)item.Value!);

        Assert.True(entry.Contains("TableTag"));
        Assert.False(entry.Contains("SchemaTag"));
        Assert.True(entry.Contains(["any", "other", "years"]));

        Assert.False(entry.Contains("other"));
        Assert.True(entry.TryGet(["any", "other", "years"], out item));
        Assert.Equal(50, (int)item.Value!);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine,
            "one.two",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Identifier.Value, target.Identifier.Value);
        Assert.Equal(source.IsPrimaryKey, target.IsPrimaryKey);
        Assert.Equal(source.IsUniqueValued, target.IsUniqueValued);
        Assert.Equal(source.IsReadOnly, target.IsReadOnly);

        Assert.True(target.Equals(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "one.two", isReadOnly: true);

        var target = source.WithIdentifier(new Identifier(engine, "alpha.beta"));
        Assert.NotSame(source, target);
        Assert.Equal("[alpha].[beta]", target.Identifier.Value);

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

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine,
            "one.two",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        var target = source.ReplaceValue("TableTag", "alpha");
        Assert.NotSame(source, target);
        Assert.Equal("[alpha].[two]", target.Identifier.Value);

        target = source.ReplaceValue("years", 60);
        Assert.NotSame(source, target);
        Assert.Equal(60, (int)target["age"].Value!);

        target = source.ReplaceValue("SchemaTag", "alpha"); // Not exist, but ok as it is well known
        Assert.NotSame(source, target);
        Assert.Equal("[alpha].[one].[two]", target.Identifier.Value);

        target = source.ReplaceValue("any", 0); // Not exist, not well known, then ignore...
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine, "one.two", isReadOnly: true);

        var sensitive = engine.KnownTags.CaseSensitiveTags;
        var tag = new Tag(sensitive, ["Other", "SchemaTag"]);
        var target = source.Add(new Pair(tag, "[alpha]"));

        Assert.NotSame(source, tag);
        Assert.Equal("[alpha].[one].[two]", target.Identifier.Value);

        try { _ = source.Add(new Pair(new Tag(sensitive, "ReadONLYtag"), false)); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Add(new Pair(new Tag(sensitive, "SchemaTag"), 0)); Assert.Fail(); }
        catch (InvalidCastException) { }

        try { _ = source.Add(new Pair(new Tag(sensitive, "PrimaryTag"), 0)); Assert.Fail(); }
        catch (InvalidCastException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine();
        var source = new SchemaEntry(engine,
            "one.two",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        var target = source.Remove("tableTAG");
        Assert.NotSame(source, target);
        Assert.Equal("[two]", target.Identifier.Value);

        target = source.Remove("YEARS");
        Assert.NotSame(source, target);
        Assert.False(target.Contains("age"));

        target = source.Remove("other");
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
            "one.two",
            isReadOnly: true,
            range: [new Pair(new Tag(false, ["age", "years"]), 50)]);

        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Null(target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.Equal(0, target.Count);

    }
}