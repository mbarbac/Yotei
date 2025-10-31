namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public class Test_Transaction
{
    //[Enforced]
    [Fact]
    public void Test_Default_Always_Valid()
    {
        IConnection connection;
        ITransaction transaction;
        var engine = new FakeEngine();

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            transaction = connection.Transaction;
            Assert.False(transaction.IsDisposed);

            transaction.Dispose();
            Assert.True(transaction.IsDisposed);
            Assert.False(connection.Transaction.IsDisposed);
        }

        Assert.True(connection.IsDisposed);
        Assert.True(transaction.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Start_Commit()
    {
        IConnection connection;
        ITransaction transaction;
        var engine = new FakeEngine();

        using (connection = new FakeConnection(engine))
        {
            transaction = connection.Transaction;

            Assert.False(connection.IsOpen);
            Assert.False(transaction.IsActive);

            var max = 3; for (int i = 1; i <= max; i++)
            {
                transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(transaction.IsActive);
                Assert.Equal(i, (transaction as FakeTransaction)?.Level);
            }
            for (int i = max - 1; i >= 0; i--)
            {
                connection.Transaction.Commit();
                Assert.Equal(i > 0, connection.IsOpen);
                Assert.Equal(i > 0, transaction.IsActive);
                Assert.Equal(i, (transaction as FakeTransaction)?.Level);
            }

            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
            Assert.False(transaction.IsDisposed);
        }

        Assert.True(connection.IsDisposed);
        Assert.True(transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Explicit_Abort()
    {
        IConnection connection;
        ITransaction transaction;
        var engine = new FakeEngine();

        using (connection = new FakeConnection(engine))
        {
            transaction = connection.Transaction;

            Assert.False(connection.IsOpen);
            Assert.False(transaction.IsActive);

            var max = 3; for (int i = 1; i <= max; i++)
            {
                transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(transaction.IsActive);
                Assert.Equal(i, (transaction as FakeTransaction)?.Level);
            }

            transaction.Abort();
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
            Assert.False(transaction.IsDisposed);
        }

        Assert.True(connection.IsDisposed);
        Assert.True(transaction.IsDisposed);
    }
}