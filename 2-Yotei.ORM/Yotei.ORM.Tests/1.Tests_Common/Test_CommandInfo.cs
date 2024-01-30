namespace Yotei.ORM.Tests;

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
        Assert.True(info.IsEmpty);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "any");
        Assert.False(info.IsEmpty);
        Assert.Equal("any", info.Text);
        Assert.Empty(info.Parameters);

        try { _ = new CommandInfo(engine, (string)null!); Assert.Fail();  }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Parameters()
    {
        var engine = new FakeEngine();
        var x007 = new Parameter("Id", "007");
        var xJames = new Parameter("FirstName", "James");
        var xBond = new Parameter("LastName", "Bond");
        var pars = new Code.ParameterList(engine, [x007, xJames, xBond]);

        var info = new CommandInfo(pars);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(3, info.Parameters.Count);
        Assert.Same(x007, info.Parameters[0]);
        Assert.Same(xJames, info.Parameters[1]);
        Assert.Same(xBond, info.Parameters[2]);

        try { _ = new CommandInfo((ParameterList)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new CommandInfo(new Code.ParameterList(engine, [x007, null!])); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Parameters()
    {
        var engine = new FakeEngine();
        var x007 = new Parameter("#Id", "007");
        var xJames = new Parameter("#FirstName", "James");
        var pars = new Code.ParameterList(engine, [x007, xJames]);

        var info = new CommandInfo("[Id]={0}, [Name]={1}", pars);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Id]=#Id, [Name]=#FirstName", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Same(x007, info.Parameters[0]);
        Assert.Same(xJames, info.Parameters[1]);

        try { _ = new CommandInfo((string)null!, pars); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new CommandInfo("any", (ParameterList)null!); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Arguments()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "[Id]={0}, Age={1}", "007", 50);

        Assert.False(info.IsEmpty);
        Assert.Equal("[Id]=#0, Age=#1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Text()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Id]={0}, Age={1}", "007", 50);

        var target = source.Add(" any");
        Assert.NotSame(source, target);
        Assert.Equal("[Id]=#0, Age=#1 any", target.Text);
        Assert.Same(source.Parameters, target.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Parameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "any");

        var pars = new Code.ParameterList(engine, [new Parameter("Id", "007"), new Parameter("Age", 50)]);
        var target = source.Add(pars);
        Assert.NotSame(source, target);
        Assert.Equal("any", target.Text);
        Assert.Same(pars, target.Parameters);

        source = new CommandInfo(engine, "[Name]={0}", "Bond");
        target = source.Add(pars);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("Bond", target.Parameters[0].Value);
        Assert.Equal("Id", target.Parameters[1].Name); Assert.Equal("007", target.Parameters[1].Value);
        Assert.Equal("Age", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Source()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Id]={0}", "007");
        var temp = new CommandInfo(engine, ", [Age]={0}", 50);

        var target = source.Add(temp);
        Assert.NotSame(source, target);
        Assert.Equal("[Id]=#0, [Age]=#1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("007", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal(50, target.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Parameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Id]={0}", "007");

        var xJames = new Parameter("#FirstName", "James");
        var xBond = new Parameter("#LastName", "Bond");
        var pars = new Code.ParameterList(engine, [xJames, xBond]);

        var target = source.Add(", [First]={0}, [Last]={1}", pars);
        Assert.NotSame(source, target);
        Assert.Equal("[Id]=#0, [First]=#FirstName, [Last]=#LastName", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("007", target.Parameters[0].Value);
        Assert.Equal("#FirstName", target.Parameters[1].Name); Assert.Equal("James", target.Parameters[1].Value);
        Assert.Equal("#LastName", target.Parameters[2].Name); Assert.Equal("Bond", target.Parameters[2].Value);

        try { source.Add("{0}", new Code.ParameterList(engine, [new Parameter("#0", "any")])); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Arguments()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Id]={0}", "007");

        var target = source.Add(", [First]={0}, [Last]={1}, [Age]={2}",
            "James",
            new { Last = "Bond" },
            new Parameter("#Age", 50));

        Assert.NotSame(source, target);
        Assert.Equal("[Id]=#0, [First]=#1, [Last]=#Last, [Age]=#Age", target.Text);

        // Args not used...
        try { source.Add("blank", 25); Assert.Fail(); }
        catch (ArgumentException) { }

        // Specification out of range...
        try { source.Add("{9}", 25); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }
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

        source = new CommandInfo(engine, "[Id]={0}, Age={1}", "007", 50);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.True(target.IsEmpty);
    }
}