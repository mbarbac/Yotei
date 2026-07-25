#pragma warning disable CA1859

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_Connection
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        using var connection = new FakeConnection(engine);

        Assert.Equal(Connection.RETRIES, connection.Retries);
        Assert.Equal(Connection.RETRYINTERVAL_MS, connection.RetryInterval.Milliseconds);
        Assert.Equal(Connection.LOCKTIMEOUT_SECS, connection.LockTimeout.Seconds);
        Assert.Null(connection.Transaction);

        Assert.False(connection.IsOpen);
        Assert.False(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        using var source = new FakeConnection(engine) { Retries = 100 };
        using var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.Retries, target.Retries);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Open_Close()
    {
        IConnection connection;
        var engine = new FakeEngine() { IgnoreCase = true };

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);

            connection.Open(); Assert.True(connection.IsOpen);
            connection.Close(); Assert.False(connection.IsOpen);
        }
        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Dispose()
    {
        IConnection connection;
        var engine = new FakeEngine() { IgnoreCase = true };

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);

            connection.Open(); Assert.True(connection.IsOpen);
        }
        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }
}