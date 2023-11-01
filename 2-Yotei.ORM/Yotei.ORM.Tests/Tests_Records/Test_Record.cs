using Record = Yotei.ORM.Records.Code.Record;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Record
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var record = new Record(engine);
        Assert.Empty(record);
        Assert.Empty(record.Schema);
    }
}