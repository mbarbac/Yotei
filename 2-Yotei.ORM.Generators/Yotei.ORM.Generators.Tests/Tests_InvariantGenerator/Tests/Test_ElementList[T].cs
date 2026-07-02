using Named = Yotei.ORM.InvariantGenerator.Tests.NamedElement;
using Chain = Yotei.ORM.InvariantGenerator.Tests.ElementList_T;

namespace Yotei.ORM.InvariantGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_ElementList_T
{
    readonly static Named xone = new("one");
    readonly static Named xtwo = new("two");
    readonly static Named xthree = new("three");
    readonly static Named xfour = new("four");
    readonly static Named xfive = new("five");

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var chain = new Chain(engine);
        Assert.Empty(chain);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var engine = new FakeEngine();
        var chain = new Chain(engine, []);
        Assert.Empty(chain);

        chain = new Chain(engine, [xone, xtwo, xthree]);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new Chain(engine, [xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Equal("ONE", ((Named)chain[1]).Name);

        try { _ = new Chain(engine, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain(engine, [xone, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain(engine, [xone, xone]); Assert.Fail(); } catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_With_Duplicates()
    {
        var engine = new FakeEngine();
        var chain = new Chain(engine) { AcceptDuplicates = true };

        chain = (Chain)chain.AddRange([xone, xone]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Nested()
    {
        var engine = new FakeEngine();
        var chain = new Chain(engine) { AcceptDuplicates = true };
        var other = new Chain(engine, [xtwo, xthree]);

        chain = (Chain)chain.AddRange([xone, other]);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Empty(target);

        engine = new FakeEngine() { IgnoreCase = true };
        source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.IsType<Chain>(target);
        Assert.True(((Chain)target).AcceptDuplicates);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Value()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xthree, xone]);

        var index = source.IndexOf(xfour); Assert.Equal(-1, index);

        index = source.IndexOf(xone); Assert.Equal(0, index);
        index = source.IndexOf(new Named("ONE")); Assert.Equal(0, index);

        index = source.LastIndexOf(xone); Assert.Equal(3, index);
        index = source.LastIndexOf(new Named("ONE")); Assert.Equal(3, index);

        var nums = source.IndexesOf(xone);
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);

        nums = source.IndexesOf(new Named("ONE"));
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var index = source.IndexOf(x => x is null); Assert.Equal(-1, index);

        index = source.IndexOf(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(0, index);

        index = source.LastIndexOf(x => x is Named named && named.Name.Contains('n'));
        Assert.Equal(2, index);

        var nums = source.IndexesOf(x => x is Named named && named.Name.Contains('e'));
        Assert.Equal(3, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(2, nums[1]);
        Assert.Equal(3, nums[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        IElement item;
        List<IElement> range;
        var source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        Assert.False(source.TryFind(x => x is null, out item));
        Assert.Null(item);

        Assert.True(source.TryFind(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xone, item);

        Assert.True(source.TryFindLast(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xthree, item);

        Assert.True(source.TryFindAll(x => x is Named named && named.Name.Contains('e'), out range));
        Assert.Equal(3, range.Count);
        Assert.Same(xone, range[0]);
        Assert.Same(xone, range[1]);
        Assert.Same(xthree, range[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.GetRange(0, 0);
        Assert.Same(source, target);

        source = new Chain(engine, [xone, xtwo, xthree]);
        target = source.GetRange(0, 3);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        try { source.GetRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(0, -1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(4, 0); Assert.Fail(); } catch (ArgumentException) { }
        try { source.GetRange(0, 4); Assert.Fail(); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        target = source.Replace(0, new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("ONE", ((Named)target[0]).Name);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        var xother = new Named("other");
        try { source.Replace(-1, xother); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Replace(3, xother); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Same()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        try { source.Replace(1, xone); Assert.Fail(); } catch (DuplicateException) { }

        source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xthree]);
        target = source.Replace(1, xtwo);
        Assert.Same(source, target);

        target = source.Replace(1, new Named("TWO"));
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty_Nested()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.Replace(0, new Chain(engine));
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Range_Nested()
    {
        var engine = new FakeEngine();
        var xalpha = new Named("alpha");
        var xbeta = new Named("beta");
        var source = new Chain(engine, [xone, xtwo, xthree]);

        var target = source.Replace(0, new Chain(engine, [xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xalpha, target[0]);
        Assert.Same(xbeta, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);

        target = source.Replace(1, new Chain(engine, [xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xalpha, target[1]);
        Assert.Same(xbeta, target[2]);
        Assert.Same(xthree, target[3]);

        target = source.Replace(2, new Chain(engine, [xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xalpha, target[2]);
        Assert.Same(xbeta, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        source = new Chain(engine, [xone]);
        target = source.Add(xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Add(xone); Assert.Fail(); } catch (DuplicateException) { }

        engine = new FakeEngine() { IgnoreCase = true };
        source = new Chain(engine, [xone, xtwo]);
        try { source.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source.AcceptDuplicates = true;
        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        target = source.Add(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Equal("ONE", ((Named)target[2]).Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.Add(new Chain(engine, [xthree, xfour]));

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Nested()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.AddRange([xthree, new Chain(engine, [xfour, xfive])]);

        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.Insert(0, xone);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        source = (Chain)target;
        target = source.Insert(1, xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        source = (Chain)target;
        target = source.Insert(0, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xthree, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);

        try { source.Insert(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Insert(0, xone); Assert.Fail(); } catch (DuplicateException) { }
        try { source.Insert(-1, xfive); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Insert(4, xfive); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }

        engine = new FakeEngine() { IgnoreCase = true };
        source = new Chain(engine, [xthree, xone, xtwo]);
        try { source.Insert(0, new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source = (Chain)target;
        source.AcceptDuplicates = true;
        target = source.Insert(3, xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xthree, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xone, target[3]);

        source = (Chain)target;
        target = source.Insert(0, new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("ONE", ((Named)target[0]).Name);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);
        Assert.Same(xtwo, target[3]);
        Assert.Same(xone, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Nested()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.Insert(2, new Chain(engine, [xthree, xfour]));

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.InsertRange(0, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, [xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Nested()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.InsertRange(1, [xthree, new Chain(engine, [xfour, xfive])]);

        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xfour, target[2]);
        Assert.Same(xfive, target[3]);
        Assert.Same(xtwo, target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.RemoveAt(0);

        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        try { source.RemoveAt(-1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveAt(3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_Empty()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        try { source.RemoveRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(3, 1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(0, 4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(1, 3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(2, 2); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Value()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(xfour);
        Assert.Same(source, target);

        target = source.Remove(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Value_IgnoreCase()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.RemoveAll(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Value_Nested()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(new Chain(engine, [xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveAll(new Chain(engine, [xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xthree, target[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(x => x is Named named && named.Name is null);
        Assert.Same(source, target);

        target = source.Remove(x => x is Named named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(x => x is Named named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => x is Named named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain(engine, [xone, xtwo, xthree]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}