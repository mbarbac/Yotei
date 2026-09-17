using Item = Yotei.ORM.Records.Code.Parameter;
using Chain = Yotei.ORM.Records.Code.ParameterList;

namespace Yotei.ORM.Records.Tests;

// ========================================================
//[Enforced]
public static class Test_ParameterList
{
    readonly static Item x007 = new("Id", "007");
    readonly static Item xJames = new("FirstName", "James");
    readonly static Item xBond = new("LastName", "Bond");
    readonly static Item xOrg = new("Organization", "Mi6");
    readonly static Item xAge = new("Age", 55);

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var chain = new Chain(engine);

        Assert.Empty(chain);
        Assert.Same(engine, chain.Engine);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var engine = new FakeEngine();
        var chain = new Chain(engine, []);
        Assert.Empty(chain);

        chain = new Chain(engine, [x007, xJames, xBond]);
        Assert.Equal(3, chain.Count);
        Assert.Same(x007, chain[0]);
        Assert.Same(xJames, chain[1]);
        Assert.Same(xBond, chain[2]);

        var xid = new Item("ID", "008");
        chain = new Chain(engine, [x007, xid]);
        Assert.Equal(2, chain.Count);
        Assert.Same(x007, chain[0]);
        Assert.Same(xid, chain[1]);

        try { _ = new Chain(engine, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain(engine, [null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain(engine, [x007, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_With_Duplicates()
    {
        var engine = new FakeEngine();
        var chain = new Chain(engine, [x007, x007]);
        Assert.Equal(2, chain.Count);
        Assert.Same(x007, chain[0]);
        Assert.Same(x007, chain[1]);

        var item = x007.Clone();
        try { _ = new Chain(engine, [x007, item]); Assert.Fail(); } catch (DuplicateException) { }

        engine = new FakeEngine() { IgnoreCase = true };
        var xid = new Item("ID", "008");
        try { _ = new Chain(engine, [x007, xid]); Assert.Fail(); } catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(x007, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Key()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine, [x007, xJames, xBond, x007]);

        var index = source.IndexOf("any"); Assert.Equal(-1, index);

        index = source.IndexOf("Id"); Assert.Equal(0, index);
        index = source.IndexOf("ID"); Assert.Equal(0, index);

        index = source.LastIndexOf("Id"); Assert.Equal(3, index);
        index = source.LastIndexOf("ID"); Assert.Equal(3, index);

        var nums = source.IndexesOf("Id");
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);

        nums = source.IndexesOf("ID");
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine, [x007, xJames, xBond, x007]);

        var index = source.IndexOf(x => x.Name is null); Assert.Equal(-1, index);

        index = source.IndexOf(x => x.Name.Contains('d'));
        Assert.Equal(0, index);

        index = source.LastIndexOf(x => x.Name.Contains('d'));
        Assert.Equal(3, index);

        var nums = source.IndexesOf(x => x.Name.Contains('d'));
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine, [x007, xJames, xBond, x007]);

        Assert.False(source.TryFind(x => x.Name is null, out var item));
        Assert.Null(item);

        Assert.True(source.TryFind(x => x.Name.Contains('d'), out item));
        Assert.Same(x007, item);

        Assert.True(source.TryFindLast(x => x.Name.Contains('d'), out item));
        Assert.Same(x007, item);

        Assert.True(source.TryFindAll(x => x.Name.Contains('d'), out var items));
        Assert.Equal(2, items.Count);
        Assert.Same(x007, items[0]);
        Assert.Same(x007, items[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine, [x007, xJames, xBond, x007]);

        var target = source.GetRange(0, 0);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);

        try { source.GetRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(0, -1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(5, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(0, 5); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);

        var target = source.Replace(0, x007);
        Assert.Same(source, target);

        var item = x007.Clone();
        try { source.Replace(0, item); Assert.Fail(); } catch (DuplicateException) { }

        target = source.Replace(0, xOrg);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xOrg, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(x007, target[3]);

        try { source.Replace(-1, xOrg); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Replace(4, xOrg); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine);

        var target = source.Add(x007);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(x007, target[0]);

        source = new Chain(engine, [x007, xJames]);
        target = source.Add(xBond);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        target = source.Add(x007);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(x007, target[2]);

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Add(x007.Clone()); Assert.Fail(); } catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddNew()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames]);

        var target = source.AddNew("any", out var item);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(item, target[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine);

        var target = source.AddRange([x007, xJames]);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Nested()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine);

        var target = source.AddRange(new Chain(engine, [x007, xJames]));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine);

        var target = source.Insert(0, x007);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(x007, target[0]);

        source = new Chain(engine, [x007, xJames]);
        target = source.Insert(2, xBond);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        target = source.Insert(2, x007);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(x007, target[2]);

        try { source.Insert(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Insert(0, x007.Clone()); Assert.Fail(); } catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertNew()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames]);

        var target = source.InsertNew(2, "any", out var item);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(item, target[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine);

        var target = source.InsertRange(0, [x007, xJames]);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Nested()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine);

        var target = source.InsertRange(0, new Chain(engine, [x007, xJames]));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        try { source.RemoveAt(-1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveAt(4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_Empty()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);

        var target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(x007, target[1]);

        try { source.RemoveRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(4, 1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(0, 5); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(1, 4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(2, 3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Key()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine, [x007, xJames, xBond, x007]);

        var target = source.Remove("any");
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

        target = source.RemoveAll("Id");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine, [x007, xJames, xBond, x007]);

        var target = source.Remove(x => x.Name.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => x.Name.Contains('d'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);
        Assert.Same(x007, target[2]);

        target = source.RemoveLast(x => x.Name.Contains('d'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);

        target = source.RemoveAll(x => x.Name.Contains('d'));
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
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine);

        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain(engine, [x007, xJames, xBond, x007]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}