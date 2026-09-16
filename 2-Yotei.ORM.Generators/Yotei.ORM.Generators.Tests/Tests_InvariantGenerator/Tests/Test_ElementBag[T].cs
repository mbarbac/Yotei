using Named = Yotei.ORM.InvariantGenerator.Tests.NamedElement;
using Chain = Yotei.ORM.InvariantGenerator.Tests.ElementBag_T;

namespace Yotei.ORM.InvariantGenerator.Tests;

// ========================================================
//[Enforced]
public static class Test_ElementBag_T
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
        var chain = new Chain(false);
        Assert.Empty(chain);

        Assert.True(chain.FlattenElements);
        Assert.False(chain.IgnoreCase);
        Assert.False(chain.AcceptDuplicates);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var chain = new Chain(false, []);
        Assert.Empty(chain);

        chain = new(false, [xone, xtwo, xthree]);
        Assert.Equal(3, chain.Count);
        Assert.Contains(xone, chain);
        Assert.Contains(xtwo, chain);
        Assert.Contains(xthree, chain);

        chain = new(false, [xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.True(chain.Contains(new Named("ONE")));

        try { _ = new Chain(false, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain(false, [xone, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain(false, [xone, xone]); Assert.Fail(); } catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_With_Duplicates()
    {
        var chain = new Chain(false) { AcceptDuplicates = true };
        chain = (Chain)chain.AddRange([xone, xone]);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.Contains(xone, chain);

        chain = new Chain(true);
        try { chain.AddRange([xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain(true) { AcceptDuplicates = true };
        chain = (Chain)chain.AddRange([xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Contains(xone, chain);
        Assert.True(chain.Contains(new Named("ONE")));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new Chain(true) { AcceptDuplicates = true, FlattenElements = false };
        Assert.True(source.IgnoreCase);
        Assert.True(source.AcceptDuplicates);
        Assert.False(source.FlattenElements);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.True(target.IgnoreCase);
        Assert.True(target.AcceptDuplicates);
        Assert.False(target.FlattenElements);

        source = new(false, [xone, xtwo]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xtwo, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        IElement item;
        List<IElement> range;
        var source = new Chain(true) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        Assert.False(source.TryFind(x => x is null, out item));
        Assert.Null(item);

        Assert.True(source.TryFind(x => x is Named named && named.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xone, item);

        Assert.True(source.TryFindAll(x => x is Named named && named.Name.Contains('e'), out range));
        Assert.Equal(3, range.Count);
        Assert.Same(xone, range[0]);
        Assert.Same(xone, range[1]);
        Assert.Same(xthree, range[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new Chain(false);
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

        source = new Chain(false, [xone]) { IgnoreCase = true };
        try { source.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source = new Chain(false) { AcceptDuplicates = true };
        source = (Chain)source.Add(xone);

        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xone, target);

        target = source.Add(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.True(target.Contains(new Named("ONE")));
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested()
    {
        var source = new Chain(false, [xone, xtwo]);
        var target = source.Add(new Chain(false, [xthree, xfour]));

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
        var source = new Chain(false, [xone, xtwo]);
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
        var source = new Chain(false, [xone, xtwo]);
        var target = source.AddRange([xthree, new Chain(false, [xfour, xfive])]);

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
    public static void Test_Remove()
    {
        var source = new Chain(true) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(xfour); Assert.Same(source, target);

        target = source.Remove(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        target = source.Remove(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        source = new Chain(true) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        target = source.RemoveAll(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xthree, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Nested()
    {
        var source = new Chain(true) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(new Chain(true, [xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

        target = source.RemoveAll(new Chain(true, [xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Contains(xthree, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain(true) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(x => x is Named named && named.Name is null);
        Assert.Same(source, target);

        target = source.Remove(x => x is Named named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Contains(xtwo, target);
        Assert.Contains(xone, target);
        Assert.Contains(xthree, target);

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
        var source = new Chain(false);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain(false, [xone, xtwo, xthree]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}