namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_SchemaEntry
{
    static bool Contains(this ISchemaEntry entry, string name, object? value)
    {
        var item = entry.Find(name);
        return item is not null && item.Value.EqualsEx(value);
    }

    static IEngine NoTagsEngine(
        bool ignoreCase = false) => new FakeEngine() { KnownTags = new KnownTags(ignoreCase) };

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_NoTags_Create_Empty()
    {
    }
}