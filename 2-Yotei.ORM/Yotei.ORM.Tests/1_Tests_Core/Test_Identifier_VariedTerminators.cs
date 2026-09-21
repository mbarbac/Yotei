namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Identifier_VariedTerminators
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var chain = new Identifier(engine);
        Assert.Empty(chain);
        Assert.Null(chain.Value);
        Assert.Equal("", chain.ToString());

        chain = new Identifier(engine, []);
        Assert.Empty(chain);
        Assert.Null(chain.Value);
        Assert.Equal("", chain.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single_Empty()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var chain = new Identifier(engine, [null]);
        Assert.Single(chain);
        Assert.Null(chain[0]);
        Assert.Null(chain[0, true]);
        Assert.Null(chain.Value);
        Assert.Equal("", chain.ToString());

        chain = new Identifier(engine, [""]);
        Assert.Single(chain);
        Assert.Null(chain[0]);
        Assert.Null(chain[0, true]);
        Assert.Null(chain.Value);
        Assert.Equal("", chain.ToString());

        chain = new Identifier(engine, [" "]);
        Assert.Single(chain);
        Assert.Null(chain[0]);
        Assert.Null(chain[0, true]);
        Assert.Null(chain.Value);
        Assert.Equal("", chain.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Multi_Empty()
    {
        var engine = new FakeEngine() { IgnoreCase = true };

        var chain = new Identifier(engine, [" . . "]);
        Assert.Equal(3, chain.Count);
        Assert.Null(chain[0]); Assert.Null(chain[0, true]);
        Assert.Null(chain[1]); Assert.Null(chain[1, true]);
        Assert.Null(chain[2]); Assert.Null(chain[2, true]);
        Assert.Null(chain.Value);
        Assert.Equal("..", chain.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Multi_Populated()
    {
        var engine = new FakeEngine() { IgnoreCase = true };

        var chain = new Identifier(engine, [" . two . "]);
        Assert.Equal(3, chain.Count);
        Assert.Null(chain[0]); Assert.Null(chain[0, true]);
        Assert.Equal("[two]", chain[1]); Assert.Equal("two", chain[1, false]);
        Assert.Null(chain[2]); Assert.Null(chain[2, true]);
        Assert.Equal(".[two].", chain.Value);
        Assert.Equal(".[two].", chain.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var engine = new FakeEngine() { IgnoreCase = true };

        var chain = new Identifier(engine, [" ", " . three . "]);
        Assert.Equal(4, chain.Count);
        Assert.Null(chain[0]); Assert.Null(chain[0, true]);
        Assert.Null(chain[1]); Assert.Null(chain[1, true]);
        Assert.Equal("[three]", chain[2]); Assert.Equal("three", chain[2, false]);
        Assert.Null(chain[3]); Assert.Null(chain[3, true]);
        Assert.Equal("..[three].", chain.Value);
        Assert.Equal("..[three].", chain.ToString());

        chain = new Identifier(engine, ["one", "one"]);
        Assert.Equal(2, chain.Count);
        Assert.Equal("[one]", chain[0]); Assert.Equal("one", chain[0, false]);
        Assert.Equal("[one]", chain[1]); Assert.Equal("one", chain[1, false]);
        Assert.Equal("[one].[one]", chain.Value);
        Assert.Equal("[one].[one]", chain.ToString());

        try { new Identifier(engine, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    /*

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_With_Duplicates()
    {
        var chain = new Chain(false) { AcceptDuplicates = true };
        chain = (Chain)chain.AddRange([xone, xone]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);

        chain = new Chain(true);
        try { chain.AddRange([xone, new Named("ONE")]); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain(true) { AcceptDuplicates = true };
        chain = (Chain)chain.AddRange([xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Equal("ONE", ((Named)chain[1]).Name);
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
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
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

        source = new Chain(true, [xone, xtwo, xthree]);
        target = source.Replace(0, new Named("ONE"));
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Same()
    {
        var source = new Chain(true, [xone, xtwo, xthree]);
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        target = source.Replace(0, new Named("ONE"));
        Assert.Same(source, target);

        target = source.Replace(1, xtwo); Assert.Same(source, target);
        try { source.Replace(1, xone); Assert.Fail(); } catch (DuplicateException) { }

        source = new Chain(true) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xthree]);
        target = source.Replace(1, xtwo); Assert.Same(source, target);
        target = source.Replace(1, new Named("TWO")); Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty_Nested()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);

        var target = source.Replace(0, new Chain(false));
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Range_Nested()
    {
        var xalpha = new Named("alpha");
        var xbeta = new Named("beta");
        var source = new Chain(false, [xone, xtwo, xthree]);

        var target = source.Replace(0, new Chain(false, [xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xalpha, target[0]);
        Assert.Same(xbeta, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);

        target = source.Replace(1, new Chain(false, [xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xalpha, target[1]);
        Assert.Same(xbeta, target[2]);
        Assert.Same(xthree, target[3]);

        target = source.Replace(2, new Chain(false, [xalpha, xbeta]));
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
    public static void Test_IndexOf()
    {
        var source = new Chain(true) { AcceptDuplicates = true };
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
        var source = new Chain(true) { AcceptDuplicates = true };
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
        IElement item;
        List<IElement> range;
        var source = new Chain(true) { AcceptDuplicates = true };
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
    public static void Test_Add()
    {
        var source = new Chain(false);
        var target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        source = (Chain)target;
        target = source.Add(xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Add(xone); Assert.Fail(); } catch (DuplicateException) { }

        source = new Chain(false, [xone]) { IgnoreCase = true };
        try { source.Add(new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source = new Chain(false) { AcceptDuplicates = true };
        source = (Chain)source.Add(xone);

        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xone, target[1]);

        target = source.Add(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal("ONE", ((Named)target[1]).Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested()
    {
        var source = new Chain(false, [xone, xtwo]);
        var target = source.Add(new Chain(false, [xthree, xfour]));

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
        var source = new Chain(false, [xone, xtwo]);
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
        var source = new Chain(false, [xone, xtwo]);
        var target = source.AddRange([xthree, new Chain(false, [xfour, xfive])]);

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
        var source = new Chain(false);
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

        source = new(true, [xone]);
        try { source.Insert(0, new Named("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source = new(true) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xthree]);

        target = source.Insert(2, xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);
        Assert.Same(xthree, target[3]);

        source = (Chain)target;
        target = source.Insert(0, new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("ONE", ((Named)target[0]).Name);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xone, target[3]);
        Assert.Same(xthree, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Nested()
    {
        var source = new Chain(false, [xone, xtwo]);
        var target = source.Insert(2, new Chain(false, [xthree, xfour]));

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
        var source = new Chain(false, [xone, xtwo]);
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
        var source = new Chain(false, [xone, xtwo]);
        var target = source.InsertRange(1, [xthree, new Chain(false, [xfour, xfive])]);

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
        var source = new Chain(false, [xone, xtwo, xthree]);
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
        var source = new Chain(false, [xone, xtwo, xthree]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new Chain(false, [xone, xtwo, xthree]);
        var target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        source = new Chain(false, [xone, xtwo, xthree]);
        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        source = new Chain(false, [xone, xtwo, xthree]);
        try { source.RemoveRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(3, 1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(0, 4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(1, 3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(2, 2); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
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
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.Remove(new Named("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);

        source = new Chain(true) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);
        target = source.RemoveLast(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
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
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveAll(new Chain(true, [xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xthree, target[0]);
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
        var source = new Chain(false);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain(false, [xone, xtwo, xthree]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
     */
}