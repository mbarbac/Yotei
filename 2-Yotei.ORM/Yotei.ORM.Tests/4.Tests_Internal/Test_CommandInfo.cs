using Yotei.ORM.Internal;

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

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Text()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine);

        info.Add("SELECT *");
        Assert.Equal("SELECT *", info.CommandText);
        Assert.Empty(info.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Parameter()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine);

        info.Add(new Code.Parameter(info.Parameters.NextName(), "James"));
        info.Add(new Code.Parameter(info.Parameters.NextName(), "Bond"));
        Assert.Empty(info.CommandText);
        Assert.Equal(2, info.Parameters.Count);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_NoText_Positional_Arguments()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine);

        info.Add(null, "Bond", 50);
        Assert.Empty(info.CommandText);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("Bond", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_NoText_Null_Argument()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine);

        info.Add(null, null!);
        Assert.Empty(info.CommandText);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Positional_Arguments()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine);

        info.Add("WHERE [Id] = {0}", "007");
        Assert.Equal("WHERE [Id] = #0", info.CommandText);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);

        info.Add(" AND [ManagerID] IS {0}", null!);;
        Assert.Equal("WHERE [Id] = #0 AND [ManagerID] IS #1", info.CommandText);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Null(info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Anonymous_Arguments()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine);

        info.Add("WHERE [Id] = {id}", new { id = "007" });
        Assert.Equal("WHERE [Id] = {id}", info.CommandText);
        Assert.Single(info.Parameters);
        Assert.Equal("id", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Mixed_Arguments()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine);

        info.Add("WHERE [Id] = {id} AND [ManagerId] = {1}",
            new { id = "007" },
            null);
        
        Assert.Equal("WHERE [Id] = {id} AND [ManagerId] = #1", info.CommandText);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("id", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Null(info.Parameters[1].Value);

        info.Add(" AND [Age] = {0}", 50);
        Assert.Equal("WHERE [Id] = {id} AND [ManagerId] = #1 AND [Age] = #2", info.CommandText);
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("id", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Null(info.Parameters[1].Value);
        Assert.Equal("#2", info.Parameters[2].Name); Assert.Equal(50, info.Parameters[2].Value);
    }

    // ----------------------------------------------------
}