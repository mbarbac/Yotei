namespace Yotei.ORM.Tests;

// ========================================================
public class FakeTransaction : ORM.Code.Transaction
{
    [SuppressMessage("", "IDE0290")]
    public FakeTransaction(IConnection connection) : base(connection) { }
    public override string ToString() => $"FakeTransaction({Connection})";

    protected override void OnStart() { }
    protected override ValueTask OnStartAsync(CancellationToken _) => ValueTask.CompletedTask;
    protected override void OnCommit() { }
    protected override ValueTask OnCommitAsync(CancellationToken _) => ValueTask.CompletedTask;
    protected override void OnAbort() { }
    protected override ValueTask OnAbortAsync() => ValueTask.CompletedTask;
}