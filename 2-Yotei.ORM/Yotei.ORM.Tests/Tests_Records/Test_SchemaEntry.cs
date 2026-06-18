using Entry = Yotei.ORM.Records.Code.SchemaEntry;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_SchemaEntry
{
    //static bool Contains(this ISchemaEntry entry, string name, object? value)
    //{
    //    var item = entry.Find(name);
    //    return item is not null && item.Value.EqualsEx(value);
    //}

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_NoTags_Create_Empty()
    //{
    //    var engine = new FakeEngine() { KnownTags = new KnownTags() };
    //    var entry = new Entry(engine);

    //    Assert.Equal(0, entry.Count);
    //    Assert.Null(entry.Identifier.Value);
    //    Assert.False(entry.IsPrimaryKey);
    //    Assert.False(entry.IsUniqueValued);
    //    Assert.False(entry.IsReadOnly);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_WithTags_Create_Empty()
    //{
    //    var engine = new FakeEngine();
    //    var entry = new Entry(engine);

    //    Assert.Equal(3, entry.Count);
    //    Assert.Null(entry.Identifier.Value);
    //    Assert.False(entry.IsPrimaryKey);
    //    Assert.False(entry.IsUniqueValued);
    //    Assert.False(entry.IsReadOnly);
    //}
}