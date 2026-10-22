#pragma warning disable CA1859

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

    static IEngine CreateEngine(bool withTags) => withTags
        ? new FakeEngine() { KnownTags = new FakeKnownTags(ignoreCase: true) }
        : new Engine() { KnownTags = new KnownTags(ignoreCase: true) };

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_No_Tags()
    {
        var engine = CreateEngine(withTags: false);
        var element = new Element(engine);

        Assert.Null(element.Identifier);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.Null(element.IsReadOnly);
        Assert.Empty(element);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_With_Tags()
    {
        var engine = CreateEngine(withTags: true);
        var element = new Element(engine);

        Assert.Null(element.Identifier);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.Null(element.IsReadOnly);
        Assert.Empty(element);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata_No_Tags()
    {
        var engine = CreateEngine(withTags: false);
        var element = new Element(engine, [new Item("Age", 50)]);

        Assert.Null(element.Identifier);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.Null(element.IsReadOnly);
        Assert.Single(element);
        Assert.True(element.Contains("Age", 50));
        Assert.True(element.Contains("AGE", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Metadata_With_Tags()
    {
        var engine = CreateEngine(withTags: true);
        var element = new Element(engine, [new Item("Age", 50)]);

        Assert.Null(element.Identifier);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.Null(element.IsReadOnly);
        Assert.Single(element);
        Assert.True(element.Contains("Age", 50));
        Assert.True(element.Contains("AGE", 50));

        element = new Element(engine, [new Item("Age", 50), new Item("ColumnTag", "any")]);
        Assert.Equal("[any]", element.Identifier!.Value);
        Assert.Equal(2, element.Count);
        Assert.True(element.Contains("AGE", 50));
        Assert.True(element.Contains("COLUMNTAG3", "any"));

        element = new Element(engine, [new Item("SchemaTag3", "any"), new Item("ColumnTag", "other")]);
        Assert.Equal("[any]..[other]", element.Identifier!.Value);
        Assert.Equal(3, element.Count);
        Assert.True(element.Contains("SCHEMATAG", "any"));
        Assert.True(element.Contains("TABLETAG2", null));
        Assert.True(element.Contains("COLUMNTAG3", "other"));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values_No_Tags()
    {
        var engine = CreateEngine(withTags: false);
        var element = new Element(engine, "column", isReadOnly: true);

        Assert.Equal("[column]", element.Identifier!.Value);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.True(element.IsReadOnly);
        Assert.Empty(element);

        element = new Element(engine, "column", isReadOnly: true, range: [new Item("Age", 50)]);
        Assert.Equal("[column]", element.Identifier!.Value);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.True(element.IsReadOnly);
        Assert.Single(element);
        Assert.True(element.Contains("AGE", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values_With_Tags()
    {
        var engine = CreateEngine(withTags: true);
        var element = new Element(engine, ".column", isReadOnly: true);

        Assert.Equal("[column]", element.Identifier!.Value);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.True(element.IsReadOnly);
        Assert.Equal(2, element.Count);
        Assert.True(element.Contains("COLUMNTAG3", "column"));
        Assert.True(element.Contains("READONLYTAG3", true));

        element = new Element(engine, "schema..", isReadOnly: true, range: [new Item("Age", 50)]);

        Assert.Equal("[schema]..", element.Identifier!.Value);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.True(element.IsReadOnly);
        Assert.Equal(5, element.Count);
        Assert.True(element.Contains("SCHEMATAG3", "schema"));
        Assert.True(element.Contains("TABLETAG2", null));
        Assert.True(element.Contains("COLUMNTAG", null));
        Assert.True(element.Contains("READONLYTAG3", true));
        Assert.True(element.Contains("AGE", 50));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Values_With_Tags_Duplicated_In_Range()
    {
        var engine = CreateEngine(withTags: true);
        var element = new Element(engine, "schema..column", range: [
            new Item("SchemaTag2", "xhema"),
            new Item("ColumnTag3", "xolumn")]);

        Assert.Equal("[xhema]..[xolumn]", element.Identifier!.Value);
        Assert.Null(element.IsPrimaryKey);
        Assert.Null(element.IsUniqueValued);
        Assert.Null(element.IsReadOnly);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Equal_No_Tags()
    {
        var engine = CreateEngine(withTags: false);
        var source = new Element(engine);
        var target = new Element(engine);
        Assert.True(source.Equals(target));

        source = new Element(engine, "table.column", isUniqueValued: true, range: [new Item("Age", 50)]);
        target = new Element(engine, "table.column", isUniqueValued: true, range: [new Item("Age", 50)]);
        Assert.True(source.Equals(target));

        target = new Element(engine, "table.column", range: [new Item("Age", 50)]);
        Assert.False(source.Equals(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Equal_With_Tags()
    {
        var engine = CreateEngine(withTags: true);
        var source = new Element(engine);
        var target = new Element(engine);
        Assert.True(source.Equals(target));

        source = new Element(engine, "table.column", isUniqueValued: true, range: [new Item("Age", 50)]);
        target = new Element(engine, range: [new Item("UniqueValuedTag", true), new Item("Age", 50), new Item("COLUMNTAG2", "column"), new Item("TABLETAG", "table")]);
        Assert.True(source.Equals(target));

        target = new Element(engine, range: [new Item("Age", 50), new Item("COLUMNTAG2", "column"), new Item("TABLETAG", "table")]);
        Assert.False(source.Equals(target));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone_No_Tags()
    {
        var engine = CreateEngine(withTags: false);
        var source = new Element(engine);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source, target);

        source = new Element(engine, "table.column", isUniqueValued: true, range: [new Item("Age", 50)]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_With_Tags()
    {
        var engine = CreateEngine(withTags: true);
        var source = new Element(engine);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source, target);

        source = new Element(engine, "table.column", isUniqueValued: true, range: [new Item("Age", 50)]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_With_Identifier()

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_With_IsPrimaryKey()

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_With_IsUniqueValued()

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_With_IsReadOnly()

    // ----------------------------------------------------

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