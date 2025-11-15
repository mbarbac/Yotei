namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public class Test_Command
{
    //[Enforced]
    [Fact]
    public void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        var command = new FakeCommand(connection);
        Assert.True(command.GetCommandInfo().IsEmpty);
        Assert.False(command.IsValid);
    }
}