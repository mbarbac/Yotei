namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable]
public partial class FakeConnection : Code.Connection
{
    public FakeConnection(IEngine engine) : base(engine)
    {
        ToDatabase.Add(
            new ValueConverter<DateOnly, DateTime>((x, _) => x.ToDateTime(new TimeOnly())));

        ToDatabase.Add(
            new ValueConverter<DateTime, DateOnly>((x, _) => new DateOnly(x.Year, x.Month, x.Day)));
    }
    protected FakeConnection(FakeConnection source) : base(source) { }
    public override string ToString() => $"FakeConnection({Engine})";

    public override bool IsOpen => _IsOpen;
    bool _IsOpen = false;

    protected override void OnOpen() => _IsOpen = true;
    protected override ValueTask OnOpenAsync(CancellationToken _) { OnOpen(); return ValueTask.CompletedTask; }
    protected override void OnClose() => _IsOpen = false;
    protected override ValueTask OnCloseAsync() { OnClose(); return ValueTask.CompletedTask; }

    public override ITransaction CreateTransaction() => new FakeTransaction(this);

    protected override IRecordsGate CreateRecordsGate() => new FakeRecordsGate(this);
}