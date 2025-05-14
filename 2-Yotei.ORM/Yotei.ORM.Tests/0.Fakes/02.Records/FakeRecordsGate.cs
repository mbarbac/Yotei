namespace Yotei.ORM.Tests;

// ========================================================
public class FakeRecordsGate : RecordsGate
{
    public FakeRecordsGate(IConnection connection) : base(connection) { }

    public override ICommandEnumerator CreateCommandEnumerator(
        IEnumerableCommand command, CancellationToken token = default)
        => throw null;

    public override ICommandExecutor CreateCommandExecutor(
        IExecutableCommand command)
        => throw null;
}