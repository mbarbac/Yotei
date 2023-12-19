namespace Yotei.ORM.Relational.Tests;

// ========================================================
//[Enforced]
public static class Test_Connection
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new Code.Connection(engine))
        {
            Assert.Null(connection.ConnectionString);
            Assert.Null(connection.Server);
            Assert.Null(connection.Database);
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
        }

        using (connection = new Code.Connection(engine, DB.ConnectionString))
        {
            Assert.Equal("(localdb)\\MSSQLLocalDB", connection.Server);
            Assert.Equal("KeroseneDB", connection.Database);
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Close()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new Code.Connection(engine, DB.ConnectionString))
        {
            Assert.False(connection.IsOpen);

            connection.Open(); Assert.True(connection.IsOpen);
            connection.Close(); Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Dispose()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new Code.Connection(engine, DB.ConnectionString))
        {
            Assert.False(connection.IsOpen);

            connection.Open(); Assert.True(connection.IsOpen);
            Assert.False(connection.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Invalid_Server()
    {
        var engine = new FakeEngine();
        var str = "Data Source = (localdb)\\whatever; Initial Catalog = KeroseneDB;";

        using (var connection = new Code.Connection(engine, str) { Retries = 0 })
        {
            try { connection.Open(); Assert.Fail(); }
            catch (SqlException) { }
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Invalid_Database()
    {
        var engine = new FakeEngine();
        var str = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = whatever;";

        using (var connection = new Code.Connection(engine, str) { Retries = 0 })
        {
            try { connection.Open(); Assert.Fail(); }
            catch (SqlException) { }
        }
    }
}