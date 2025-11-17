namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_MetadataEntry
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var entry = new MetadataEntry("Len", 25);
        Assert.Equal("Len", entry.Name);
        Assert.Equal(25, entry.Value);

        try { _ = new MetadataEntry(null!, 50); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare()
    {
        var source = new MetadataEntry("Len", 25);
        var target = new MetadataEntry("LEN", 25);

        Assert.False(source.Equals(target));
        Assert.True(source.Equals(target, caseSensitiveNames: false));
    }
}