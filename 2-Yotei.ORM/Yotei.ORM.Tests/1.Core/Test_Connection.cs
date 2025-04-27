#pragma warning disable IDE0079
#pragma warning disable CA1859

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Core;

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

        Assert.Equal(Connection.RETRIES, connection.Retries);
        Assert.Equal(Connection.RETRYINTERVAL, connection.RetryInterval.Milliseconds);
        Assert.NotNull(connection.Transaction);
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
        Assert.NotSame(source.Transaction, target.Transaction);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Open_Close()
    {
        IConnection connection;

        using (connection = new FakeConnection(new FakeEngine()))
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
        Assert.True(connection.Transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Close_From_Transaction()
    {
        IConnection connection;

        using (connection = new FakeConnection(new FakeEngine()))
        {
            Assert.False(connection.IsOpen);

            connection.Transaction.Start();
            Assert.True(connection.IsOpen);
            Assert.True(connection.Transaction.IsActive);

            connection.Transaction.Commit();
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
            Assert.False(connection.Transaction.IsActive);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Close_From_Transaction_Implicit_Abort()
    {
        IConnection connection;

        using (connection = new FakeConnection(new FakeEngine()))
        {
            Assert.False(connection.IsOpen);

            connection.Transaction.Start();
            Assert.True(connection.IsOpen);
            Assert.True(connection.Transaction.IsActive);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);

        Assert.False(connection.Transaction.IsActive);
        Assert.True(connection.Transaction.IsDisposed);
    }
}