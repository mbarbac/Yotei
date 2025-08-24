namespace Yotei.ORM.Tests;

// ========================================================
public class FakeCommandExecutor : CommandExecutor
{
    public FakeCommandExecutor(IExecutableCommand command) : base(command) { }

    public int FakeResult { get; set; }

    // -----------------------------------------------------

    protected override int OnExecute() => FakeResult;
    protected override ValueTask<int> OnExecuteAsync(CancellationToken _) => ValueTask.FromResult(FakeResult);
}