namespace Yotei.ORM.Tests.Core;

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
    public static void Test_Create_Range()
    {
        var engine = new FakeEngine();
        var items = new ParameterList(engine, []);
        Assert.Empty(items);

        items = new ParameterList(engine, [x007, xJames, xBond]);
        Assert.Equal(3, items.Count);
        Assert.Same(x007, items[0]);
        Assert.Same(xJames, items[1]);
        Assert.Same(xBond, items[2]);

        try { _ = new ParameterList(engine, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new ParameterList(engine, [null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new ParameterList(engine, [x007, null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Duplicates()
    {
        var engine = new FakeEngine();
        var items = new ParameterList(engine, [x007, x007]);
        Assert.Equal(2, items.Count);
        Assert.Same(x007, items[0]);
        Assert.Same(x007, items[1]);

        items = new ParameterList(engine, [x007, xJames, x007]);
        Assert.Equal(3, items.Count);
        Assert.Same(x007, items[0]);
        Assert.Same(xJames, items[1]);
        Assert.Same(x007, items[2]);

        try { _ = new ParameterList(engine, [x007, new Parameter("Id", null)]); Assert.Fail(); }
        catch (DuplicateException) { }

        engine = new FakeEngine() { CaseSensitiveNames = true };
        items = new ParameterList(engine, [x007, new Parameter("ID", null)]);
        Assert.Equal(2, items.Count);
        Assert.Same(x007, items[0]);
        Assert.Equal("ID", items[1].Name);
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Create_Extended()
    //{
    //    var engine = new FakeEngine();
    //    var other = new ParameterList(engine, [xJames, xBond]);
    //    var items = new ParameterList(engine, [x007, other]);

    //    Assert.Equal(3, items.Count);
    //    Assert.Same(x007, items[0]);
    //    Assert.Same(xJames, items[1]);
    //    Assert.Same(xBond, items[2]);
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var items = new ParameterList(engine, [x007, xJames, xBond, x007]);
        var target = items.Clone();

        Assert.NotSame(items, target);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(x007, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var items = new ParameterList(engine, [x007, xJames, xBond, x007]);

        Assert.Equal(-1, items.IndexOf("xAge"));

        Assert.Equal(0, items.IndexOf("Id"));
        Assert.Equal(0, items.IndexOf("ID"));

        Assert.Equal(3, items.LastIndexOf("Id"));
        Assert.Equal(3, items.LastIndexOf("ID"));

        var list = items.IndexesOf("Id");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = items.IndexesOf("ID");
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

        Assert.Equal(-1, items.IndexOf(x => ((Parameter)x).Name.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => ((Parameter)x).Name.Contains('d')));
        Assert.Equal(3, items.LastIndexOf(x => ((Parameter)x).Name.Contains('d')));

        var list = items.IndexesOf(x => ((Parameter)x).Name.Contains('d'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond, x007]);
        var target = source.GetRange(0, 0);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);

        try { source.GetRange(-1, 0); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { source.GetRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { source.GetRange(5, 0); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.GetRange(0, 5); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond]);
        var target = source.Replace(1, xJames);
        Assert.Same(source, target);

        target = source.Replace(1, x007);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Equal(x007, target[1]);
        Assert.Same(xBond, target[2]);

        try { source.Replace(1, new Parameter("Id", null)); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Replace_Extended()
    //{
    //    var engine = new FakeEngine();
    //    var source = new ParameterList(engine, [x007, xJames, xBond]);
    //    var other = new ParameterList(engine, [xMi6, xAge]);

    //    var target = source.Replace(1, other);
    //    Assert.NotSame(source, target);
    //    Assert.Equal(4, target.Count);
    //    Assert.Same(x007, target[0]);
    //    Assert.Equal(xMi6, target[1]);
    //    Assert.Same(xAge, target[2]);
    //    Assert.Same(xBond, target[3]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Replace_Extended_Empty()
    //{
    //    var engine = new FakeEngine();
    //    var source = new ParameterList(engine, [x007, xJames, xBond]);
    //    var other = new ParameterList(engine);

    //    var target = source.Replace(1, other);
    //    Assert.Same(source, target);

    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames]);
        var target = source.Add(xBond);

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        try { _ = source.Add(new Parameter("Id", 99)); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Add(new Parameter("", 99)); Assert.Fail(); }
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
    public static void Test_Add_Duplicates()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames]);
        var target = source.Add(x007);

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(x007, target[2]);

        source = new ParameterList(engine, [x007, xJames]);
        try { _ = source.Add(new Parameter("Id", 99)); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Add_Extended()
    //{
    //    var engine = new FakeEngine();
    //    var source = new ParameterList(engine, [x007, xJames, xBond]);
    //    var other = new ParameterList(engine, []);

    //    var target = source.Add(other);
    //    Assert.Same(source, target);

    //    other = new ParameterList(engine, [xMi6, xAge]);
    //    target = source.Add(other);
    //    Assert.NotSame(source, target);
    //    Assert.Equal(5, target.Count);
    //    Assert.Same(x007, target[0]);
    //    Assert.Equal(xJames, target[1]);
    //    Assert.Same(xBond, target[2]);
    //    Assert.Same(xMi6, target[3]);
    //    Assert.Same(xAge, target[4]);
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xBond, xMi6]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(xMi6, target[3]);

        try { _ = source.AddRange([new Parameter("Id", 99)]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.AddRange([xAge, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.AddRange([xAge, new Parameter("", 99)]); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_AddRange_Extended()
    //{
    //    var engine = new FakeEngine();
    //    var source = new ParameterList(engine, [x007, xJames]);
    //    var other = new ParameterList(engine, []);

    //    var target = source.AddRange([other]);
    //    Assert.Same(source, target);

    //    other = new ParameterList(engine, [xMi6, xAge]);
    //    target = source.AddRange([xBond, other]);
    //    Assert.NotSame(source, target);
    //    Assert.Equal(5, target.Count);
    //    Assert.Same(x007, target[0]);
    //    Assert.Equal(xJames, target[1]);
    //    Assert.Same(xBond, target[2]);
    //    Assert.Same(xMi6, target[3]);
    //    Assert.Same(xAge, target[4]);
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames]);
        var target = source.Insert(2, xBond);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        try { _ = source.Insert(0, new Parameter("Id", 99)); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Insert(0, new Parameter("", 99)); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Duplicates()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames]);
        var target = source.Insert(2, x007);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(x007, target[2]);

        source = new ParameterList(engine, [x007, xJames]);
        try { _ = source.Insert(0, new Parameter("Id", 99)); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Insert_Extended()
    //{
    //    var engine = new FakeEngine();
    //    var source = new ParameterList(engine, [x007, xJames, xBond]);
    //    var other = new ParameterList(engine, []);

    //    var target = source.Insert(3, other);
    //    Assert.Same(source, target);

    //    other = new ParameterList(engine, [xMi6, xAge]);
    //    target = source.Insert(3, other);
    //    Assert.NotSame(source, target);
    //    Assert.Equal(5, target.Count);
    //    Assert.Same(x007, target[0]);
    //    Assert.Equal(xJames, target[1]);
    //    Assert.Same(xBond, target[2]);
    //    Assert.Same(xMi6, target[3]);
    //    Assert.Same(xAge, target[4]);
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames]);
        var target = source.InsertRange(2, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, [xBond, xMi6]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(xMi6, target[3]);

        try { _ = source.InsertRange(0, [new Parameter("Id", 99)]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.InsertRange(0, [xAge, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.InsertRange(0, [xAge, new Parameter("", 99)]); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_InsertRange_Extended()
    //{
    //    var engine = new FakeEngine();
    //    var source = new ParameterList(engine, [x007, xJames]);
    //    var other = new ParameterList(engine, []);

    //    var target = source.InsertRange(2, [other]);
    //    Assert.Same(source, target);

    //    other = new ParameterList(engine, [xMi6, xAge]);
    //    target = source.InsertRange(2, [xBond, other]);
    //    Assert.NotSame(source, target);
    //    Assert.Equal(5, target.Count);
    //    Assert.Same(x007, target[0]);
    //    Assert.Equal(xJames, target[1]);
    //    Assert.Same(xBond, target[2]);
    //    Assert.Same(xMi6, target[3]);
    //    Assert.Same(xAge, target[4]);
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond, x007]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        try { source.RemoveAt(999); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond, x007]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 4);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = source.RemoveRange(5, 0); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = source.RemoveRange(0, 5); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond, x007]);
        var target = source.Remove("four");
        Assert.Same(source, target);

        target = source.Remove("Id");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        target = source.Remove("ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        target = source.RemoveLast("Id");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        target = source.RemoveLast("ID");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        target = source.RemoveAll("Id");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);

        target = source.RemoveAll("ID");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Remove_Item_Extended()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var source = new ParameterList(engine, [x007, xJames, xBond, x007]);
        var target = source.Remove(x => ((Parameter)x).Name.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => ((Parameter)x).Name.Contains('d'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        target = source.RemoveLast(x => ((Parameter)x).Name.Contains('d'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        target = source.RemoveAll(x => ((Parameter)x).Name.Contains('d'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
    }

    // ----------------------------------------------------

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
    }
}