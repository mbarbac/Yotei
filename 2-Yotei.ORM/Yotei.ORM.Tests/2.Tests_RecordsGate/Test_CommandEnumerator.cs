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
        var command = new FakeCommand(connection);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        List<IRecord> items = [];
        foreach (var item in command) items.Add(item!);

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
        var command = new FakeCommand(connection);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        List<IRecord> items = [];
        await foreach (var item in command.ConfigureAwait(false)) items.Add(item!);

        Assert.Equal(3, items.Count);
        for (int i = 0; i < 3; i++) Assert.Equal(command.FakeSchema, items[i].Schema!);
        Assert.Equal("007", items[0][0]);
        Assert.Equal("SP1", items[1][0]);
        Assert.Equal("SP2", items[2][0]);
    }

    //[Enforced]
    [Fact]
    public static async Task Test_Async_Enumerate_Cancellation()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

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
        using var source = new CancellationTokenSource(ms * 2);

        List<IRecord> items = [];
        try
        {
            await foreach (var item in command
                .WithCancellation(source.Token)
                .ConfigureAwait(false)) items.Add(item!);

            Assert.Fail();
        }
        catch (OperationCanceledException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ToList()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

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
    [SuppressMessage("", "IDE0079")]
    [SuppressMessage("", "xUnit1030")]
    public static async Task Test_ToListAsync()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        var items = await command.ToListAsync().ConfigureAwait(false);
        Assert.Equal(3, items.Count);
        for (int i = 0; i < 3; i++) Assert.Equal(command.FakeSchema, items[i].Schema!);
        Assert.Equal("007", items[0][0]);
        Assert.Equal("SP1", items[1][0]);
        Assert.Equal("SP2", items[2][0]);
    }

    //[Enforced]
    [Fact]
    [SuppressMessage("", "IDE0079")]
    [SuppressMessage("", "xUnit1030")]
    public static async Task Test_ToListAsync_Cancellation()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

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
        using var source = new CancellationTokenSource(ms * 2);

        try { await command.ToListAsync(source.Token).ConfigureAwait(false); Assert.Fail(); }
        catch (OperationCanceledException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_First()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

        var item = command.First();
        Assert.Null(item);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        item = command.First();
        Assert.NotNull(item);
        Assert.Equal("007", item[0]);
    }

    //[Enforced]
    [Fact]
    [SuppressMessage("", "IDE0079")]
    [SuppressMessage("", "xUnit1030")]
    public static async Task Test_FirstAsync()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

        var item = await command.FirstAsync();
        Assert.Null(item);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        item = await command.FirstAsync().ConfigureAwait(false);
        Assert.NotNull(item);
        Assert.Equal("007", item[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Last()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

        var item = command.Last();
        Assert.Null(item);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        item = command.Last();
        Assert.NotNull(item);
        Assert.Equal("SP2", item[0]);
    }

    //[Enforced]
    [Fact]
    [SuppressMessage("", "IDE0079")]
    [SuppressMessage("", "xUnit1030")]
    public static async Task Test_LastAsync()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

        var item = await command.LastAsync();
        Assert.Null(item);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        item = await command.LastAsync().ConfigureAwait(false);
        Assert.NotNull(item);
        Assert.Equal("SP2", item[0]);
    }

    //[Enforced]
    [Fact]
    [SuppressMessage("", "IDE0079")]
    [SuppressMessage("", "xUnit1030")]
    public static async Task Test_LastAsync_Cancellation()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

        var item = await command.LastAsync();
        Assert.Null(item);

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
        using var source = new CancellationTokenSource(ms * 2);

        try { await command.LastAsync(source.Token).ConfigureAwait(false); Assert.Fail(); }
        catch (OperationCanceledException) { }
    }
}