namespace Yotei.ORM.Tests;

// ========================================================
public class FakeTransaction : Transaction
{
    public FakeTransaction(IConnection connection) : base(connection) => Instance = Interlocked.Increment(ref Sequence);
    public override string ToString() => $"#{Instance}:FakeTransaction({Connection})";

    long Instance { get; }
    static long Sequence = 0;

    protected override void OnStart() { }
    protected override ValueTask OnStartAsync(CancellationToken _) => ValueTask.CompletedTask;
    protected override void OnCommit() { }
    protected override ValueTask OnCommitAsync(CancellationToken _) => ValueTask.CompletedTask;
    protected override void OnAbort() { }
    protected override ValueTask OnAbortAsync() => ValueTask.CompletedTask;
}