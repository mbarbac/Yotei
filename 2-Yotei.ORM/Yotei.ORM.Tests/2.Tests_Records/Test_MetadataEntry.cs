namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
//[Enforced]
public static class Test_MetadataEntry
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var tag = new MetadataTag(false, ["one", "two"]);
        var entry = new MetadataEntry(tag, null);

        Assert.Same(tag, entry.Tag);
        Assert.Null(entry.Value);

        try { entry = new(null!, null); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }
}