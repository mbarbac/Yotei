using Named = Yotei.ORM.InvariantGenerator.Tests.NamedElement;
using Chain = Yotei.ORM.InvariantGenerator.Tests.ElementList_T;
using Builder = Yotei.ORM.InvariantGenerator.Tests.ElementList_T.Builder;

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

        try { _ = new Chain(engine, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain(engine, [xone, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
        try { _ = new Chain(engine, [xone, xone]); Assert.Fail(); } catch (DuplicateException) { }

        chain = new Chain(engine, [xone, new Named("ONE")]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Equal("ONE", ((Named)chain[1]).Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_With_Duplicates()
    {
        var engine = new FakeEngine();
        var builder = new Builder(engine) { AcceptDuplicates = true };

        builder.AddRange([xone, xone]);
        var chain = builder.ToInstance();
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);
    }
}