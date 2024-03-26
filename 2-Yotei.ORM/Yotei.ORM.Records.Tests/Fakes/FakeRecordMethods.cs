namespace Yotei.ORM.Records.Tests;

// ========================================================
public class FakeRecordMethods(IConnection connection) : RecordMethods(connection)
{
    protected override ICommandEnumerator DoCreateCommandEnumerator(
        IEnumerableCommand command, CancellationToken token)
        => new FakeEnumerator(command, token);

    protected override ICommandExecutor DoCreateCommandExecutor(
        IExecutableCommand command)
        => new FakeExecutor(command);
}