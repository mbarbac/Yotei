using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using DB = Yotei.ORM.Relational.FakeDB.DB;

namespace Yotei.ORM.Relational.Tests.Core;

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
            Assert.Equal(Code.Transaction.ISOLATIONLEVEL, connection.IsolationLevel);
            Assert.Null(connection.ConnectionString);
            Assert.Null(connection.Server);
            Assert.Null(connection.Database);

            Assert.Null(connection.DbConnection);
            //Assert.NotNull(connection.Transaction);
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
        }

        using (connection = new Code.Connection(engine, DB.ConnectionString))
        {
            Assert.Equal("(localdb)\\MSSQLLocalDB", connection.Server);
            Assert.Equal("YoteiDB", connection.Database);

            Assert.Null(connection.DbConnection);
            //Assert.NotNull(connection.Transaction);
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new Code.Connection(engine, DB.ConnectionString);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Retries, target.Retries);
        Assert.Equal(source.RetryInterval, target.RetryInterval);
        Assert.NotSame(source.ToDatabase, target.ToDatabase);
        //Assert.NotSame(source.Transaction, target.Transaction);

        Assert.Equal(source.ConnectionString, target.ConnectionString);
        Assert.Equal(source.Server, target.Server);
        Assert.Equal(source.Database, target.Database);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Open_Close()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new Code.Connection(engine, DB.ConnectionString))
        {
            Assert.False(connection.IsOpen);

            connection.Open();
            Assert.True(connection.IsOpen);
            //Assert.False(connection.Transaction.IsActive);

            connection.Close();
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
            //Assert.False(connection.Transaction.IsActive);
            //Assert.False(connection.Transaction.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
        //Assert.True(connection.Transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Invalid_Server()
    {
        var engine = new FakeEngine();
        var cnstr =
            "Data Source = (localdb)\\MSSQLLocalDB;" +
            "Initial Catalog = whatever;" +
            "Connect Timeout = 5;";

        using (var connection = new Code.Connection(engine, cnstr) { Retries = 2 })
        {
            try { connection.Open(); Assert.Fail(); }
            catch (SqlException) { }
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Invalid_Database()
    {
        var engine = new FakeEngine();
        var cnstr =
            "Data Source = (localdb)\\MSSQLLocalDB;" +
            "Initial Catalog = whatever;" +
            "Connect Timeout = 5;";

        using (var connection = new Code.Connection(engine, cnstr) { Retries = 2 })
        {
            try { connection.Open(); Assert.Fail(); }
            catch (SqlException) { }
        }
    }
}