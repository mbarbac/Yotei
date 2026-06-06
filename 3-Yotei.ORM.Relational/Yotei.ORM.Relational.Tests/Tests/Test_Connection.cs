#pragma warning disable CA1859

namespace Yotei.ORM.Relational.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Connection
{
    const string SERVER = "(LocalDB)\\MSSQLLocalDB";
    const string DATABASE = "YoteiDB";
    const string CNSTRING = "Server=(LocalDB)\\MSSQLLocalDB;Database=YoteiDB;";

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();
        using var cn = new Code.Connection(engine);

        Assert.Equal(ORM.Code.Connection.RETRIES, cn.Retries);
        Assert.Equal(ORM.Code.Connection.RETRYINTERVALMS, cn.RetryInterval.TotalMilliseconds);
        Assert.Equal(ORM.Code.Connection.LOCKTIMEOUTSECS, cn.LockTimeout.TotalSeconds);
        Assert.False(cn.IsOpen);
        Assert.Null(cn.Transaction);
        Assert.Equal(Code.Connection.ISOLATIONLEVEL, cn.IsolationLevel);

        Assert.Null(cn.ConnectionString);
        Assert.Null(cn.Server);
        Assert.Null(cn.Database);
        Assert.Null(cn.DbConnection);
    }

    //[Enforced]
    [Fact]
    public static void Test_Set_ConnectionString()
    {
        var engine = new FakeEngine();
        using var cn = new Code.Connection(engine);

        var str = "Server=Any;Database=Other";
        cn.ConnectionString = str;

        Assert.Equal("Data Source=Any;Initial Catalog=Other", cn.ConnectionString);
        Assert.Equal("Any", cn.Server);
        Assert.Equal("Other", cn.Database);
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

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Open_Close_Explicit()
    {
        var engine = new FakeEngine();
        using var cn = new Code.Connection(engine, CNSTRING);

        Assert.False(cn.IsOpen);
        cn.Open(); Assert.True(cn.IsOpen);
        cn.Close(); Assert.False(cn.IsOpen);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Close_Implicit()
    {
        IConnection cn;
        var engine = new FakeEngine();

        using (cn = new Code.Connection(engine, CNSTRING))
        {
            Assert.False(cn.IsOpen);
            cn.Open(); Assert.True(cn.IsOpen);
        }
        Assert.False(cn.IsOpen);
        Assert.True(cn.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Invalid_Server()
    {
        var engine = new FakeEngine();
        using var cn = new Code.Connection(
            engine,
            $"Server=Any;Database={DATABASE};Connect Timeout=1")
        {
            Retries = 0,
            RetryInterval = TimeSpan.FromMilliseconds(0),
            LockTimeout = TimeSpan.FromMilliseconds(0),
        };
        try { cn.Open(); Assert.Fail(); } catch (SqlException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Invalid_Database()
    {
        var engine = new FakeEngine();
        using var cn = new Code.Connection(
            engine,
            $"Server={SERVER};Database=Other;Connect Timeout=1")
        {
            Retries = 0,
            RetryInterval = TimeSpan.FromMilliseconds(0),
            LockTimeout = TimeSpan.FromMilliseconds(0),
        };
        try { cn.Open(); Assert.Fail(); } catch (SqlException) { }
    }
}