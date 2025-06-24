using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using Record = Yotei.ORM.Records.Code.Record;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_CommandEnumerator
{
    public record Person (string id, string first, string last, string country)
    {
        public override string ToString() => $"{Id}, {First} {Last}, {Country}";
        public string Id { get; init; } = id.NotNullNotEmpty();
        public string First { get; init; } = first.NotNullNotEmpty();
        public string Last { get; init; } = last.NotNullNotEmpty();
        public string Country { get; init; } = country.NotNullNotEmpty();
    }

    //[Enforced]
    [Fact]
    public static void Test()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = connection.Records.Raw();

        var iter = command.SelectItems(x => new Person(
            (string)x["Employee.Id"]!,
            (string)x["Employee.First"]!,
            (string)x["Last"]!,
            (string)x["Country.Id"]!));

        var inner = (FakeCommandEnumerator)iter.Enumerator;

        var schema = inner.FakeSchema = new Schema(engine, [
            new SchemaEntry(engine, "Employee.Id", isPrimaryKey: true),
            new SchemaEntry(engine, "Employee.First"),
            new SchemaEntry(engine, "Last"),
            new SchemaEntry(engine, "Country.Id")]);

        inner.FakeRecords.Add(new Record(["007", "James", "Bond", "UK"]) { Schema = schema });
        inner.FakeRecords.Add(new Record(["001", "Miguel", "Cervantes", "SP"]) { Schema = schema });
        inner.FakeRecords.Add(new Record(["002", "Diego", "Velazquez", "SP"]) { Schema = schema });
        inner.FakeRecords.Add(new Record(["003", "Francisco", "Quevedo", "SP"]) { Schema = schema });

        var items = iter.ToList();
        Assert.Equal(4, items.Count);
        Assert.Equal("Bond", items[0]!.Last);
        Assert.Equal("Cervantes", items[1]!.Last);
        Assert.Equal("Velazquez", items[2]!.Last);
        Assert.Equal("Quevedo", items[3]!.Last);
    }
}