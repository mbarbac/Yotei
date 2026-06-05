namespace Yotei.ORM.Relational.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Connection
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();
        using var connection = new Code.Connection(engine);

        Assert.Equal(ORM.Code.Connection.RETRIES, connection.Retries);
        Assert.Equal(ORM.Code.Connection.RETRYINTERVALMS, connection.RetryInterval.TotalMilliseconds);
        Assert.Equal(ORM.Code.Connection.LOCKTIMEOUTSECS, connection.LockTimeout.TotalSeconds);
        Assert.False(connection.IsOpen);
        Assert.Null(connection.Transaction);
        Assert.Equal(Code.Connection.ISOLATIONLEVEL, connection.IsolationLevel);

        Assert.Null(connection.ConnectionString);
        Assert.Null(connection.Server);
        Assert.Null(connection.Database);
        Assert.Null(connection.DbConnection);
    }

    //[Enforced]
    [Fact]
    public static void Test_Set_ConnectionString()
    {
        var engine = new FakeEngine();
        using var connection = new Code.Connection(engine);

        var str = "Server=(LocalDB)\\MSSQLLocalDB;Database=YoteiDB";
        connection.ConnectionString = str;

        Assert.Equal("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=YoteiDB", connection.ConnectionString);
        Assert.Equal("(LocalDB)\\MSSQLLocalDB", connection.Server);
        Assert.Equal("YoteiDB", connection.Database);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        using var source = new Code.Connection(engine)
        {
            Retries = 99,
            RetryInterval = TimeSpan.FromSeconds(100),
            LockTimeout = TimeSpan.FromSeconds(200),
            IsolationLevel = IsolationLevel.ReadCommitted,
            ConnectionString = "Server=Any;Database=Other"
        };

        using var target = source.Clone();

        Assert.Same(source.Engine, target.Engine);
        Assert.Equal(source.Retries, target.Retries);
        Assert.Equal(source.RetryInterval, target.RetryInterval);
        Assert.Equal(source.LockTimeout, target.LockTimeout);
        Assert.Null(target.Transaction);
        Assert.Equal(source.IsolationLevel, target.IsolationLevel);

        Assert.Equal(source.ConnectionString, target.ConnectionString);
        Assert.Equal(source.Server, target.Server);
        Assert.Equal(source.Database, target.Database);
    }
}