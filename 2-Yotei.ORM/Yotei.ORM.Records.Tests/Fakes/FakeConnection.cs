namespace Yotei.ORM.Records.Tests;

// ========================================================
[Cloneable]
public partial class FakeConnection : Code.Connection, IConnection
{
    public FakeConnection(IEngine engine) : base(engine) { }
    protected FakeConnection(FakeConnection source) : base(source) { }
    public override string ToString() => $"FakeConnection({Engine})";

    public override bool IsOpen => _IsOpen;
    bool _IsOpen = false;

    protected override void OnOpen() => _IsOpen = true;
    protected override ValueTask OnOpenAsync(CancellationToken _) { OnOpen(); return ValueTask.CompletedTask; }
    protected override void OnClose() => _IsOpen = false;
    protected override ValueTask OnCloseAsync() { OnClose(); return ValueTask.CompletedTask; }

    protected override ITransaction CreateTransaction() => new FakeTransaction(this);

    public override IRecordMethods Records => new FakeRecordMethods(this);
}