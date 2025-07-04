﻿namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_KnownTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var tags = new KnownTags(false);

        Assert.Empty(tags);
        Assert.Empty(tags.IdentifierTags);
        Assert.Null(tags.PrimaryKeyTag);
        Assert.Null(tags.UniqueValuedTag);
        Assert.Null(tags.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var tags = new FakeKnownTags(false);

        Assert.NotEmpty(tags);
        Assert.Equal(3, tags.IdentifierTags.Count);
        Assert.Equal("SchemaTag", tags.IdentifierTags[0].Default);
        Assert.Equal("TableTag", tags.IdentifierTags[1].Default);
        Assert.Equal("ColumnTag", tags.IdentifierTags[2].Default);
        Assert.Equal("PrimaryTag", tags.PrimaryKeyTag!.Default);
        Assert.Equal("UniqueTag", tags.UniqueValuedTag!.Default);
        Assert.Equal("ReadOnlyTag", tags.ReadOnlyTag!.Default);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Duplicated()
    {
        try
        {
            _ = new KnownTags(
                false,
                new IdentifierTags(
                    false, [
                        new MetadataTag(false, "TableTag"),
                        new MetadataTag(false, "ColumnTag")]),
                new MetadataTag(false, "TableTag"));

            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var tags = new FakeKnownTags(false);

        Assert.False(tags.Contains("any"));
        Assert.True(tags.Contains("SCHEMATAG"));
        Assert.True(tags.Contains(["any", "SCHEMATAG"]));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new FakeKnownTags(false);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.CaseSensitiveTags, target.CaseSensitiveTags);
        Assert.Same(source.IdentifierTags, target.IdentifierTags);
        Assert.Same(source.PrimaryKeyTag, target.PrimaryKeyTag);
        Assert.Same(source.UniqueValuedTag, target.UniqueValuedTag);
        Assert.Same(source.ReadOnlyTag, target.ReadOnlyTag);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_IdentifierTags()
    {
        var source = new FakeKnownTags(false);

        var items = new IdentifierTags(false);
        var target = source.WithIdentifierTags(items);
        Assert.NotSame(source, target);
        Assert.Same(items, target.IdentifierTags);

        items = new IdentifierTags(false, [new MetadataTag(false, "SchemaTag")]);
        target = source.WithIdentifierTags(items);
        Assert.NotSame(source, target);
        Assert.Same(items, target.IdentifierTags);

        items = new IdentifierTags(false, [new MetadataTag(false, "PRIMARYTAG")]);
        try { source.WithIdentifierTags(items); Assert.Fail(); }
        catch (DuplicateException) { }

        items = null!;
        try { source.WithIdentifierTags(items); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_PrimaryKeyTag()
    {
        var source = new FakeKnownTags(false);
        var target = source.WithPrimaryKeyTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.PrimaryKeyTag);

        target = source.WithPrimaryKeyTag(source.PrimaryKeyTag);
        Assert.NotSame(source, target);
        Assert.Same(source.PrimaryKeyTag, target.PrimaryKeyTag);

        var xany = new MetadataTag(false, "any");
        target = source.WithPrimaryKeyTag(xany);
        Assert.NotSame(source, target);
        Assert.Same(xany, target.PrimaryKeyTag);

        xany = new MetadataTag(false, "SCHEMATAG");
        try { source.WithPrimaryKeyTag(xany); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_UniqueValuedTag()
    {
        var source = new FakeKnownTags(false);
        var target = source.WithUniqueValuedTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.UniqueValuedTag);

        target = source.WithUniqueValuedTag(source.UniqueValuedTag);
        Assert.NotSame(source, target);
        Assert.Same(source.UniqueValuedTag, target.UniqueValuedTag);

        var xany = new MetadataTag(false, "any");
        target = source.WithUniqueValuedTag(xany);
        Assert.NotSame(source, target);
        Assert.Same(xany, target.UniqueValuedTag);

        xany = new MetadataTag(false, "SCHEMATAG");
        try { source.WithUniqueValuedTag(xany); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_ReadOnlyTag()
    {
        var source = new FakeKnownTags(false);
        var target = source.WithReadOnlyTag(null);
        Assert.NotSame(source, target);
        Assert.Null(target.ReadOnlyTag);

        target = source.WithReadOnlyTag(source.ReadOnlyTag);
        Assert.NotSame(source, target);
        Assert.Same(source.ReadOnlyTag, target.ReadOnlyTag);

        var xany = new MetadataTag(false, "any");
        target = source.WithReadOnlyTag(xany);
        Assert.NotSame(source, target);
        Assert.Same(xany, target.ReadOnlyTag);

        xany = new MetadataTag(false, "SCHEMATAG");
        try { source.WithReadOnlyTag(xany); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new FakeKnownTags(false);
        var target = source.Clear();

        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Empty(target.IdentifierTags);
        Assert.Null(target.PrimaryKeyTag);
        Assert.Null(target.UniqueValuedTag);
        Assert.Null(target.ReadOnlyTag);
    }
}