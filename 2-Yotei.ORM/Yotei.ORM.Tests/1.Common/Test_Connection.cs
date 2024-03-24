#pragma warning disable CA1859

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Connection
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);

        Assert.Equal(Code.Connection.RETRIES, connection.Retries);
        Assert.Equal(Code.Connection.RETRYINTERVAL, connection.RetryInterval.Milliseconds);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new FakeConnection(engine);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Retries, target.Retries);
        Assert.Equal(source.RetryInterval, target.RetryInterval);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Open_Close()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);

            connection.Open();
            Assert.True(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);

            connection.Close();
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
            Assert.False(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Close_With_Active_Transaction()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);

            connection.Open();
            Assert.True(connection.IsOpen);

            connection.Transaction.Start();
            Assert.True(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);

            connection.Close();
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            Assert.False(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Open_Dispose()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);

            connection.Open();
            Assert.True(connection.IsOpen);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Dispose_With_Active_Transaction()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);

            connection.Open();
            Assert.True(connection.IsOpen);

            connection.Transaction.Start();
            Assert.True(connection.Transaction.IsActive);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);

        Assert.False(connection.Transaction.IsActive);
        Assert.True(connection.Transaction.IsDisposed);
    }
}