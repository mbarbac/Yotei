using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Records;
/*
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
        var info = command.GetCommandInfo();
        Assert.True(info.IsEmpty);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var command = new FakeCommand(connection, "SELECT * FROM [Emps] WHERE [Id] = {0}", "007");
        var info = command.GetCommandInfo();
        Assert.Equal("SELECT * FROM [Emps] WHERE [Id] = #0", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("007", info.Parameters[0].Value);
    }
}
*/