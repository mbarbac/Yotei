﻿namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_ParameterList
{
    readonly static Parameter x007 = new("Id", "007");
    readonly static Parameter xJames = new("FirstName", "James");
    readonly static Parameter xBond = new("LastName", "Bond");
    readonly static Parameter xMi6 = new("Organization", "Mi6");
    readonly static Parameter xAge = new("Age", 55);

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var items = new ParameterList(engine);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var items = new ParameterList(engine, [x007]);
        Assert.Single(items);
        Assert.Same(x007, items[0]);

        try { _ = new ParameterList(engine, [null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var engine = new FakeEngine();
        var items = new ParameterList(engine, []);
        Assert.Empty(items);

        items = new ParameterList(engine, [x007, xJames, xBond]);
        Assert.Equal(3, items.Count);
        Assert.Same(x007, items[0]);
        Assert.Same(xJames, items[1]);
        Assert.Same(xBond, items[2]);

        try { _ = new ParameterList(engine, (IEnumerable<IParameter>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new ParameterList(engine, [x007, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_With_Duplicates()
    {
        var engine = new FakeEngine();
        var items = new ParameterList(engine, [x007, x007]);
        Assert.Equal(2, items.Count);
        Assert.Same(x007, items[0]);
        Assert.Same(x007, items[1]);

        try { _ = new ParameterList(engine, [x007, new Parameter("id", null)]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new ParameterList(engine, [x007, new Parameter("ID", null)]); Assert.Fail(); }
        catch (DuplicateException) { }

        engine = new FakeEngine() { CaseSensitiveNames = true };
        items = new ParameterList(engine, [x007, new Parameter("ID", null)]);
        Assert.Same(x007, items[0]);
        Assert.Equal("ID", items[1].Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Item()
    {
        var engine = new FakeEngine();
        var items = new ParameterList(engine, [x007, xJames, xBond, x007]);

        Assert.Equal(-1, items.IndexOf("any"));

        Assert.Equal(0, items.IndexOf("ID"));
        Assert.Equal(3, items.LastIndexOf("ID"));

        var list = items.IndexesOf("ID");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var engine = new FakeEngine();
        var items = new ParameterList(engine, [x007, xJames, xBond, x007]);

        Assert.Equal(-1, items.IndexOf(x => x.Name.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => x.Name.Contains('d')));
        Assert.Equal(3, items.LastIndexOf(x => x.Name.Contains('d')));

        var list = items.IndexesOf(x => x.Name.Contains('d'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond, x007]);

        var target = source.GetRange(0, 0);
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Equal(4, source.Count);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Equal(4, source.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond]);

        var target = source.Replace(0, x007);
        Assert.Same(source, target);

        target = source.Replace(0, new Parameter("Id", 99));
        Assert.NotSame(source, target);
        Assert.NotSame(x007, target[0]);
        Assert.Equal("Id", target[0].Name);
        Assert.Equal(3, source.Count);

        try { source.Replace(1, new Parameter("ID", 99)); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Replace(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Replace(0, new Parameter("", 99)); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond]);

        var target = source.Add(xMi6);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(xMi6, target[3]);
        Assert.Equal(3, source.Count);

        target = source.Add(x007);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(x007, target[3]);
        Assert.Equal(3, source.Count);

        try { source.Add(new Parameter("Id", 99)); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(new Parameter("", 99)); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddNew()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond]);

        var target = source.AddNew(99, out var item);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(item, target[3]);
        Assert.Equal(source.NextName(), item!.Name);
        Assert.Equal(99, item.Value);
        Assert.Equal(3, source.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xMi6, xAge]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(xMi6, target[3]);
        Assert.Same(xAge, target[4]);
        Assert.Equal(3, source.Count);

        try { source.AddRange(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddRange([null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.AddRange([new Parameter("", 99)]); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond]);

        var target = source.Insert(3, xMi6);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(xMi6, target[3]);
        Assert.Equal(3, source.Count);

        target = source.Insert(3, x007);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(x007, target[3]);
        Assert.Equal(3, source.Count);

        try { source.Insert(3, new Parameter("Id", 99)); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Insert(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(3, new Parameter("", 99)); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void InsertRange()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond]);

        var target = source.InsertRange(3, []);
        Assert.Same(source, target);

        target = source.InsertRange(3, [xMi6, xAge]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(xMi6, target[3]);
        Assert.Same(xAge, target[4]);
        Assert.Equal(3, source.Count);

        try { source.InsertRange(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.InsertRange(3, [null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.InsertRange(3, [new Parameter("", 99)]); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Equal(3, source.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(1, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(x007, target[0]);
        Assert.Equal(3, source.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond, x007]);

        var target = source.Remove("any");
        Assert.Same(source, target);

        target = source.Remove("ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);
        Assert.Equal(4, source.Count);

        target = source.RemoveLast("ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Equal(4, source.Count);

        target = source.RemoveAll("ID");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Equal(4, source.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond, x007]);

        var target = source.Remove(x => x.Name.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => x.Name.Contains('d'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);
        Assert.Equal(4, source.Count);

        target = source.RemoveLast(x => x.Name.Contains('d'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Equal(4, source.Count);

        target = source.RemoveAll(x => x.Name.Contains('d'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Equal(4, source.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine);

        var target = source.Clear();
        Assert.Same(source, target);

        source = new ParameterList(engine, [x007, xJames, xBond, x007]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Equal(4, source.Count);
    }
}