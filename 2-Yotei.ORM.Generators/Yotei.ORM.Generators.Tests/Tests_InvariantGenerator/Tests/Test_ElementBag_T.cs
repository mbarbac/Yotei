using Named = Yotei.ORM.InvariantGenerator.Tests.NamedElement;
using Chain = Yotei.ORM.InvariantGenerator.Tests.ElementBag_T;

namespace Yotei.ORM.InvariantGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_ElementBag_T
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
        Assert.Contains(xone, chain);
        Assert.Contains(xtwo, chain);
        Assert.Contains(xthree, chain);

        chain = new Chain(engine, [xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.True(chain.Contains(x => x is Named named && named.Name == "ONE"));

        try { _ = new Chain(null!); Assert.Fail(); } catch (ArgumentNullException) { }
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
        Assert.Contains(xone, chain);
        Assert.Contains(xone, chain);

        engine = new FakeEngine() { IgnoreCase = true };
        chain = new Chain(engine);
        try { chain.AddRange([xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain(engine) { AcceptDuplicates = true };
        chain = (Chain)chain.AddRange([xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.True(chain.Contains(x => x is Named named && named.Name == "ONE"));
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
        source = new Chain(engine, [xone, xtwo]) { AcceptDuplicates = true };
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.IsType<Chain>(target);
        Assert.True(((Chain)target).AcceptDuplicates);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        IElement item;
        List<IElement> range;

        var chain = new Chain(engine) { AcceptDuplicates = true };
        chain = (Chain)chain.AddRange([xone, xtwo, xone, xthree]);

        Assert.False(chain.Find(x => x is null, out item));
        Assert.Null(item);

        Assert.True(chain.Find(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Contains(item, chain);

        Assert.True(chain.FindAll(x => x is Named named && named.Name.Contains('e'), out range));
        Assert.Equal(3, range.Count);
        Assert.Contains(xone, range);
        Assert.Contains(xthree, range);
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
        Assert.Contains(xone, target);

        source = (Chain)target;
        target = source.Add(xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Add(xone); Assert.Fail(); } catch (DuplicateException) { }

        engine = new FakeEngine() { IgnoreCase = true };
        source = new Chain(engine, [xone]);
        try { source.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source.AcceptDuplicates = true;
        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xone, target);

        source = (Chain)target;
        target = source.Add(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xone, target);
        Assert.True(target.Contains(x => x is Named named && named.Name == "ONE"));
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
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
        Assert.Contains(xfour, target);
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
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
        Assert.Contains(xfour, target);
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
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
        Assert.Contains(xfour, target);
        Assert.Contains(xfive, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Value()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(xfour);
        Assert.Same(source, target);

        target = source.Remove(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);
        target = source.RemoveAll(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
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
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
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
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        target = source.RemoveAll(new Chain(engine, [xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Contains(xthree, target);
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
        Assert.Contains(xtwo, target);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        target = source.RemoveAll(x => x is Named named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
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