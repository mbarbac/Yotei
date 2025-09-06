namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_CommandInfo
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsEmpty);

        info = new CommandInfo(engine, "");
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsEmpty);

        info = new CommandInfo(engine, "", []);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsEmpty);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Text()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "SELECT");
        Assert.Equal("SELECT", info.Text);
        Assert.Empty(info.Parameters);
        Assert.False(info.IsEmpty);

        // Dangling specification...
        try { _ = new CommandInfo(engine, "{any}"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Values()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "{0}", null);
        Assert.Equal("#0", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);

        info = new CommandInfo(engine, "{0} {1}", null, null);
        Assert.Equal("#0 #1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Null(info.Parameters[1].Value);

        info = new CommandInfo(engine, "{0} {1}", "James", "Bond");
        Assert.Equal("#0 #1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        // Unused value...
        try { _ = new CommandInfo(engine, "{0}", "one", "two"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Parameters()
    {
        var xfirst = new Parameter("#First", "James");
        var xlast = new Parameter("Last", "Bond");
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "{0} {1}", xfirst, xlast);
        Assert.Equal("#First #Last", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new CommandInfo(engine, "{First} {#Last}", xfirst, xlast);
        Assert.Equal("#First #Last", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        // Unused value...
        try { _ = new CommandInfo(engine, "{0}", xfirst, xlast); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Anonymous()
    {
        var xfirst = new { @First = "James" };
        var xlast = new { Last = "Bond" };
        var engine = new FakeEngine() { ParameterPrefix = "@" };

        var info = new CommandInfo(engine, "{0} {1}", xfirst, xlast);
        Assert.Equal("@First @Last", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("@First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("@Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new CommandInfo(engine, "{First} {@Last}", xfirst, xlast);
        Assert.Equal("@First @Last", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("@First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("@Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        // Unused value...
        try { _ = new CommandInfo(engine, "{0}", xfirst, xlast); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Values_As_Collection()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "{0} {1}", [null, "Bond"]);
        Assert.Equal("#0 #1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        object[] array = ["James", "Bond"];
        info = new CommandInfo(engine, "{0} {1}", null, array);
        Assert.Equal("#0 #1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name);
        var temp = Assert.IsType<object[]>(info.Parameters[1].Value);
        Assert.Equal("James", temp[0]);
        Assert.Equal("Bond", temp[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Duplicated_Names()
    {
        var xone = new Parameter("Any", 1);
        var engine = new FakeEngine() { ParameterPrefix = "@" };

        var xtwo = new Parameter("Any", 2);
        try { _ = new CommandInfo(engine, "{0} {1}", xone, xtwo); Assert.Fail(); }
        catch (DuplicateException) { }

        var xthree = new Parameter("@Any", 2);
        try { _ = new CommandInfo(engine, "{0} {1}", xone, xthree); Assert.Fail(); }
        catch (DuplicateException) { }

        var xfour = new { Any = 3 };
        try { _ = new CommandInfo(engine, "{0} {1}", xone, xfour); Assert.Fail(); }
        catch (DuplicateException) { }

        var xfive = new { @Any = 3 };
        try { _ = new CommandInfo(engine, "{0} {1}", xone, xfive); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.True(target.IsEmpty);

        source = new CommandInfo(engine, "{0} {1}", "James", "Bond");
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new CommandInfo(engine, "{0} {1}", "James", "Bond");
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.True(target.IsEmpty);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_Text()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        var target = source.ReplaceText(string.Empty);
        Assert.Same(source, target);

        source = new CommandInfo(engine, "any");
        target = source.ReplaceText("any");
        Assert.Same(source, target);

        source = new CommandInfo(engine, "{0} {1}", "James", "Bond");
        target = source.ReplaceText("any");
        Assert.NotSame(source, target);
        Assert.Equal("any", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        target = source.ReplaceText("{3}");
        Assert.NotSame(source, target);
        Assert.Equal("{3}", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        target = source.ReplaceText("#3");
        Assert.NotSame(source, target);
        Assert.Equal("#3", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_Values()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        var target = source.ReplaceValues([]);
        Assert.Same(source, target);

        source = new CommandInfo(engine, "{0} {1}", "James", "Bond");
        target = source.ReplaceValues([]);
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Empty(target.Parameters);

        target = source.ReplaceValues("one");
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("one", target.Parameters[0].Value);

        target = source.ReplaceValues("one", "two", "three");
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("one", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("two", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal("three", target.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Parameters()
    {
        var xone = new Parameter("#One", 1);
        var xtwo = new Parameter("Two", 2);
        var xthree = new Parameter("Three", 3);

        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "{0} {1}", new { First = "James" }, new { Last = "Bond" });
        var target = source.ReplaceValues(xone);
        Assert.NotSame(source, target);
        Assert.Equal("#First #Last", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#One", target.Parameters[0].Name); Assert.Equal(1, target.Parameters[0].Value);

        target = source.ReplaceValues(xone, xtwo, xthree);
        Assert.NotSame(source, target);
        Assert.Equal("#First #Last", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#One", target.Parameters[0].Name); Assert.Equal(1, target.Parameters[0].Value);
        Assert.Equal("#Two", target.Parameters[1].Name); Assert.Equal(2, target.Parameters[1].Value);
        Assert.Equal("#Three", target.Parameters[2].Name); Assert.Equal(3, target.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Anonymous()
    {
        var xone = new { @One = 1 };
        var xtwo = new { Two = 2 };
        var xthree = new { Three = 3 };
        var engine = new FakeEngine() { ParameterPrefix = "@" };

        var source = new CommandInfo(engine, "{0} {1}", new { First = "James" }, new { Last = "Bond" });
        var target = source.ReplaceValues(xone);
        Assert.NotSame(source, target);
        Assert.Equal("@First @Last", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("@One", target.Parameters[0].Name); Assert.Equal(1, target.Parameters[0].Value);

        target = source.ReplaceValues(xone, xtwo, xthree);
        Assert.NotSame(source, target);
        Assert.Equal("@First @Last", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("@One", target.Parameters[0].Name); Assert.Equal(1, target.Parameters[0].Value);
        Assert.Equal("@Two", target.Parameters[1].Name); Assert.Equal(2, target.Parameters[1].Value);
        Assert.Equal("@Three", target.Parameters[2].Name); Assert.Equal(3, target.Parameters[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Info()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "{0} {1}", new { First = "James" }, new { Last = "Bond" });
        var other = new CommandInfo(engine, " {0} {1}", new { One = 1 }, new { Two = 2 });

        var target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.Equal("#First #Last #One #Two", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#One", target.Parameters[2].Name); Assert.Equal(1, target.Parameters[2].Value);
        Assert.Equal("#Two", target.Parameters[3].Name); Assert.Equal(2, target.Parameters[3].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Info_Inconsistent()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "{0} {1}", new { First = "James" }, new { Last = "Bond" });
        var other = source.ReplaceText(" any");

        var target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.Equal("#First #Last any", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal("James", target.Parameters[2].Value);
        Assert.Equal("#3", target.Parameters[3].Name); Assert.Equal("Bond", target.Parameters[3].Value);

        other = source.ReplaceValues(new { Other = "any" });
        target = source.Add(other);
        Assert.Equal("#First #Last#First #Last", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Other", target.Parameters[2].Name); Assert.Equal("any", target.Parameters[2].Value);
    }
}