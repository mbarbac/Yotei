namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_Command
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        ICommand command;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        command = new FakeCommand(connection);
        info = command.GetCommandInfo();
        Assert.False(command.IsValid);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);

        Assert.Null(command.RawLocale);
        Assert.Same(connection.Locale, command.Locale);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        ICommand command;
        ICommandInfo info;
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        command = new FakeCommand(connection,
            "SELECT * FROM [Emps] WHERE [Id] = {0}",
            "007");
        Assert.True(command.IsValid);

        info = command.GetCommandInfo();
        Assert.False(info.IsEmpty);
        Assert.Equal("SELECT * FROM [Emps] WHERE [Id] = #0", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name);
        Assert.Equal("007", info.Parameters[0].Value);
    }
}