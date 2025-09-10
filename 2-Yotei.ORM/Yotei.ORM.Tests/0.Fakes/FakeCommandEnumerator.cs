using Record = Yotei.ORM.Records.Code.Record;

namespace Yotei.ORM.Tests;

// ========================================================
public class FakeCommandEnumerator : CommandEnumerator
{
    int Index = 0;
    public object?[][] FakeArrays { get; set; } = [];
    public ISchema? FakeSchema { get; set; }
    public int FakeDelayMs { get; set; }

    // ----------------------------------------------------

    public FakeCommandEnumerator(IEnumerableCommand command, CancellationToken token = default)
        : base(command, token)
    {
        if (command is FakeEnumerableCommand temp)
        {
            FakeArrays = temp.FakeArrays;
            FakeSchema = temp.FakeSchema;
            FakeDelayMs = temp.FakeDelayMs;
        }
    }

    // ----------------------------------------------------

    protected override ISchema? OnInitialize() => FakeSchema;
    protected override IRecord? OnNextResult()
    {
        if (Index >= FakeArrays.Length) return null;

        var values = FakeArrays[Index++];
        if (FakeSchema is null) return new Record(values);

        var engine = FakeSchema.Engine;
        return new Record(engine, values, FakeSchema);
    }
    protected override void OnTerminate() => Index = 0;
    protected override void OnAbort() => Index = 0;

    // ----------------------------------------------------

    protected override ValueTask<ISchema?> OnInitializeAsync() => ValueTask.FromResult(OnInitialize());
    protected override async ValueTask<IRecord?> OnNextResultAsync()
    {
        if (FakeDelayMs > 0) await Task.Delay(FakeDelayMs);
        return OnNextResult();
    }
    protected override ValueTask OnTerminateAsync() { Index = 0; return ValueTask.CompletedTask; }
    protected override ValueTask OnAbortAsync() { Index = 0; return ValueTask.CompletedTask; }
}