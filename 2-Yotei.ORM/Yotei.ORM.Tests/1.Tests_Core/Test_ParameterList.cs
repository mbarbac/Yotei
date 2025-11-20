using Chain = Yotei.ORM.Code.ParameterList;
using Item = Yotei.ORM.Code.Parameter;

namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_ParameterList
{
    readonly static Item x007 = new("Id", "007");
    readonly static Item xJames = new("FirstName", "James");
    readonly static Item xBond = new("LastName", "Bond");
    readonly static Item xMi6 = new("Organization", "Mi6");
    readonly static Item xAge = new("Age", 55);

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        IEngine engine = new FakeEngine();
        var items = new Chain(engine); Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, []);
        Assert.Empty(items);

        items = new Chain(engine, [x007, xJames, xBond]);
        Assert.Equal(3, items.Count);
        Assert.Same(x007, items[0]);
        Assert.Same(xJames, items[1]);
        Assert.Same(xBond, items[2]);

        try { _ = new Chain(engine, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(engine, [null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Chain(engine, [x007, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Duplicates()
    {
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, [x007, x007]);
        Assert.Equal(2, items.Count);
        Assert.Same(x007, items[0]);
        Assert.Same(x007, items[1]);

        items = new Chain(engine, [x007, xJames, x007]);
        Assert.Equal(3, items.Count);
        Assert.Same(x007, items[0]);
        Assert.Same(xJames, items[1]);
        Assert.Same(x007, items[2]);

        try { _ = new Chain(engine, [x007, new Item("Id", null)]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Chain(engine, [x007, new Item("ID", null)]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, [x007, xJames, xBond, x007]);
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
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, [x007, xJames, xBond, x007]);

        Assert.Equal(-1, items.IndexOf("xAge"));

        Assert.Equal(0, items.IndexOf("Id"));
        Assert.Equal(0, items.IndexOf("ID"));

        Assert.Equal(3, items.LastIndexOf("Id"));
        Assert.Equal(3, items.LastIndexOf("ID"));

        var list = items.AllIndexesOf("Id");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = items.AllIndexesOf("ID");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, [x007, xJames, xBond, x007]);

        Assert.Equal(-1, items.IndexOf(x => x.Name.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => x.Name.Contains('d')));
        Assert.Equal(3, items.LastIndexOf(x => x.Name.Contains('d')));

        var list = items.AllIndexesOf(x => x.Name.Contains('d'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.GetRange(0, 0); Assert.Same(source, target);

        source = new Chain(engine, [x007, xJames, xBond, x007]);
        target = source.GetRange(0, 4);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);

        try { source.GetRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

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
    public static void Test_Replace_Same()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond]);
        var target = source.Replace(1, xJames);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Standard()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond]);
        var target = source.Replace(1, x007);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Equal(x007, target[1]);
        Assert.Same(xBond, target[2]);

        try { source.Replace(1, new Item("id", null)); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames]);
        var target = source.Add(xBond);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        try { _ = source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Duplicates()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames]);
        var target = source.Add(x007);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(x007, target[2]);

        try { _ = source.Add(new Item("id", null)); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddNew()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames]);
        var target = source.AddNew(99, out var item);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(item, target[2]);
        Assert.NotNull(item); Assert.Equal(99, item.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xBond, xMi6]);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(xMi6, target[3]);

        try { _ = source.AddRange([new Item("id", null)]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.AddRange([xAge, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames]);
        var target = source.Insert(2, xBond);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        try { _ = source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Duplicates()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames]);
        var target = source.Insert(2, x007);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(x007, target[2]);

        try { _ = source.Insert(0, new Item("id", null)); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertNew()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames]);
        
        var target = source.InsertNew(0, 99, out var item);        
        Assert.Equal(3, target.Count);
        Assert.Same(item, target[0]); Assert.Equal(99, item.Value);
        Assert.Same(x007, target[1]);
        Assert.Same(xJames, target[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames]);
        var target = source.InsertRange(2, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, [xBond, xMi6]);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(xMi6, target[3]);

        try { _ = source.InsertRange(0, [new Item("id", null)]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.InsertRange(0, [xAge, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);
        var target = source.RemoveAt(0);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        target = source.RemoveAt(2);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(x007, target[2]);

        try { source.RemoveAt(999); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 4);
        Assert.Empty(target);

        target = source.RemoveRange(0, 1);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        target = source.RemoveRange(3, 1);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { _ = source.RemoveRange(5, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { _ = source.RemoveRange(0, 5); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);
        var target = source.Remove("four");
        Assert.Same(source, target);

        target = source.Remove("id");
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        target = source.Remove("ID");
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_Last()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);
        var target = source.RemoveLast("id");
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        target = source.RemoveLast("ID");
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_All()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);
        var target = source.RemoveAll("id");
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);

        target = source.RemoveAll("ID");
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);
        var target = source.Remove(x => x.Name.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => x.Name.Contains('d'));
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate_Last()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);
        var target = source.RemoveLast(x => x.Name.Contains('d'));
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate_All()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);
        var target = source.RemoveAll(x => x.Name.Contains('d'));
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain(engine, [x007, xJames, xBond, x007]);
        target = source.Clear();
        Assert.Empty(target);
    }
}