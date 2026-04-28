#pragma warning disable IDE0018

using IItem = Yotei.ORM.Generators.InvariantGenerator.Tests.IElement;
using TItem = Yotei.ORM.Generators.InvariantGenerator.Tests.NamedElement;
using IHost = Yotei.ORM.Generators.InvariantGenerator.Tests.IElementList_T;
using THost = Yotei.ORM.Generators.InvariantGenerator.Tests.ElementList_T;
using System.Runtime.Serialization;

namespace Yotei.ORM.Generators.InvariantGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_ElementList_T
{
    readonly static TItem xone = new("one");
    readonly static TItem xtwo = new("two");
    readonly static TItem xthree = new("three");
    readonly static TItem xfour = new("four");
    readonly static TItem xfive = new("five");

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var host = new THost(engine);
        Assert.Empty(host);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var engine = new FakeEngine();
        var host = new THost(engine, []);
        Assert.Empty(host);

        host = new THost(engine, [xone, xtwo, xthree]);
        Assert.Equal(3, host.Count);
        Assert.Same(xone, host[0]);
        Assert.Same(xtwo, host[1]);
        Assert.Same(xthree, host[2]);

        host = new THost(engine.WithIgnoreCase(false), [xone, new TItem("ONE")]);

        Assert.Equal(2, host.Count);
        Assert.Same(xone, host[0]);
        Assert.Equal("ONE", ((TItem)host[1]).Name);

