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

        var fakes = new FakeConverters();
        source.ToDatabaseConverters.AddRange(fakes);

        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(2, target.ToDatabaseConverters.Count);
        Assert.NotNull(target.ToDatabaseConverters.Find<DateTime>());
        Assert.NotNull(target.ToDatabaseConverters.Find<DateOnly>());
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

            connection.Close();
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Close_From_Transaction()
    {
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(new FakeEngine()))
        {
            Assert.False(connection.IsOpen);

            using (transaction = connection.CreateTransaction())
            {
                transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(transaction.IsActive);

                transaction.Commit();
                Assert.False(connection.IsOpen);
                Assert.False(connection.IsDisposed);
                Assert.False(transaction.IsActive);
            }
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
        Assert.True(transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Close_From_Transaction_Implicit_Abort()
    {
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(new FakeEngine()))
        {
            Assert.False(connection.IsOpen);

            using (transaction = connection.CreateTransaction())
            {
                transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(transaction.IsActive);
            }

            Assert.False(transaction.IsActive);
            Assert.True(transaction.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }
}