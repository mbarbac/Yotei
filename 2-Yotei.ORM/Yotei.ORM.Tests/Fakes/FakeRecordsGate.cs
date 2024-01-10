namespace Yotei.ORM.Tests;

// ========================================================
public class FakeRecordsGate : Records.Code.RecordsGate
{
    [SuppressMessage("", "IDE0290")]
    public FakeRecordsGate(IConnection connection) : base(connection) { }
}