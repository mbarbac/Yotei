using Xunit.Internal;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Identifier_Varied_Terminators
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
        var chain = new Identifier(engine, null);
        Assert.Single(chain);
        Assert.Null(chain[0]);
        Assert.Null(chain[0, true]);
        Assert.Null(chain.Value);
        Assert.Equal("", chain.ToString());
        
        chain = new Identifier(engine, "");
        Assert.Single(chain);
        Assert.Null(chain[0]);
        Assert.Null(chain[0, true]);
        Assert.Null(chain.Value);
        Assert.Equal("", chain.ToString());

        chain = new Identifier(engine, " ");
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

        var chain = new Identifier(engine, " . . ");
        Assert.Equal(3, chain.Count);
        Assert.Null(chain[0]); Assert.Null(chain[0, true]);
        Assert.Null(chain[1]); Assert.Null(chain[1, true]);
        Assert.Null(chain[2]); Assert.Null(chain[2, true]);
        Assert.Null(chain.Value);
        Assert.Equal("..", chain.ToString());
        Assert.Equal("", chain.ToString(true, true));

        chain = new Identifier(engine, " [ ] . [ ] ");
        Assert.Equal(2, chain.Count);
        Assert.Null(chain[0]); Assert.Null(chain[0, true]);
        Assert.Null(chain[1]); Assert.Null(chain[1, true]);
        Assert.Null(chain.Value);
        Assert.Equal(".", chain.ToString());
        Assert.Equal("", chain.ToString(true, true));
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Multi_Populated()
    {
        var engine = new FakeEngine() { IgnoreCase = true };

        var chain = new Identifier(engine, " . two . ");
        Assert.Equal(3, chain.Count);
        Assert.Null(chain[0]); Assert.Null(chain[0, true]);
        Assert.Equal("[two]", chain[1]); Assert.Equal("two", chain[1, false]);
        Assert.Null(chain[2]); Assert.Null(chain[2, true]);
        Assert.Equal(".[two].", chain.Value);
        Assert.Equal(".[two].", chain.ToString());
        Assert.Equal("[two].", chain.ToString(true, true));

        chain = new Identifier(engine, "[ aa . bb ]");
        Assert.Single(chain);
        Assert.Equal("aa . bb", chain[0, false]);
        Assert.Equal("[aa . bb]", chain.Value);
        Assert.Equal("[aa . bb]", chain.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var engine = new FakeEngine() { IgnoreCase = true };

        var chain = new Identifier(engine, " ", " . three . ");
        Assert.Equal(4, chain.Count);
        Assert.Null(chain[0]); Assert.Null(chain[0, true]);
        Assert.Null(chain[1]); Assert.Null(chain[1, true]);
        Assert.Equal("[three]", chain[2]); Assert.Equal("three", chain[2, false]);
        Assert.Null(chain[3]); Assert.Null(chain[3, true]);
        Assert.Equal("..[three].", chain.Value);
        Assert.Equal("..[three].", chain.ToString());
        Assert.Equal("[three].", chain.ToString(true, true));

        chain = new Identifier(engine, "one", "one");
        Assert.Equal(2, chain.Count);
        Assert.Equal("[one]", chain[0]); Assert.Equal("one", chain[0, false]);
        Assert.Equal("[one]", chain[1]); Assert.Equal("one", chain[1, false]);
        Assert.Equal("[one].[one]", chain.Value);
        Assert.Equal("[one].[one]", chain.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Null(target.Value);
        Assert.Equal("", target.ToString());

        source = new Identifier(engine, []);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Null(target.Value);
        Assert.Equal("", target.ToString());

        source = new Identifier(engine, " . aa ", " bb . cc ", "");
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Null(target[0]);
        Assert.Equal("[aa]", target[1]);
        Assert.Equal("[bb]", target[2]);
        Assert.Equal("[cc]", target[3]);
        Assert.Null(target[4]);
        Assert.Equal(".[aa].[bb].[cc].", target.Value);
        Assert.Equal(".[aa].[bb].[cc].", target.ToString());
        Assert.Equal("[aa].[bb].[cc].", target.ToString(true, true));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var chain = new Identifier(engine, "one", "two", "three", "one");

        var index = chain.IndexOf("[aa.bb]"); Assert.Equal(-1, index);
        try { chain.IndexOf("aa.bb"); Assert.Fail(); } catch (ArgumentException) { }

        index = chain.IndexOf("one"); Assert.Equal(0, index);
        index = chain.IndexOf("ONE"); Assert.Equal(0, index);

        index = chain.LastIndexOf("one"); Assert.Equal(3, index);
        index = chain.LastIndexOf("ONE"); Assert.Equal(3, index);

        var nums = chain.IndexesOf("one");
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);

        nums = chain.IndexesOf("ONE");
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var chain = new Identifier(engine, "one", "two", "three", "one");

        var index = chain.IndexOf(x => x is null); Assert.Equal(-1, index);

        index = chain.IndexOf(x => x is not null && x.Contains('n')); Assert.Equal(0, index);
        index = chain.LastIndexOf(x => x is not null && x.Contains('n')); Assert.Equal(3, index);

        var nums = chain.IndexesOf(x => x is not null && x.Contains('n'));
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine, "one.two.three.four");
        var target = source.GetRange(0, 4);
        Assert.Same(source, target);
            
        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0, false]);
        Assert.Equal("three", target[1, false]);
        Assert.Equal("[two].[three]", target.Value);
        Assert.Equal("[two].[three]", target.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Reduce()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine);
        var target = source.Reduce();
        Assert.Same(source, target);

        source = new Identifier(engine, "one");
        target = source.Reduce();
        Assert.Same(source, target);

        source = new Identifier(engine, " . two . three ");
        target = source.Reduce();
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0, false]);
        Assert.Equal("three", target[1, false]);
        Assert.Equal("[two].[three]", target.Value);
        Assert.Equal("[two].[three]", target.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine, "aa..cc");
        var target = source.Replace(1, "bb");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("aa", target[0, false]);
        Assert.Equal("bb", target[1, false]);
        Assert.Equal("cc", target[2, false]);
        Assert.Equal("[aa].[bb].[cc]", target.Value);
        Assert.Equal("[aa].[bb].[cc]", target.ToString());

        target = source.Replace(1, "xx", "yy");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("aa", target[0, false]);
        Assert.Equal("xx", target[1, false]);
        Assert.Equal("yy", target[2, false]);
        Assert.Equal("cc", target[3, false]);
        Assert.Equal("[aa].[xx].[yy].[cc]", target.Value);
        Assert.Equal("[aa].[xx].[yy].[cc]", target.ToString());

        target = source.Replace(0, null);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Null(target[0, false]);
        Assert.Null(target[1, false]);
        Assert.Equal("cc", target[2, false]);
        Assert.Equal("..[cc]", target.Value);
        Assert.Equal("..[cc]", target.ToString());
        Assert.Equal("[cc]", target.ToString(true, true));
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Same()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine, "aa..cc");
        var target = source.Replace(0, "aa");
        Assert.Same(source, target);

        target = source.Replace(1, null);
        Assert.Same(source, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Empty()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine);
        var target = source.Add("");
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Null(target[0, false]);
        Assert.Null(target.Value);
        Assert.Equal("", target.ToString());

        target = source.Add(" .  two ");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Null(target[0, false]);
        Assert.Equal("two", target[1, false]);
        Assert.Equal(".[two]", target.Value);
        Assert.Equal(".[two]", target.ToString());
        Assert.Equal("[two]", target.ToString(true, true));
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine, " one ");
        var target = source.Add(" two ");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("one", target[0, false]);
        Assert.Equal("two", target[1, false]);
        Assert.Equal("[one].[two]", target.Value);
        Assert.Equal("[one].[two]", target.ToString());

        target = source.Add(" . three ");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0, false]);
        Assert.Null(target[1, false]);
        Assert.Equal("three", target[2, false]);
        Assert.Equal("[one]..[three]", target.Value);
        Assert.Equal("[one]..[three]", target.ToString());

        target = source.Add(" two . ");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0, false]);
        Assert.Equal("two", target[1, false]);
        Assert.Null(target[2, false]);
        Assert.Equal("[one].[two].", target.Value);
        Assert.Equal("[one].[two].", target.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert_Empty()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine);
        var target = source.Insert(0, "");
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Null(target[0, false]);
        Assert.Null(target.Value);
        Assert.Equal("", target.ToString());

        target = source.Insert(0, " .  two ");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Null(target[0, false]);
        Assert.Equal("two", target[1, false]);
        Assert.Equal(".[two]", target.Value);
        Assert.Equal(".[two]", target.ToString());
        Assert.Equal("[two]", target.ToString(true, true));

        try { source.Insert(1, "any"); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine, " one ");
        var target = source.Insert(1, " two ");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("one", target[0, false]);
        Assert.Equal("two", target[1, false]);
        Assert.Equal("[one].[two]", target.Value);
        Assert.Equal("[one].[two]", target.ToString());

        target = source.Insert(1, " . three ");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0, false]);
        Assert.Null(target[1, false]);
        Assert.Equal("three", target[2, false]);
        Assert.Equal("[one]..[three]", target.Value);
        Assert.Equal("[one]..[three]", target.ToString());

        target = source.Insert(1, " two . ");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0, false]);
        Assert.Equal("two", target[1, false]);
        Assert.Null(target[2, false]);
        Assert.Equal("[one].[two].", target.Value);
        Assert.Equal("[one].[two].", target.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine, "one.two.three.four");
        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("two", target[0, false]);
        Assert.Equal("three", target[1, false]);
        Assert.Equal("four", target[2, false]);
        Assert.Equal("[two].[three].[four]", target.Value);
        Assert.Equal("[two].[three].[four]", target.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_Empty()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine, "one.two.three.four");
        var target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("two", target[0, false]);
        Assert.Equal("three", target[1, false]);
        Assert.Equal("four", target[2, false]);
        Assert.Equal("[two].[three].[four]", target.Value);
        Assert.Equal("[two].[three].[four]", target.ToString());

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("one", target[0, false]);
        Assert.Equal("four", target[1, false]);
        Assert.Equal("[one].[four]", target.Value);
        Assert.Equal("[one].[four]", target.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine, "one.two.three.one");
        var target = source.Remove("any");
        Assert.Same(source, target);

        target = source.Remove("one");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("two", target[0, false]);
        Assert.Equal("three", target[1, false]);
        Assert.Equal("one", target[2, false]);
        Assert.Equal("[two].[three].[one]", target.Value);
        Assert.Equal("[two].[three].[one]", target.ToString());

        target = source.RemoveLast("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0, false]);
        Assert.Equal("two", target[1, false]);
        Assert.Equal("three", target[2, false]);
        Assert.Equal("[one].[two].[three]", target.Value);
        Assert.Equal("[one].[two].[three]", target.ToString());

        target = source.RemoveAll("one");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0, false]);
        Assert.Equal("three", target[1, false]);
        Assert.Equal("[two].[three]", target.Value);
        Assert.Equal("[two].[three]", target.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine, "one.two.three.one");
        var target = source.Remove(x => x is null);
        Assert.Same(source, target);

        target = source.Remove(x => x != null && x.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("two", target[0, false]);
        Assert.Equal("three", target[1, false]);
        Assert.Equal("one", target[2, false]);
        Assert.Equal("[two].[three].[one]", target.Value);
        Assert.Equal("[two].[three].[one]", target.ToString());

        target = source.RemoveLast(x => x != null && x.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0, false]);
        Assert.Equal("two", target[1, false]);
        Assert.Equal("three", target[2, false]);
        Assert.Equal("[one].[two].[three]", target.Value);
        Assert.Equal("[one].[two].[three]", target.ToString());

        target = source.RemoveAll(x => x != null && x.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0, false]);
        Assert.Equal("three", target[1, false]);
        Assert.Equal("[two].[three]", target.Value);
        Assert.Equal("[two].[three]", target.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Identifier(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Identifier(engine, "one.two.three.one");
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Equal(0, target.Count);
    }
}