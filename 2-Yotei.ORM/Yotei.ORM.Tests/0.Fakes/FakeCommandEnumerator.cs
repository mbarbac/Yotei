namespace Yotei.ORM.Tests;

// ========================================================
public class FakeCommandEnumerator : CommandEnumerator
{
    public FakeCommandEnumerator(
        IEnumerableCommand command, CancellationToken token = default) : base(command, token) { }

    int Index = 0;
    public List<IRecord> FakeRecords { get; } = [];
    public ISchema? FakeSchema { get; set; }

    // ----------------------------------------------------

    protected override ISchema? OnInitialize() => FakeSchema;
    protected override IRecord? OnNextResult()
    {
        if (Index < FakeRecords.Count) return FakeRecords[Index++];
        return null;
    }
    protected override void OnTerminate() => Index = 0;
    protected override void OnAbort() => Index = 0;

    // ----------------------------------------------------

    protected override ValueTask<ISchema?> OnInitializeAsync() => ValueTask.FromResult(FakeSchema);
    protected override ValueTask<IRecord?> OnNextResultAsync()
    {
        if (Index < FakeRecords.Count)
        {
            var value = FakeRecords[Index++];
            return ValueTask.FromResult<IRecord?>(value);
        }
        return ValueTask.FromResult<IRecord?>(null);
    }
    protected override ValueTask OnTerminateAsync() { Index = 0; return ValueTask.CompletedTask; }
    protected override ValueTask OnAbortAsync() { Index = 0; return ValueTask.CompletedTask; }
}