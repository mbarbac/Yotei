namespace Yotei.ORM.Tests;

// ========================================================
public class FakeRecordsGate : RecordsGate
{
    public FakeRecordsGate(IConnection connection) : base(connection) { }
}