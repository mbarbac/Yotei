using Xunit.Internal;

namespace Yotei.ORM.Records.Tests;

// ========================================================
//[Enforced]
public static class Test_KnownTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var tags = new KnownTags(false);
        Assert.False(tags.IgnoreCase);
        Assert.Null(tags.IdentifierTags);
        Assert.Null(tags.PrimaryKeyTag);
        Assert.Null(tags.UniqueValuedTag);
        Assert.Null(tags.ReadOnlyTag);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Fake_Default()
    {
        var tags = new FakeKnownTags();

        Assert.False(tags.IgnoreCase);
        Assert.Equal(3, tags.IdentifierTags!.Value.Length);
        Assert.Equal("SchemaTag", tags.IdentifierTags.Value[0].Default);
        Assert.Equal("TableTag", tags.IdentifierTags.Value[1].Default);
        Assert.Equal("ColumnTag", tags.IdentifierTags.Value[2].Default);
        Assert.Equal("PrimaryKeyTag", tags.PrimaryKeyTag!.Default);
        Assert.Equal("UniqueValuedTag", tags.UniqueValuedTag!.Default);
        Assert.Equal("ReadOnlyTag", tags.ReadOnlyTag!.Default);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var ignoreCase = true;
        var source = new KnownTags(
            ignoreCase,
            [   new MetadataTag(ignoreCase, ["Alpha"]),
                new MetadataTag(ignoreCase, ["Beta"]),
                new MetadataTag(ignoreCase, ["Delta"]),
            ],
            new MetadataTag(ignoreCase, ["Epsilon"]),
            new MetadataTag(ignoreCase, ["Gamma"]),
            new MetadataTag(ignoreCase, ["Omega"]));

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.IgnoreCase, target.IgnoreCase);
        Assert.Equal(source.IdentifierTags, target.IdentifierTags);
        Assert.Same(source.PrimaryKeyTag, target.PrimaryKeyTag);
        Assert.Same(source.UniqueValuedTag, target.UniqueValuedTag);
        Assert.Same(source.ReadOnlyTag, target.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_IdentifierTags()
    {
        var source = new FakeKnownTags();

        var target = source.WithIdentifierTags(null);
        Assert.NotSame(source, target);
        Assert.Null(target.IdentifierTags);

        target = source.WithIdentifierTags([]);
        Assert.NotSame(source, target);
        Assert.Null(target.IdentifierTags);

        try { source.WithIdentifierTags([null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        target = source.WithIdentifierTags([new MetadataTag(false, "Alpha")]);
        Assert.NotSame(source, target);
        Assert.Single(target.IdentifierTags!);
        Assert.Equal("Alpha", target.IdentifierTags!.Value[0].Default);

        try { source.WithIdentifierTags([new MetadataTag(true, "Alpha")]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.WithIdentifierTags([new MetadataTag(false, "ReadOnlyTag2")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_PrimaryKeyTag()
    {
        var source = new FakeKnownTags();

        var target = source.WithPrimaryKeyTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.PrimaryKeyTag);

        target = source.WithPrimaryKeyTag(new MetadataTag(false, "Alpha"));
        Assert.NotSame(source, target);
        Assert.Equal("Alpha", target.PrimaryKeyTag!.Default);

        try { source.WithPrimaryKeyTag(new MetadataTag(true, "Alpha")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.WithPrimaryKeyTag(new MetadataTag(false, "SchemaTag2")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_UniqueValuedTag()
    {
        var source = new FakeKnownTags();

        var target = source.WithUniqueValuedTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.UniqueValuedTag);

        target = source.WithUniqueValuedTag(new MetadataTag(false, "Alpha"));
        Assert.NotSame(source, target);
        Assert.Equal("Alpha", target.UniqueValuedTag!.Default);

        try { source.WithUniqueValuedTag(new MetadataTag(true, "Alpha")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.WithUniqueValuedTag(new MetadataTag(false, "SchemaTag2")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_ReadOnlyTag()
    {
        var source = new FakeKnownTags();

        var target = source.WithReadOnlyTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.ReadOnlyTag);

        target = source.WithReadOnlyTag(new MetadataTag(false, "Alpha"));
        Assert.NotSame(source, target);
        Assert.Equal("Alpha", target.ReadOnlyTag!.Default);

        try { source.WithReadOnlyTag(new MetadataTag(true, "Alpha")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.WithReadOnlyTag(new MetadataTag(false, "SchemaTag2")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_EnumerateNames()
    {
        var tags = new FakeKnownTags();
        Assert.Equal([
            "SchemaTag", "SchemaTag2", "SchemaTag3",
            "TableTag", "TableTag2", "TableTag3",
            "ColumnTag", "ColumnTag2", "ColumnTag3",
            "PrimaryKeyTag", "PrimaryKeyTag2", "PrimaryKeyTag3",
            "UniqueValuedTag", "UniqueValuedTag2", "UniqueValuedTag3",
            "ReadOnlyTag", "ReadOnlyTag2", "ReadOnlyTag3",
            ],
            tags.Names);
    }

    //[Enforced]
    [Fact]
    public static void Test_Contains()
    {
        var tags = new FakeKnownTags();
        Assert.False(tags.Contains("any"));
        Assert.True(tags.Contains("ReadOnlyTag3"));
    }

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny()
    {
        var tags = new FakeKnownTags();
        Assert.False(tags.ContainsAny(["any", "other"]));
        Assert.True(tags.ContainsAny(["any", "ReadOnlyTag3"]));
    }
}