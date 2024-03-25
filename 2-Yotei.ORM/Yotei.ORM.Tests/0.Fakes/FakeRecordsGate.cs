namespace Yotei.ORM.Tests;

// ========================================================
public class FakeRecordsGate(IConnection connection) : RecordsGate(connection)
{
    protected override ICommandEnumerator DoCreateCommandEnumerator(
        IEnumerableCommand command, CancellationToken token)
        => new FakeEnumerator(command, token);

    protected override ICommandExecutor DoCreateCommandExecutor(
        IExecutableCommand command)
        => new FakeExecutor(command);
}