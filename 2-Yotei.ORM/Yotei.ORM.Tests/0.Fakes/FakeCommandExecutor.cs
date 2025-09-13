namespace Yotei.ORM.Tests;

// ========================================================
public class FakeCommandExecutor : CommandExecutor
{
    public FakeCommandExecutor(IExecutableCommand command) : base(command)
    {
        if (command is FakeCommand temp)
        {
            FakeExeResult = temp.FakeExeResult;
            FakeDelayMs = temp.FakeDelayMs;
        }
    }

    public int FakeExeResult { get; set; }
    public int FakeDelayMs { get; set; }

    // -----------------------------------------------------

    protected override int OnExecute() => FakeExeResult;
    protected override async ValueTask<int> OnExecuteAsync(CancellationToken token)
    {
        if (FakeDelayMs > 0)
        {
            await Task.Delay(FakeDelayMs, token);
            token.ThrowIfCancellationRequested();
        }
        return FakeExeResult;
    }
}