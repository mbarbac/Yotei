namespace Yotei.ORM.Tests;

// ========================================================
public class FakeConnection : Connection
{
    public FakeConnection(Engine engine) : base(engine) { }
    protected FakeConnection(FakeConnection source) : base(source) { }
    public override string ToString() => $"FakeConnection({Engine})";

    public override FakeConnection Clone() => new(this);

    public override bool IsOpen => _IsOpen;
    bool _IsOpen = false;

    protected override void OnOpen() => _IsOpen = true;
    protected override ValueTask OnOpenAsync(CancellationToken _) { OnOpen(); return ValueTask.CompletedTask; }
    protected override void OnClose() => _IsOpen = false;
    protected override ValueTask OnCloseAsync() { OnClose(); return ValueTask.CompletedTask; }

    protected override Transaction CreateTransaction() => new FakeTransaction(this);
}