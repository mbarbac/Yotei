#pragma warning disable CA1859

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Identifier_Common
{
    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        
        var source = new Identifier(engine);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source, target);

        source = new Identifier(engine, "one..three");
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Null(target[1]);
        Assert.Equal("three", target[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Compare_Empty_To_Empty()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine);
        var target = new Identifier(engine);
        Assert.True(source.Equals(target));
        Assert.Equal(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare_From_Single()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine, "one");
        var target = new Identifier(engine);
        Assert.False(source.Equals(target)); Assert.NotEqual(source, target);
        Assert.False(target.Equals(source)); Assert.NotEqual(target, source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare_From_Multiple()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine, ["one", "two"]);

        var target = new Identifier(engine);
        Assert.False(source.Equals(target)); Assert.NotEqual(source, target);
        Assert.False(target.Equals(source)); Assert.NotEqual(target, source);

        target = new Identifier(engine, "one");
        Assert.False(source.Equals(target)); Assert.NotEqual(source, target);
        Assert.False(target.Equals(source)); Assert.NotEqual(target, source);

        target = new Identifier(engine, ["one", null]);
        Assert.False(source.Equals(target)); Assert.NotEqual(source, target);
        Assert.False(target.Equals(source)); Assert.NotEqual(target, source);

        target = new Identifier(engine, [null, "two"]);
        Assert.False(source.Equals(target)); Assert.NotEqual(source, target);
        Assert.False(target.Equals(source)); Assert.NotEqual(target, source);

        target = new Identifier(engine, ["one", "two"]);
        Assert.True(source.Equals(target)); Assert.Equal(source, target);
        Assert.True(target.Equals(source)); Assert.Equal(target, source);

        engine = new FakeEngine() { IgnoreCase = true };
        source = new Identifier(engine, ["one", "two"]);
        target = new Identifier(engine, ["ONE", "TWO"]);
        Assert.True(source.Equals(target)); Assert.Equal(source, target);
        Assert.True(target.Equals(source)); Assert.Equal(target, source);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Match_Empty()
    {
        var engine = new FakeEngine();
        var item = new Identifier(engine);
        Assert.True(item.Match(null));
        Assert.True(item.Match(""));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match_Populated()
    {
        IIdentifier item;
        var engine = new FakeEngine();

        item = new Identifier(engine); Assert.False(item.Match("any"));

        item = new Identifier(engine, "one");
        Assert.True(item.Match(null));
        Assert.True(item.Match("one"));

        item = new Identifier(engine, "one.two.three");
        Assert.True(item.Match(null));
        Assert.True(item.Match(".three"));
        Assert.True(item.Match("..three"));
        Assert.True(item.Match("two.three"));
        Assert.True(item.Match("one..three"));
        Assert.True(item.Match("one.two."));
        Assert.True(item.Match(".two."));

        item = new Identifier(engine, "one..three");
        Assert.True(item.Match(null));
        Assert.False(item.Match(".two."));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match_Populated_IgnoreCase()
    {
        IIdentifier item;
        var engine = new FakeEngine() { IgnoreCase = true };

        item = new Identifier(engine, "one"); Assert.True(item.Match("ONE"));

        item = new Identifier(engine, "one.two.three");
        Assert.True(item.Match(null));
        Assert.True(item.Match(".THREE"));
        Assert.True(item.Match("..THREE"));
        Assert.True(item.Match("TWO.THREE"));
        Assert.True(item.Match("ONE..THREE"));
        Assert.True(item.Match("ONE.TWO."));
        Assert.True(item.Match(".TWO."));

        item = new Identifier(engine, "one..three");
        Assert.True(item.Match(null));
        Assert.False(item.Match(".TWO."));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Contains()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var item = new Identifier(engine);
        Assert.False(item.Contains("any"));

        item = new Identifier(engine, "one.two.three");
        Assert.True(item.Contains("one"));
        Assert.True(item.Contains("TWO"));

        Assert.True(item.Contains(x => x is not null && x.Contains('h')));
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexes()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var item = new Identifier(engine);
        Assert.Equal(-1, item.IndexOf("any"));

        item = new Identifier(engine, "one.two.one.four");
        Assert.Equal(0, item.IndexOf("ONE"));
        Assert.Equal(2, item.LastIndexOf("ONE"));
        Assert.Equal([0, 2], item.IndexesOf("ONE"));

        Assert.Equal(0, item.IndexOf(x => x is not null && x.Contains('e')));
        Assert.Equal(2, item.LastIndexOf(x => x is not null && x.Contains('e')));
        Assert.Equal([0, 2], item.IndexesOf(x => x is not null && x.Contains('e')));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Reduce()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine);
        var target = source.Reduce();
        Assert.Same(source, target);

        source = new Identifier(engine, "one"); Assert.Equal(1, source.Count);
        target = source.Reduce();
        Assert.Same(source, target);

        source = new Identifier(engine, [null, "one"]); Assert.Equal(1, source.Count);
        target = source.Reduce();
        Assert.Same(source, target);

        source = new Identifier(engine, [null, "one"], reduce: false); Assert.Equal(2, source.Count);
        target = source.Reduce();
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("one", target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine, [null], reduce: false);
        var target = source.Replace(0, null, reduce: false);
        Assert.Same(source, target);

        source = new Identifier(engine, "one");
        target = source.Replace(0, "ONE");
        Assert.Same(source, target);

        source = new Identifier(engine, "one.two.three");
        target = source.Replace(1, "any.other");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("any", target[1]);
        Assert.Equal("other", target[2]);
        Assert.Equal("three", target[3]);

        target = source.Replace(0, null);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine);
        var target = source.Add(null);
        Assert.Same(source, target);

        target = source.Add(null, reduce: false);
        Assert.NotSame(source, target);
        Assert.Equal(string.Empty, target.ToStringEx(reduce: false, wrap: true));

        target = source.Add("one");
        Assert.NotSame(source, target);
        Assert.Equal("[one]", target.ToStringEx(reduce: false, wrap: true));

        source = new Identifier(engine, "one");
        target = source.Add("two..four");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Null(target[2]);
        Assert.Equal("four", target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine);

        var target = source.AddRange([null, null]);
        Assert.Same(source, target);

        target = source.AddRange([null, null], reduce: false);
        Assert.NotSame(source, target);
        Assert.Equal(".", target.ToStringEx(reduce: false));

        source = new Identifier(engine, "one");
        target = source.AddRange([null, "three"]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Null(target[1]);
        Assert.Equal("three", target[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine);
        var target = source.Insert(0, null);
        Assert.Same(source, target);

        target = source.Insert(0, null, reduce: false);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Null(target[0]);

        source = new Identifier(engine, "one");
        target = source.Insert(0, null);
        Assert.Same(source, target);

        target = source.Insert(0, null, reduce: false);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Null(target[0]);
        Assert.Equal("one", target[1]);

        target = source.Insert(1, null);
        Assert.NotSame(source, target);
        Assert.Equal("[one].", target.ToStringEx(reduce: false));
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine);

        var target = source.InsertRange(0, [null, null]);
        Assert.Same(source, target);

        target = source.InsertRange(0, [null, null], reduce: false);
        Assert.NotSame(source, target);
        Assert.Equal(".", target.ToStringEx(reduce: false));

        source = new Identifier(engine, "one.two");
        target = source.InsertRange(1, ["any", null, "other.another"]);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("any", target[1]);
        Assert.Null(target[2]);
        Assert.Equal("other", target[3]);
        Assert.Equal("another", target[4]);
        Assert.Equal("two", target[5]);
        Assert.Equal("[one].[any]..[other].[another].[two]", target.ToStringEx(reduce: false));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine() { IgnoreCase = true };

        var source = new Identifier(engine, "one");
        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(0, target.Count);

        source = new Identifier(engine, "one..three");
        target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[three]", target.ToStringEx(reduce: false));

        target = source.RemoveAt(0, reduce: false);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal(".[three]", target.ToStringEx(reduce: false));

        target = source.RemoveAt(2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[one].", target.ToStringEx(reduce: false));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine, "one..three.four");
        var target = source.RemoveRange(0, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[three].[four]", target.ToStringEx(reduce: false));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Part()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine, "one.two.one.four");
        var target = source.Remove("any");
        Assert.Same(source, target);

        target = source.Remove("one");
        Assert.NotSame(source, target);
        Assert.Equal("[two].[one].[four]", target.ToStringEx(reduce: false));

        target = source.RemoveLast("one");
        Assert.NotSame(source, target);
        Assert.Equal("[one].[two].[four]", target.ToStringEx(reduce: false));

        target = source.RemoveAll("ONE");
        Assert.NotSame(source, target);
        Assert.Equal("[two].[four]", target.ToStringEx(reduce: false));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine, "one.two.one.four");
        var target = source.Remove(x => x is null);
        Assert.Same(source, target);

        target = source.Remove(x => x is not null && x.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal("[two].[one].[four]", target.ToStringEx(reduce: false));

        target = source.RemoveLast(x => x is not null && x.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal("[one].[two].[four]", target.ToStringEx(reduce: false));

        target = source.RemoveAll(x => x is not null && x.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal("[two].[four]", target.ToStringEx(reduce: false));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Identifier(engine, "one.two.three");
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Equal(0, target.Count);
    }
}