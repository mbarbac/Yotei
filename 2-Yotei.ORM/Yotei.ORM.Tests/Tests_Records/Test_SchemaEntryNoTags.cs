using NuGet.Frameworks;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_SchemaEntryNoTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new SchemaEntry(engine);

        Assert.Equal(0, entry.Count);

        Assert.Null(entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.False(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new SchemaEntry(
            engine,
            "one.two",
            isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        Assert.Equal(1, entry.Count);
        Assert.True(entry.Contains("age"));

        Assert.Equal("[one].[two]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.ContainsItem("age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Metadata()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var entry = new SchemaEntry(
            engine,
            "one..three",
            isUniqueValued: true,
            metadata: [
                new MetadataPair("age", 50)
            ]);

        Assert.Equal(1, entry.Count);
        Assert.True(entry.Contains("age"));

        Assert.Equal("[one]..[three]", entry.Identifier.Value);
        Assert.False(entry.IsPrimaryKey);
        Assert.True(entry.IsUniqueValued);
        Assert.False(entry.IsReadOnly);
        Assert.True(entry.ContainsItem("age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine,
            "one.two",
            isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Clone();
        Assert.NotSame(source, target);

        Assert.Equal(1, target.Count);
        Assert.True(target.Contains("age"));

        Assert.Equal("[one].[two]", target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.True(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
        Assert.True(target.ContainsItem("age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Equality() { }

    //[Enforced]
    [Fact]
    public static void Test_WithIdentifier()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two", isUniqueValued: true,
            metadata: [new MetadataPair("age", 50)]);

        var target = source.WithIdentifier(new Identifier(engine, "x"));
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[x]", target.Identifier.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_WithPrimaryKey()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.WithIsPrimaryKey(true);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.IsPrimaryKey);
    }

    //[Enforced]
    [Fact]
    public static void Test_WithUniqueValued()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.WithIsUniqueValued(true);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.IsUniqueValued);
    }

    //[Enforced]
    [Fact]
    public static void Test_WithReadOnly()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.WithIsReadOnly(true);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.IsReadOnly);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Other()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Replace("AGE", 60);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.ContainsItem("age", 60));
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two");

        var target = source.Add(new("age", 50));
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.True(target.ContainsItem("age", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Remove("age");
        Assert.NotSame(source, target);
        Assert.Equal(0, target.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var source = new SchemaEntry(
            engine, "one.two",
            metadata: [new MetadataPair("age", 50)]);

        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Equal(0, target.Count);
        Assert.Null(target.Identifier.Value);
        Assert.False(target.IsPrimaryKey);
        Assert.False(target.IsUniqueValued);
        Assert.False(target.IsReadOnly);
    }
}