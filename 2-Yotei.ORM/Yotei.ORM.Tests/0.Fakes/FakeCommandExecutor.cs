namespace Yotei.ORM.Tests;

// ========================================================
public class FakeCommandExecutor : CommandExecutor
{
    public FakeCommandExecutor(IExecutableCommand command) : base(command)
    {
        if (command is FakeCommand temp)
        {
            FakeExecResult = temp.FakeExecResult;
            FakeDelayMs = temp.FakeDelayMs;
        }
    }

    public int FakeExecResult { get; set; }
    public int FakeDelayMs { get; set; }

    // -----------------------------------------------------

    protected override int OnExecute() => FakeExecResult;
    protected override async ValueTask<int> OnExecuteAsync(CancellationToken token)
    {
        if (FakeDelayMs > 0)
        {
            await Task.Delay(FakeDelayMs, token);
            token.ThrowIfCancellationRequested();
        }
        return FakeExecResult;
    }
}