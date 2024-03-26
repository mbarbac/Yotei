namespace Yotei.ORM.Records.Tests;

// ========================================================
//[Enforced]
public static class Test_RawCommand
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new RawCommand(connection);

        var info = command.GetCommandInfo();
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Text()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new RawCommand(connection, "FROM [Emps]");
        var info = command.GetCommandInfo();
        Assert.False(info.IsEmpty);
        Assert.Equal("FROM [Emps]", info.Text);
        Assert.Empty(info.Parameters);

        command = new RawCommand(connection, "FROM [Emps] WHERE [Id]={0}", "007");
        info = command.GetCommandInfo();
        Assert.False(info.IsEmpty);
        Assert.Equal("FROM [Emps] WHERE [Id]=#0", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("007", info.Parameters[0].Value);
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Create_Populated_Lambda()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Append_Text()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new RawCommand(connection, "FROM [Emps] WHERE [Id]={0}", "007");
        var info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE [Id]=#0", info.Text);

        command.Append(" AND [Ctry]={0}", "UK");
        info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE [Id]=#0 AND [Ctry]=#1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("UK", info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Append_Lambda()
    //{
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new RawCommand(connection, "FROM [Emps] WHERE [Id]={0}", "007");
        var info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE [Id]=#0", info.Text);

        command.Clear();
        info = command.GetCommandInfo();
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
    }
}