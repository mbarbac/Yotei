using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Entry = Yotei.ORM.Records.Code.SchemaEntry;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public class Test_SchemaEntry
{
    static bool Contains(ISchemaEntry entry, string name, object? value)
    {
        var item = entry.Find(name);
        return item is not null && item.Value.EqualsEx(value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Create_Empty()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var item = new Entry(engine);

        Assert.Null(item.Identifier.Value);
        Assert.False(item.IsPrimaryKey);
        Assert.False(item.IsUniqueValued);
        Assert.False(item.IsReadOnly);
        Assert.Equal(0, item.Count);

        engine = new FakeEngine();
        item = new Entry(engine);

        Assert.Null(item.Identifier.Value);
        Assert.False(item.IsPrimaryKey);
        Assert.False(item.IsUniqueValued);
        Assert.False(item.IsReadOnly);
        Assert.Equal(3, item.Count);
    }

    //[Enforced]
    [Fact]
    public void Test_Create_From_Values()
    {
        var engine = new FakeEngine() { KnownTags = new KnownTags(false) };
        var item = new Entry(engine, "column", isReadonly: true);
        Assert.Equal("[column]", item.Identifier.Value);
        Assert.False(item.IsPrimaryKey);
        Assert.False(item.IsUniqueValued);
        Assert.True(item.IsReadOnly);
        Assert.Equal(4, item.Count);

        //Assert.NotNull(item.Identifier); Assert.Null(item.Identifier.Value);
        //Assert.False(item.IsPrimaryKey);
        //Assert.False(item.IsUniqueValued);
        //Assert.False(item.IsReadOnly);
        //Assert.Equal(0, item.Count);

        //engine = new FakeEngine();
        //item = new Entry(engine);

        //Assert.NotNull(item.Identifier); Assert.Null(item.Identifier.Value);
        //Assert.False(item.IsPrimaryKey);
        //Assert.False(item.IsUniqueValued);
        //Assert.False(item.IsReadOnly);
        //Assert.Equal(3, item.Count);
    }
}