        try { _ = new THost(engine, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new THost(engine, [xone, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new THost(engine, [xone, xone]); Assert.Fail(); } catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_With_Duplicates()
    {
        var engine = new FakeEngine();
        var host = new THost(engine) { AllowDuplicates = true };
        host = (THost)host.AddRange([xone, xone]);
        Assert.Equal(2, host.Count);
        Assert.Same(xone, host[0]);
        Assert.Same(xone, host[1]);

        host = new THost(engine.WithIgnoreCase(true));
        try { host.AddRange([xone, new TItem("ONE")]); Assert.Fail(); } catch (DuplicateException) { }

        host = new THost(engine.WithIgnoreCase(true)) { AllowDuplicates = true };
        host = (THost)host.AddRange([xone, new TItem("ONE")]);
        Assert.Equal(2, host.Count);
        Assert.Same(xone, host[0]);
        Assert.Equal("ONE", ((TItem)host[1]).Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new THost(engine);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Empty(target);

        source = new THost(engine) { AllowDuplicates = true };
        source = (THost)source.AddRange([xone, xtwo]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.IsType<THost>(target);
        Assert.True(((THost)target).AllowDuplicates);
        Assert.Same(target.Engine, source.Engine);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine() { IgnoreCase = false };
        var source = new THost(engine, [xone, xtwo, xthree]);
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        target = source.Replace(0, new TItem("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("ONE", ((TItem)target[0]).Name);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        var xother = new TItem("other");
        try { source.Replace(-1, xother); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Replace(3, xother); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Same()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [xone, xtwo, xthree]);
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        try { source.Replace(1, xone); Assert.Fail(); } catch (DuplicateException) { }

        source = new THost(engine.WithIgnoreCase(true)) { AllowDuplicates = true };
        source = (THost)source.AddRange([xone, xtwo, xthree]);
        target = source.Replace(1, xtwo);
        Assert.Same(source, target);

        target = source.Replace(1, new TItem("TWO"));
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty_Nested()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, [xone, xtwo, xthree]);
        var target = source.Replace(0, new THost(engine));
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Range_Nested()
    {
        var engine = new FakeEngine();
        var xalpha = new TItem("alpha");
        var xbeta = new TItem("beta");
        var source = new THost(engine, [xone, xtwo, xthree]);

        var target = source.Replace(0, new THost(engine, [xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xalpha, target[0]);
        Assert.Same(xbeta, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);

        target = source.Replace(1, new THost(engine, [xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xalpha, target[1]);
        Assert.Same(xbeta, target[2]);
        Assert.Same(xthree, target[3]);

        target = source.Replace(2, new THost(engine, [xalpha, xbeta]));
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
    public static void Test_IndexOf_Value()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new THost(engine) { AllowDuplicates = true };
        source = (THost)source.AddRange([xone, xtwo, xthree, xone]);

        var index = source.IndexOf(xfour); Assert.Equal(-1, index);

        index = source.IndexOf(xone); Assert.Equal(0, index);
        index = source.IndexOf(new TItem("ONE")); Assert.Equal(0, index);

        index = source.LastIndexOf(xone); Assert.Equal(3, index);
        index = source.LastIndexOf(new TItem("ONE")); Assert.Equal(3, index);

        var nums = source.IndexesOf(xone);
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);

        nums = source.IndexesOf(new TItem("ONE"));
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var engine = new FakeEngine();
        var source = new THost(engine) { AllowDuplicates = true };
        source = (THost)source.AddRange([xone, xtwo, xone, xthree]);

        var index = source.IndexOf(x => x is null); Assert.Equal(-1, index);

        index = source.IndexOf(x => x is TItem TItem && TItem.Name.Contains('n'));
        Assert.Equal(0, index);

        index = source.LastIndexOf(x => x is TItem TItem && TItem.Name.Contains('n'));
        Assert.Equal(2, index);

        var nums = source.IndexesOf(x => x is TItem TItem && TItem.Name.Contains('e'));
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
        var engine = new FakeEngine();
        IElement item;
        List<IElement> range;
        var source = new THost(engine) { AllowDuplicates = true };
        source = (THost)source.AddRange([xone, xtwo, xone, xthree]);

        Assert.False(source.Find(x => x is null, out item));
        Assert.Null(item);

        Assert.True(source.Find(x => x is TItem TItem && TItem.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xone, item);

        Assert.True(source.FindLast(x => x is TItem TItem && TItem.Name.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xthree, item);

        Assert.True(source.FindAll(x => x is TItem TItem && TItem.Name.Contains('e'), out range));
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
        var engine = new FakeEngine();
        var source = new THost(engine);
        var target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        source = new THost(engine, [xone]);
        target = source.Add(xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Add(xone); Assert.Fail(); } catch (DuplicateException) { }

        engine = new FakeEngine() { IgnoreCase = true };
        source = new THost(engine, [xone]);
        try { source.Add(new TItem("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source = (THost)source.WithAllowDuplicates(true);
        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xone, target[1]);

        target = source.Add(new TItem("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal("ONE", ((TItem)target[1]).Name);
    }
}/*

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested()
    {
        var engine = new FakeEngine();
        var source = new THost(engine,[xone, xtwo]);
        var target = source.Add(new THost(engine,[xthree, xfour]));

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
        var source = new THost(engine,[xone, xtwo]);
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
        var source = new THost(engine,[xone, xtwo]);
        var target = source.AddRange([xthree, new THost(engine,[xfour, xfive])]);

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
        var source = new THost(engine);
        var target = source.Insert(0, xone);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        source = (THost)target;
        target = source.Insert(1, xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        source = (THost)target;
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

        source.IgnoreCase = true;
        try { source.Insert(0, new TItem("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source = (THost)target;
        source.AllowDuplicates = true;
        target = source.Insert(3, xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xthree, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xone, target[3]);

        source = (THost)target;
        target = source.Insert(0, new TItem("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("ONE", ((TItem)target[0]).Name);
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
        var source = new THost(engine,[xone, xtwo]);
        var target = source.Insert(2, new THost(engine,[xthree, xfour]));

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
        var source = new THost(engine,[xone, xtwo]);
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
        var source = new THost(engine,[xone, xtwo]);
        var target = source.InsertRange(1, [xthree, new THost(engine,[xfour, xfive])]);

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
        var source = new THost(engine,[xone, xtwo, xthree]);
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
        var source = new THost(engine,[xone, xtwo, xthree]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var source = new THost(engine,[xone, xtwo, xthree]);
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
        var engine = new FakeEngine();
        var source = new THost(engine) { AllowDuplicates = true, IgnoreCase = true };
        source = (THost)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(xfour);
        Assert.Same(source, target);

        target = source.Remove(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(new TItem("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(new TItem("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Value_IgnoreCase()
    {
        var engine = new FakeEngine();
        var source = new THost(engine) { AllowDuplicates = true, IgnoreCase = true };
        source = (THost)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.RemoveAll(new TItem("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Value_Nested()
    {
        var engine = new FakeEngine();
        var source = new THost(engine) { AllowDuplicates = true, IgnoreCase = true };
        source = (THost)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(new THost(engine,[xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveAll(new THost(engine,[xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xthree, target[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var source = new THost(engine) { AllowDuplicates = true, IgnoreCase = true };
        source = (THost)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(x => x is TItem TItem && TItem.Name is null);
        Assert.Same(source, target);

        target = source.Remove(x => x is TItem TItem && TItem.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(x => x is TItem TItem && TItem.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => x is TItem TItem && TItem.Name.Contains('n'));
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
        var source = new THost(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new THost(engine,[xone, xtwo, xthree]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}*/