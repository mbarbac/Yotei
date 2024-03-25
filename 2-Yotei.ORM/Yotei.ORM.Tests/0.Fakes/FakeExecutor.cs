#pragma warning disable IDE0290 // Use primary constructor

namespace Yotei.ORM.Tests;

// ========================================================
public class FakeExecutor : CommandExecutor
{
    public FakeExecutor(IExecutableCommand command) : base(command) { }
    public override string ToString() => $"FakeExecutor({Command})";

    public int FakeResult { get; set; }

    // ----------------------------------------------------

    protected override int OnExecute() => FakeResult;
    protected override ValueTask<int> OnExecuteAsync(CancellationToken token) => ValueTask.FromResult(FakeResult);
}