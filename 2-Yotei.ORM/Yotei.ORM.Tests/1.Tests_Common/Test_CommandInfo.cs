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
        Assert.Empty(info.CommandText);
        Assert.Empty(info.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "whatever");
        Assert.Equal("whatever", info.CommandText);
        Assert.Empty(info.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Parameters()
    {
        var engine = new FakeEngine();
        var x007 = new Parameter("Id", "007");
        var xJames = new Parameter("FirstName", "James");
        var xBond = new Parameter("LastName", "Bond");
        var pars = new ParameterList(engine, [x007, xJames, xBond]);

        var info = new CommandInfo(null, pars);
        Assert.Empty(info.CommandText);
        Assert.Same(pars, info.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_Arguments()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "[Id] = {0}, Age = {1}", "007", 50);
        Assert.Equal("[Id] = #0, Age = #1", info.CommandText);
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
        var info = new CommandInfo(engine, "[Id] = {0}, Age = {1}", "007", 50);

        info.Add(" whatever");
        Assert.Equal("[Id] = #0, Age = #1 whatever", info.CommandText);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Parameter()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "[Id] = {0}, Age = {1}", "007", 50);

        info.Add(new Parameter("#any", null));
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
        Assert.Equal("#any", info.Parameters[2].Name); Assert.Null(info.Parameters[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddTextArgs_Numeric_Specification()
    {
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "[Id] = {0}", "007");
        info.Add(", Age = {0}", 50);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddTextArgs_Numeric_Over_Null_Text()
    {
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "[Id] = {0}", "007");
        info.Add(", Age = {0}", 50);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);

        info.Add(null, "other");
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("#2", info.Parameters[2].Name); Assert.Equal("other", info.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddTextArgs_Numeric_No_Corresponding_Text()
    {
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "[Id] = {0}", "007");
        info.Add(", Age = {0}", 50);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);

        try { info.Add("", "other"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddTextArgs_Parameter_Specification()
    {
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "[Id] = {0}", "007");
        info.Add(", Age = {#age}", new Parameter("#age", 50));
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#age", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddTextArgs_Parameter_Specification_Over_Null_Text()
    {
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "[Id] = {0}", "007");
        info.Add(null, new Parameter("#age", 50));
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#age", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddTextArgs_Parameter_Specification_No_Corresponding_Text()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "[Id] = {0}", "007");

        try { info.Add("", new Parameter("#age", 50)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddTextArgs_Anonymous_Specification()
    {
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "[Id] = {0}", "007");
        info.Add(", Age = {age}", new { age = 50 });
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#age", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);

        try { info.Add(", Age = {age}", new { age = 50, other = 9 }); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddTextArgs_Anonymous_Specification_Over_Null_Text()
    {
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "[Id] = {0}", "007");
        info.Add(null, new { age = 50 });
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#age", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddTextArgs_Anonymous_Specification_No_Corresponding_Text()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "[Id] = {0}", "007");

        try { info.Add("", new { age = 50 }); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Info()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Id] = {0}", "007");
        var target = new CommandInfo(engine, ", Age = {0}", 50);

        source.Add(target);
        Assert.Equal("[Id] = #0, Age = #1", source.CommandText);
        Assert.Equal(2, source.Parameters.Count);
        Assert.Equal("#0", source.Parameters[0].Name); Assert.Equal("007", source.Parameters[0].Value);
        Assert.Equal("#1", source.Parameters[1].Name); Assert.Equal(50, source.Parameters[1].Value);
    }
}