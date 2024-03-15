namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_MetadataEntry
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        MetadataTag tag = new(false, ["one", "two"]);
        object? value = null;

        var entry = new MetadataEntry(tag, value);
        Assert.Same(tag, entry.Tag);
        Assert.Null(entry.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Tag()
    {
        MetadataTag tag = new(false, ["one", "two"]);

        var source = new MetadataEntry(tag, null);
        var target = source.WithTag(tag);
        Assert.Same(source, target);

        var other = tag.Clone();
        target = source.WithTag(other);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Value()
    {
        var source = new MetadataEntry(new MetadataTag(false, ["one", "two"]), null);
        var target = source.WithValue(null);
        Assert.Same(source, target);

        source = new MetadataEntry(new MetadataTag(false, ["one", "two"]), "007");
        target = source.WithValue("007");
        Assert.Same(source, target);

        target = source.WithValue(50);
        Assert.NotSame(source, target);
    }
}