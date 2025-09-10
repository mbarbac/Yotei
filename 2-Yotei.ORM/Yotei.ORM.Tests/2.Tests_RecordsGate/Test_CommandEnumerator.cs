namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_CommandEnumerator
{
    //[Enforced]
    [Fact]
    public static void Test_Simple_Enumerate()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeEnumerableCommand(connection);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        var items = command.ToList();
        Assert.Equal(3, items.Count);
        for (int i = 0; i < 3; i++) Assert.Equal(command.FakeSchema, items[i].Schema!);
        Assert.Equal("007", items[0][0]);
        Assert.Equal("SP1", items[1][0]);
        Assert.Equal("SP2", items[2][0]);
    }

    //[Enforced]
    [Fact]
    public static async Task Test_Async_Enumerate()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeEnumerableCommand(connection);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        var source = new CancellationTokenSource();
        var items = await command.ToListAsync(source.Token);
        Assert.Equal(3, items.Count);
        for (int i = 0; i < 3; i++) Assert.Equal(command.FakeSchema, items[i].Schema!);
        Assert.Equal("007", items[0][0]);
        Assert.Equal("SP1", items[1][0]);
        Assert.Equal("SP2", items[2][0]);
    }

    //[Enforced]
    [Fact]
    public static async Task Test_Async_Enumerate_Cancelled()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeEnumerableCommand(connection);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        var ms = 150;
        command.FakeDelayMs = ms;

        var source = new CancellationTokenSource(ms * 2);
        try { await command.ToListAsync(source.Token); Assert.Fail(); }
        catch (OperationCanceledException) { }
    }
}