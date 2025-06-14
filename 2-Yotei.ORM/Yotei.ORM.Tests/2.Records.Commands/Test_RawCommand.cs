using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Records;

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
        IRawCommand command;
        ICommandInfo info;

        command = new RawCommand(connection);
        info = command.GetCommandInfo();
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Text()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        ICommandInfo info;

        command = new RawCommand(connection, "FROM [Emps]");
        info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps]", info.Text);
        Assert.Empty(info.Parameters);

        command = new RawCommand(connection, "FROM [Emps] WHERE [Id]={0}", "007");
        info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE [Id]=#0", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Lambda()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        ICommandInfo info;

        command = new RawCommand(connection);
        command.Append(x => x("FROM [Emps] WHERE ")(x.Id = "007"));
        info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE ([Id] = #0)", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Lambda_To_Text()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        ICommandInfo info;

        command = new RawCommand(connection);
        command.Append(x => "FROM [Emps] WHERE [Id] = '007'");
        info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE [Id] = '007'", info.Text);
        Assert.Empty(info.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Append_Only_Text()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        ICommandInfo info;

        command = new RawCommand(connection, "FROM [Emps] WHERE [Id]={0}", "007");
        command.Append(" ANY");
        info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE [Id]=#0 ANY", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Append_Only_Values()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        ICommandInfo info;

        command = new RawCommand(connection, "FROM [Emps] WHERE [Id]={0}", "007");
        command.Append(null, new Parameter("Age", 50));
        info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE [Id]=#0", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#Age", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Append_Text_And_Values()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        ICommandInfo info;

        command = new RawCommand(connection, "FROM [Emps] WHERE [Id]={0}", "007");
        command.Append(" AND [Age]={0}", 50);
        info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE [Id]=#0 AND [Age]=#1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);

        var page = new Parameter("Age", 50);
        command = new RawCommand(connection, "FROM [Emps] WHERE [Id]={0}", "007");
        command.Append(" AND [Age]={0}", page);
        info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE [Id]=#0 AND [Age]=#Age", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#Age", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);

        var xage = new { Age = 50 };
        command = new RawCommand(connection, "FROM [Emps] WHERE [Id]={0}", "007");
        command.Append(" AND [Age]={0}", xage);
        info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE [Id]=#0 AND [Age]=#Age", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#Age", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Append_Lambda()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        ICommandInfo info;

        command = new RawCommand(connection, "FROM [Emps] WHERE [Id]={0}", "007");
        command.Append(x => x(" AND ")(x.Age = 50));
        info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE [Id]=#0 AND ([Age] = #1)", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Append_Lambda_NULL()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        ICommandInfo info;

        command = new RawCommand(connection, "FROM [Emps] WHERE [Id]={0}", "007");
        command.Append(x => x(" AND ")(x.Ctry != null));
        info = command.GetCommandInfo();
        Assert.Equal("FROM [Emps] WHERE [Id]=#0 AND ([Ctry] IS NOT NULL)", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        IRawCommand command;
        ICommandInfo info;

        command = new RawCommand(connection, "FROM [Emps] WHERE [Id]={0}", "007");
        command.Clear();
        info = command.GetCommandInfo();
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
    }
}