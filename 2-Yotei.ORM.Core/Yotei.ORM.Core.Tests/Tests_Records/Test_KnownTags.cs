using Xunit.Sdk;

namespace Yotei.ORM.Core.Tests;

// ========================================================
//[Enforced]
public static class Test_KnownTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new KnownTags(false);
        Assert.Empty(items.IdentifierTags);
        Assert.Null(items.PrimaryKeyTag);
        Assert.Null(items.UniqueValuedTag);
        Assert.Null(items.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var items = new KnownTags(false,
            new KnownIdentifierTags(false, "schema.table.column"),
            "primary",
            "unique",
            "readonly");

        Assert.Equal(3, items.IdentifierTags.Count);
        Assert.Equal("schema", items.IdentifierTags[0]);
        Assert.Equal("table", items.IdentifierTags[1]);
        Assert.Equal("column", items.IdentifierTags[2]);
        Assert.Equal("primary", items.PrimaryKeyTag);
        Assert.Equal("unique", items.UniqueValuedTag);
        Assert.Equal("readonly", items.ReadOnlyTag);

        try
        {
            items = new KnownTags(false,
                new KnownIdentifierTags(false, "schema.table.column"),
                "table");
            Assert.Fail();
        }
        catch (DuplicateException) { }
        try
        {
            items = new KnownTags(false,
                new KnownIdentifierTags(false, "schema.table.column"),
                "primary",
                "table");
            Assert.Fail();
        }
        catch (DuplicateException) { }
        try
        {
            items = new KnownTags(false,
                new KnownIdentifierTags(false, "schema.table.column"),
                "primary",
                "unique",
                "table");
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }
}