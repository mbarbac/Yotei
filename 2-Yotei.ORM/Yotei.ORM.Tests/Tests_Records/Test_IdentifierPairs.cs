using TPair = System.Collections.Generic.KeyValuePair<string, string?>;
using IdentifierTags = Yotei.ORM.Records.Code.IdentifierPairs;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var items = new IdentifierTags(engine);

        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var items = new IdentifierTags(engine, new TPair("schema", null));

        Assert.Single(items);
        Assert.Equal("schema", items[0].Key); Assert.Null(items[0].Value);

        try { _ = new IdentifierTags(engine, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new IdentifierTags(engine, new TPair()); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierTags(engine, new TPair(null!, null)); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierTags(engine, new TPair(" ", null)); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierTags(engine, new TPair("a.b", null)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Multiple()
    {
        var engine = new FakeEngine();
        var items = new IdentifierTags(
            engine,
            [new TPair("schema", null), new TPair("table", null), new TPair("column", null)]);

        Assert.Equal(3, items.Count);
        Assert.Equal("schema", items[0].Key); Assert.Null(items[0].Value);
        Assert.Equal("table", items[1].Key); Assert.Null(items[1].Value);
        Assert.Equal("column", items[2].Key); Assert.Null(items[2].Value);

        try { _ = new IdentifierTags(engine, [new TPair("schema", null), new TPair()]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var items = new IdentifierTags(
            engine,
            [new TPair("schema", null), new TPair("table", null), new TPair("column", null)]);

        Assert.Equal(-1, items.IndexOf("none"));
        Assert.Equal(0, items.IndexOf("SCHEMA"));
        Assert.Equal(1, items.IndexOf("TABLE"));
        Assert.Equal(2, items.IndexOf("COLUMN"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(
            engine,
            [new TPair("schema", null), new TPair("table", null), new TPair("column", null)]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("schema", target[0].Key); Assert.Null(target[0].Value);
        Assert.Equal("table", target[1].Key); Assert.Null(target[1].Value);
        Assert.Equal("column", target[2].Key); Assert.Null(target[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(
            engine,
            [new TPair("schema", null), new TPair("table", null), new TPair("column", null)]);

        var target = source.GetRange(0, 0);
        Assert.Empty(target);

        target = source.GetRange(0, source.Count);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("table", target[0].Key); Assert.Null(target[0].Value);
        Assert.Equal("column", target[1].Key); Assert.Null(target[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(
            engine,
            [new TPair("schema", null), new TPair("table", null), new TPair("column", null)]);

        var target = source.Replace(0, new TPair("schema", null));
        Assert.Same(source, target);

        target = source.Replace(0, new TPair("SCHEMA", null));
        Assert.Same(source, target);

        target = source.Replace(0, new TPair("schema", "value"));
        Assert.NotSame(source, target);
        Assert.Equal("schema", target[0].Key); Assert.Equal("value", target[0].Value);
        Assert.Equal("table", target[1].Key); Assert.Null(target[1].Value);
        Assert.Equal("column", target[2].Key); Assert.Null(target[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(
            engine,
            [new TPair("schema", null), new TPair("table", null)]);

        var target = source.Add(new TPair("column", null));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("schema", target[0].Key); Assert.Null(target[0].Value);
        Assert.Equal("table", target[1].Key); Assert.Null(target[1].Value);
        Assert.Equal("column", target[2].Key); Assert.Null(target[2].Value);

        try { source.Add(new TPair()); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add(new TPair(null!, "any")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add(new TPair("TABLE", null)); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(engine, [new TPair("schema", null)]);

        var target = source.AddRange([new TPair("table", null), new TPair("column", null)]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("schema", target[0].Key); Assert.Null(target[0].Value);
        Assert.Equal("table", target[1].Key); Assert.Null(target[1].Value);
        Assert.Equal("column", target[2].Key); Assert.Null(target[2].Value);

        try { source.AddRange([new TPair("table", null), new TPair()]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(
            engine,
            [new TPair("schema", null), new TPair("table", null)]);

        var target = source.Insert(2, new TPair("column", null));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("schema", target[0].Key); Assert.Null(target[0].Value);
        Assert.Equal("table", target[1].Key); Assert.Null(target[1].Value);
        Assert.Equal("column", target[2].Key); Assert.Null(target[2].Value);

        try { source.Insert(2, new TPair()); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(2, new TPair(null!, "any")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(2, new TPair("TABLE", null)); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(engine, [new TPair("schema", null)]);

        var target = source.InsertRange(1, [new TPair("table", null), new TPair("column", null)]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("schema", target[0].Key); Assert.Null(target[0].Value);
        Assert.Equal("table", target[1].Key); Assert.Null(target[1].Value);
        Assert.Equal("column", target[2].Key); Assert.Null(target[2].Value);

        try { source.InsertRange(1, [new TPair("table", null), new TPair()]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(
            engine,
            [new TPair("schema", null), new TPair("table", null), new TPair("column", null)]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("table", target[0].Key); Assert.Null(target[0].Value);
        Assert.Equal("column", target[1].Key); Assert.Null(target[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(
            engine,
            [new TPair("schema", null), new TPair("table", null), new TPair("column", null)]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, source.Count);
        Assert.Empty(target);

        target = source.RemoveRange(0, 2);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("column", target[0].Key); Assert.Null(target[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(
            engine,
            [new TPair("schema", null), new TPair("table", null), new TPair("column", null)]);

        var target = source.Remove("none");
        Assert.Same(source, target);

        target = source.Remove("TABLE");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("schema", target[0].Key); Assert.Null(target[0].Value);
        Assert.Equal("column", target[1].Key); Assert.Null(target[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(
            engine,
            [new TPair("schema", null), new TPair("table", null), new TPair("column", null)]);

        var target = source.Clear();
        Assert.Empty(target);
    }
}