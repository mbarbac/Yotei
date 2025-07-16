using Record = Yotei.ORM.Records.Code.Record;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_CommandEnumerator
{
    //[Enforced]
    [Fact]
    public static async Task Test_As_Enumerable_With_CancellationToken()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeEnumerableCommand(connection);
        var records = new List<Record> { new(["a1"]), new(["a2"]) };
        var schema = new Schema(engine, [new SchemaEntry(engine, "Id")]);

        var iter = new FakeCommandEnumerator(command);
        iter.FakeRecords.AddRange(records);
        iter.FakeSchema = schema;

        var cts = new CancellationTokenSource(50 * 1000);
        var token = cts.Token;

        var results = new List<IRecord>();
        await foreach (var record in iter.WithCancellation(token))
            if (record is not null) results.Add(record);

        Assert.Equal(records.Count, results.Count);

        return;
    }

    // ----------------------------------------------------
}