namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_CommandEnumerator_T
{
    public record Persona(string Id, string Fist, string Last) { }
    public static Persona ToPersona(IRecord record)
        => new((string)record[0]!, (string)record[1]!, (string)record[2]!);

    // ----------------------------------------------------

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

        List<Persona> items = [];
        foreach (var item in command.WithConverter(ToPersona)) items.Add(item!);

        Assert.Equal(3, items.Count);
        Assert.Equal("007", items[0].Id);
        Assert.Equal("SP1", items[1].Id);
        Assert.Equal("SP2", items[2].Id);
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

        List<Persona> items = [];
        await foreach (var item in command
            .WithConverter(ToPersona)
            .ConfigureAwait(false))
            items.Add(item!);

        Assert.Equal(3, items.Count);
        Assert.Equal("007", items[0].Id);
        Assert.Equal("SP1", items[1].Id);
        Assert.Equal("SP2", items[2].Id);
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

        List<Persona> items = [];
        try
        {
            await foreach (var item in command
                .WithConverter(ToPersona)
                .WithCancellation(source.Token)
                .ConfigureAwait(false))
                items.Add(item!);

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

        var items = command.WithConverter(ToPersona).ToList();
        Assert.Equal(3, items.Count);
        Assert.Equal("007", items[0].Id);
        Assert.Equal("SP1", items[1].Id);
        Assert.Equal("SP2", items[2].Id);
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

        var items = await command
            .WithConverter(ToPersona)
            .ToListAsync()
            .ConfigureAwait(false);

        Assert.Equal(3, items.Count);
        Assert.Equal("007", items[0].Id);
        Assert.Equal("SP1", items[1].Id);
        Assert.Equal("SP2", items[2].Id);
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

        try
        {
            await command
                .WithConverter(ToPersona)
                .ToListAsync(source.Token)
                .ConfigureAwait(false);

            Assert.Fail();
        }
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

        var item = command.WithConverter(ToPersona).FirstOrDefault();
        Assert.Null(item);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        item = command.WithConverter(ToPersona).FirstOrDefault();
        Assert.NotNull(item);
        Assert.Equal("007", item.Id);
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

        var item = await command.WithConverter(ToPersona).FirstOrDefaultAsync();
        Assert.Null(item);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        item = await command.WithConverter(ToPersona).FirstOrDefaultAsync().ConfigureAwait(false);
        Assert.NotNull(item);
        Assert.Equal("007", item.Id);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Last()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

        var item = command.WithConverter(ToPersona).LastOrDefault();
        Assert.Null(item);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        item = command.WithConverter(ToPersona).LastOrDefault();
        Assert.NotNull(item);
        Assert.Equal("SP2", item.Id);
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

        var item = await command.WithConverter(ToPersona).LastOrDefaultAsync();
        Assert.Null(item);

        var xid = new SchemaEntry(engine, "Employees.Id", isPrimaryKey: true);
        var xfirst = new SchemaEntry(engine, "Employees.FirstName");
        var xlast = new SchemaEntry(engine, "LastName");
        command.FakeSchema = new Schema(engine, [xid, xfirst, xlast]);
        command.FakeArrays = [
            ["007", "James", "Bond"],
            ["SP1", "Miguel", "Cervantes"],
            ["SP2", "Diego", "Velazquez"]];

        item = await command.WithConverter(ToPersona).LastOrDefaultAsync().ConfigureAwait(false);
        Assert.NotNull(item);
        Assert.Equal("SP2", item.Id);
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

        var item = await command.WithConverter(ToPersona).LastOrDefaultAsync();
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

        try
        {
            await command
                .WithConverter(ToPersona)
                .LastOrDefaultAsync(source.Token)
                .ConfigureAwait(false);

            Assert.Fail();
        }
        catch (OperationCanceledException) { }
    }
}