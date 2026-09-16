#pragma warning disable CA1859

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Transaction
{
    //[Enforced]
    [Fact]
    public static void Test_Transaction_Start_Commit()
    {
        IConnection connection;
        ITransaction transaction;
        var engine = new FakeEngine() { IgnoreCase = true };

        using (connection = new FakeConnection(engine))
        {
            Assert.Null(connection.Transaction);

            transaction = connection.StartTransaction();
            Assert.NotNull(connection.Transaction);
            Assert.True(connection.Transaction.IsActive);
            Assert.True(connection.IsOpen);

            connection.Transaction.Commit();
            Assert.Null(connection.Transaction);
            Assert.False(connection.IsOpen);
        }

        Assert.False(((FakeTransaction)transaction).IsAborted);
        Assert.Null(connection.Transaction);
        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Transaction_Start_Dispose()
    {
        IConnection connection;
        ITransaction transaction;
        var engine = new FakeEngine() { IgnoreCase = true };

        using (connection = new FakeConnection(engine))
        {
            Assert.Null(connection.Transaction);

            transaction = connection.StartTransaction();
            Assert.NotNull(connection.Transaction);
            Assert.True(connection.Transaction.IsActive);
            Assert.True(connection.IsOpen);
        }

        Assert.True(((FakeTransaction)transaction).IsAborted);
        Assert.Null(connection.Transaction);
        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Transaction_Start_Abort()
    {
        IConnection connection;
        ITransaction transaction;
        var engine = new FakeEngine() { IgnoreCase = true };

        using (connection = new FakeConnection(engine))
        {
            Assert.Null(connection.Transaction);

            transaction = connection.StartTransaction();
            Assert.NotNull(connection.Transaction);
            Assert.True(connection.Transaction.IsActive);
            Assert.True(connection.IsOpen);

            connection.Transaction.Abort();
            Assert.Null(connection.Transaction);
            Assert.False(connection.IsOpen);
        }

        Assert.True(((FakeTransaction)transaction).IsAborted);
        Assert.Null(connection.Transaction);
        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }
}