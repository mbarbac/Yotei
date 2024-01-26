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
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Parameters()
    {
        var engine = new FakeEngine();
        var x007 = new Parameter("Id", "007");
        var xJames = new Parameter("FirstName", "James");
        var xBond = new Parameter("LastName", "Bond");
        var pars = new ParameterList(engine, [x007, xJames, xBond]);

        var info = new CommandInfo(null, pars);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(3, info.Parameters.Count);
        Assert.Same(x007, info.Parameters[0]);
        Assert.Same(xJames, info.Parameters[1]);
        Assert.Same(xBond, info.Parameters[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Arguments()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, 
            "[Id] = {0}, Age = {1}",
            "007",
            50);

        Assert.False(info.IsEmpty);
        Assert.Equal("[Id] = #0, Age = #1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Text()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Id] = {0}, Age = {1}", "007", 50);

        var target = source.Add(" any");
        Assert.NotSame(source, target);
        Assert.Equal("[Id] = #0, Age = #1 any", target.Text);
        Assert.Same(source.Parameters, target.Parameters);
    }
}