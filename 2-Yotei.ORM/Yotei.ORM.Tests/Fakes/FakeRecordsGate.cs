namespace Yotei.ORM.Tests;

// ========================================================
public class FakeRecordsGate : Records.Code.RecordsGate
{
    [SuppressMessage("", "IDE0290")]
    public FakeRecordsGate(IConnection connection) : base(connection) { }

    public override ICommandEnumerator CreateCommandEnumerator(
        IEnumerableCommand command, CancellationToken token = default)
        => throw null;

    public override ICommandExecutor CreateCommandExecutor(
        IExecutableCommand command)
        => throw null;
}