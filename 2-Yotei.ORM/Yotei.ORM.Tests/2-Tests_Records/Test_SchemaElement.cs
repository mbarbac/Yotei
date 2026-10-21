using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Item = Yotei.ORM.Records.Code.MetadataItem;
using Element = Yotei.ORM.Records.Code.SchemaElement;

namespace Yotei.ORM.Records.Tests;

// ========================================================
[Enforced]
public static class Test_SchemaElement
{
    static bool Contains(this ISchemaElement element, string name, object? value)
    {
        var item = element.Find(name);
        return item is not null && item.Value.EqualsEx(value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_With_No_KnownTags()
    {
        var engine = new Engine();
        var element = new Element(engine);

        Assert.Null(element.Identifier);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.Null(element.IsReadOnly);
        Assert.Empty(element);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_With_KnownTags()
    {
        var engine = new FakeEngine();
        var element = new Element(engine);

        Assert.Null(element.Identifier);
        Assert.False(element.IsPrimaryKey);
        Assert.False(element.IsUniqueValued);
        Assert.False(element.IsReadOnly);
        Assert.Equal(3, element.Count);
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Create_From_Metadata_With_KnowTags()

    //[Enforced]
    //[Fact]
    //public static void Test_Create_From_Metadata_With_No_KnownTags()

    //[Enforced]
    //[Fact]
    //public static void Test_Create_From_Values_With_KnownTags()

    //[Enforced]
    //[Fact]
    //public static void Test_Create_From_Values_With_No_KnownTags()

    //[Enforced]
    //[Fact]
    //public static void Test_Clone()

    //[Enforced]
    //[Fact]
    //public static void Test_With_Identifier()

    //[Enforced]
    //[Fact]
    //public static void Test_With_IsPrimaryKey()

    //[Enforced]
    //[Fact]
    //public static void Test_With_IsUniqueValued()

    //[Enforced]
    //[Fact]
    //public static void Test_With_IsReadOnly()

    //[Enforced]
    //[Fact]
    //public static void Test_Count()

    //[Enforced]
    //[Fact]
    //public static void Test_This_Getter()

    //[Enforced]
    //[Fact]
    //public static void Test_This_Setter()

    //[Enforced]
    //[Fact]
    //public static void Test_Contains()

    //[Enforced]
    //[Fact]
    //public static void Test_Find()

    //[Enforced]
    //[Fact]
    //public static void Test_Add()

    //[Enforced]
    //[Fact]
    //public static void Test_AddRange()

    //[Enforced]
    //[Fact]
    //public static void Test_Update()

    //[Enforced]
    //[Fact]
    //public static void Test_UpdateRange()

    //[Enforced]
    //[Fact]
    //public static void Test_Remove()

    //[Enforced]
    //[Fact]
    //public static void Test_Clear()
}