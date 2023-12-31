using KnownPairs = Yotei.ORM.Records.Code.KnownPairs;
using IdentifierPairs = Yotei.ORM.Records.Code.IdentifierPairs;
using SPair = System.Collections.Generic.KeyValuePair<string, string?>;
using BPair = System.Collections.Generic.KeyValuePair<string, bool>;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_KnownPairs
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var items = new KnownPairs(engine);

        Assert.Null(items.IdentifierPairs);
        Assert.Null(items.PrimaryKeyPair);
        Assert.Null(items.ReadOnlyPair);
    }
    
    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var items = new KnownPairs(engine)
        {
            IdentifierPairs = new IdentifierPairs(engine, [new SPair("schema", null), new SPair("table", null), new SPair("column", null)]),
            PrimaryKeyPair = new BPair("primary", false),
            ReadOnlyPair = new BPair("readonly", false)
        };

        Assert.Equal(3, items.IdentifierPairs.Count);
        Assert.Equal("schema", items.IdentifierPairs[0].Key); Assert.Null(items.IdentifierPairs[0].Value);
        Assert.Equal("table", items.IdentifierPairs[1].Key); Assert.Null(items.IdentifierPairs[1].Value);
        Assert.Equal("column", items.IdentifierPairs[2].Key); Assert.Null(items.IdentifierPairs[2].Value);
        Assert.Equal("primary", items.PrimaryKeyPair.Value.Key);
        Assert.Equal("readonly", items.ReadOnlyPair.Value.Key);
    }

    //[Enforced]
    [Fact]
    public static void Test_Contains()
    {
        var engine = new FakeEngine();
        var items = new KnownPairs(engine)
        {
            IdentifierPairs = new IdentifierPairs(engine, [new SPair("schema", null), new SPair("table", null), new SPair("column", null)]),
            PrimaryKeyPair = new BPair("primary", false),
            ReadOnlyPair = new BPair("readonly", false)
        };

        Assert.True(items.Contains("SCHEMA"));
        Assert.True(items.Contains("TABLE"));
        Assert.True(items.Contains("COLUMN"));
        Assert.True(items.Contains("PRIMARY"));
        Assert.True(items.Contains("READONLY"));
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Identifiers()
    {
        var engine = new FakeEngine();
        var source = new KnownPairs(engine) { IdentifierPairs = new(engine, [new SPair("schema", null)]) };

        var target = source.WithIdentifierPairs(new IdentifierPairs(engine, [new SPair("schema", null), new SPair("table", null)]));
        Assert.NotSame(source, target);
        Assert.NotNull(target.IdentifierPairs);
        Assert.Equal(2, target.IdentifierPairs.Count);
        Assert.Equal("schema", target.IdentifierPairs[0].Key); Assert.Null(target.IdentifierPairs[0].Value);
        Assert.Equal("table", target.IdentifierPairs[1].Key); Assert.Null(target.IdentifierPairs[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_PrimaryKey()
    {
        var engine = new FakeEngine();
        var source = new KnownPairs(engine) { PrimaryKeyPair = new BPair("primary", false) };

        var target = source.WithPrimaryKeyPair(new BPair("primary", false));
        Assert.Same(source, target);

        target = source.WithPrimaryKeyPair(new BPair("other", true));
        Assert.NotSame(source, target);
        Assert.NotNull(target.PrimaryKeyPair);
        Assert.Equal("other", target.PrimaryKeyPair.Value.Key);
        Assert.True(target.PrimaryKeyPair.Value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_ReadOnly()
    {
        var engine = new FakeEngine();
        var source = new KnownPairs(engine) { ReadOnlyPair = new BPair("readonly", false) };

        var target = source.WithReadOnlyPair(new BPair("readonly", false));
        Assert.Same(source, target);

        target = source.WithReadOnlyPair(new BPair("other", true));
        Assert.NotSame(source, target);
        Assert.NotNull(target.ReadOnlyPair);
        Assert.Equal("other", target.ReadOnlyPair.Value.Key);
        Assert.True(target.ReadOnlyPair.Value.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
    }
}