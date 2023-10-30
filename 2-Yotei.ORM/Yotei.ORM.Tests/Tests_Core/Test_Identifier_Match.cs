namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Identifier_Match
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        var engine = new FakeEngine();

        // Empty vs Empty: match...
        var source = Identifier.Create(engine);
        var target = Identifier.Create(engine);
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        // Empty vs Single: no match...
        source = Identifier.Create(engine);
        target = Identifier.Create(engine, "x");
        Assert.False(source.Match(target));
        Assert.True(target.Match(source)); // Single vs Empty: match...

        // Empty vs Multi: no match...
        source = Identifier.Create(engine);
        target = Identifier.Create(engine, "x..z");
        Assert.False(source.Match(target));
        Assert.True(target.Match(source)); // Multi vs Empty: match...
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_No_CaseSensitive_Single_Single()
    {
        var engine = new FakeEngine() { CaseSensitiveNames = false };

        var source = Identifier.Create(engine, "x");
        var target = Identifier.Create(engine, "X");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "x");
        target = Identifier.Create(engine, "any");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_No_CaseSensitive_Same_Lenght()
    {
        var engine = new FakeEngine() { CaseSensitiveNames = false };

        var source = Identifier.Create(engine, "x.y.z");
        var target = Identifier.Create(engine, "X..");
        Assert.True(source.Match(target));
        Assert.False(target.Match(source)); // Reverse: no match...

        source = Identifier.Create(engine, "x.y.z");
        target = Identifier.Create(engine, ".Y.");
        Assert.True(source.Match(target));
        Assert.False(target.Match(source)); // Reverse: no match...

        source = Identifier.Create(engine, "x.y.z");
        target = Identifier.Create(engine, "..Z");
        Assert.True(source.Match(target));
        Assert.False(target.Match(source)); // Reverse: no match...

        source = Identifier.Create(engine, "x.y.z");
        target = Identifier.Create(engine, "X.Y.");
        Assert.True(source.Match(target));
        Assert.False(target.Match(source)); // Reverse: no match...

        source = Identifier.Create(engine, "x.y.z");
        target = Identifier.Create(engine, ".Y.Z");
        Assert.True(source.Match(target));
        Assert.False(target.Match(source)); // Reverse: no match...

        source = Identifier.Create(engine, "x..z");
        target = Identifier.Create(engine, "X..");
        Assert.True(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "x..z");
        target = Identifier.Create(engine, "any..");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_No_CaseSensitive_Diff_Lenght()
    {
        var engine = new FakeEngine() { CaseSensitiveNames = false };

        // Single vs Multi...
        var source = Identifier.Create(engine, "z");
        var target = Identifier.Create(engine, "..Z");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source)); // Match because empty filters!

        source = Identifier.Create(engine, "z");
        target = Identifier.Create(engine, "x..Z");
        Assert.False(source.Match(target));
        Assert.True(target.Match(source)); // Match because 'one' is a single filter!

        source = Identifier.Create(engine, "any");
        target = Identifier.Create(engine, "..z");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "any");
        target = Identifier.Create(engine, "x..z");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));

        // Multi vs Multi...
        source = Identifier.Create(engine, "x.y.z");
        target = Identifier.Create(engine, ".Z");
        Assert.True(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "x.y.z");
        target = Identifier.Create(engine, ".Z");
        Assert.True(source.Match(target));
        Assert.False(target.Match(source));

        source = Identifier.Create(engine, "x..z");
        target = Identifier.Create(engine, ".Z");
        Assert.True(source.Match(target));
        Assert.False(target.Match(source));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_CaseSensitive_Single_Single()
    {
        var engine = new FakeEngine() { CaseSensitiveNames = true };

        var source = Identifier.Create(engine, "x");
        var target = Identifier.Create(engine, "x");
        Assert.True(source.Match(target));
        Assert.True(target.Match(source));

        source = Identifier.Create(engine, "x");
        target = Identifier.Create(engine, "X");
        Assert.False(source.Match(target));
        Assert.False(target.Match(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_CaseSensitive_Same_Lenght()
    {
        var engine = new FakeEngine() { CaseSensitiveNames = true };

        var source = Identifier.Create(engine, "x.y.z");
        var target = Identifier.Create(engine, "X..");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "x.y.z");
        target = Identifier.Create(engine, ".Y.");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "x.y.z");
        target = Identifier.Create(engine, "..Z");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "x.y.z");
        target = Identifier.Create(engine, "X.Y.");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "x.y.z");
        target = Identifier.Create(engine, ".Y.Z");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "x..z");
        target = Identifier.Create(engine, "X..");
        Assert.False(source.Match(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_CaseSensitive_Diff_Lenght()
    {
        var engine = new FakeEngine() { CaseSensitiveNames = true };

        // Single vs Multi...
        var source = Identifier.Create(engine, "z");
        var target = Identifier.Create(engine, "..Z");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "z");
        target = Identifier.Create(engine, "x..Z");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "any");
        target = Identifier.Create(engine, "..z");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "any");
        target = Identifier.Create(engine, "x..z");
        Assert.False(source.Match(target));

        // Multi vs Multi...
        source = Identifier.Create(engine, "x.y.z");
        target = Identifier.Create(engine, ".Z");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "x.y.z");
        target = Identifier.Create(engine, ".Z");
        Assert.False(source.Match(target));

        source = Identifier.Create(engine, "x..z");
        target = Identifier.Create(engine, ".Z");
        Assert.False(source.Match(target));
    }
}