namespace Yotei.ORM.Tests;

// ========================================================
public class FakeTransaction : Transaction
{
    [SuppressMessage("", "IDE0290")]
    public FakeTransaction(IConnection connection) : base(connection) { }
    public override string ToString() => $"FakeTransaction({Connection})";

    public bool IsAborted { get; private set; }

    protected override void OnStart() { }
    protected override ValueTask OnStartAsync(CancellationToken _) => ValueTask.CompletedTask;
    protected override void OnCommit() { }
    protected override ValueTask OnCommitAsync(CancellationToken _) => ValueTask.CompletedTask;
    protected override void OnAbort() { IsAborted = true; }
    protected override ValueTask OnAbortAsync() { OnAbort(); return ValueTask.CompletedTask; }
}