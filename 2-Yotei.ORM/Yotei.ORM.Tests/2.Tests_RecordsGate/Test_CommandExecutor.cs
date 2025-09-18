#pragma warning disable IDE0017

namespace Yotei.ORM.Tests.RecordsGated;

// ========================================================
//[Enforced]
public static class Test_CommandExecutor
{
    //[Enforced]
    [Fact]
    public static void Test_Simple_Execution()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);
        command.FakeExeResult = 7;

        var value = command.Execute();
        Assert.Equal(7, value);
    }

    //[Enforced]
    [Fact]
    [SuppressMessage("", "IDE0079")]
    [SuppressMessage("", "xUnit1030")]
    public static async Task Test_Async_Execution()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);
        command.FakeExeResult = 7;

        var value = await command.ExecuteAsync().ConfigureAwait(false);
        Assert.Equal(7, value);
    }

    //[Enforced]
    [Fact]
    [SuppressMessage("", "IDE0079")]
    [SuppressMessage("", "xUnit1030")]
    public static async Task Test_Async_Execution_Cancelled()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

        command.FakeExeResult = 7;
        command.FakeDelayMs = 300;
        using var source = new CancellationTokenSource(command.FakeDelayMs / 2);

        try { await command.ExecuteAsync(source.Token).ConfigureAwait(false); Assert.Fail(); }
        catch (OperationCanceledException) { }
    }
}