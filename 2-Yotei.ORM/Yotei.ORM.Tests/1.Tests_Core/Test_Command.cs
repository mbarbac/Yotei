namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_Command
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var command = new FakeCommand(connection);
        Assert.True(command.GetCommandInfo().IsEmpty);
        Assert.False(command.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var command = new FakeCommand(
            connection,
            "SELECT * FROM [Emps] WHERE [Id] = {0}",
            "007");

        var info = command.GetCommandInfo();
        Assert.False(info.IsEmpty);
        Assert.True(command.IsValid);
        Assert.Equal("SELECT * FROM [Emps] WHERE [Id] = #0", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
    }
}