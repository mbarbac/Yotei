using Record = Yotei.ORM.Records.Code.Record;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_CommandEnumerator
{
    //[Enforced]
    [Fact]
    public static void Test_()
    {
        var connection = new FakeConnection(new FakeEngine());
        var command = new FakeCommand(connection);
        var records = new Record[] { new(["a1"]), new(["a2"]) };

        var iter = new FakeCommandEnumerator(command);
    }
}