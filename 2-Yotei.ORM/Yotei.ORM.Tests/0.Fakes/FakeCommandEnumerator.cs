namespace Yotei.ORM.Tests;

// ========================================================
public class FakeCommandEnumerator : CommandEnumerator
{
    int Index = 0;
    public List<IRecord> FakeRecords { get; } = [];
    public ISchema? FakeSchema { get; set; }

    public FakeCommandEnumerator(
        IEnumerableCommand command, CancellationToken token = default) : base(command, token) { }

    // ----------------------------------------------------

    protected override ISchema? OnInitialize() => FakeSchema;
    protected override IRecord? OnNextResult()
    {
        return Index < FakeRecords.Count ? FakeRecords[Index++] : null;
    }
    protected override void OnTerminate() => Index = 0;
    protected override void OnAbort() => Index = 0;

    // ----------------------------------------------------

    protected override ValueTask<ISchema?> OnInitializeAsync() => ValueTask.FromResult(FakeSchema);
    protected override ValueTask<IRecord?> OnNextResultAsync() => ValueTask.FromResult(OnNextResult());
    protected override ValueTask OnTerminateAsync() { Index = 0; return ValueTask.CompletedTask; }
    protected override ValueTask OnAbortAsync() { Index = 0; return ValueTask.CompletedTask; }
}