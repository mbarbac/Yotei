namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_Transaction_Default
{
    //[Enforced]
    [Fact]
    public static void Test_Exist_Valid()
    {
        Connection connection;
        Transaction transaction;
        var engine = new FakeEngine();

        using (connection = new FakeConnection(engine))
        {
            transaction = connection.Transaction;

            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
            Assert.False(transaction.IsDisposed);

            transaction.Dispose();
            Assert.True(transaction.IsDisposed);
            
            transaction = connection.Transaction;
            Assert.False(transaction.IsDisposed);
        }

        Assert.True(connection.IsDisposed);
        Assert.True(transaction.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Commit()
    {
        Connection connection;
        Transaction transaction;
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
                Assert.Equal(i, transaction.Level);
            }
            for (int i = max - 1; i >= 0; i--)
            {
                connection.Transaction.Commit();
                Assert.Equal(i > 0, connection.IsOpen);
                Assert.Equal(i > 0, transaction.IsActive);
                Assert.Equal(i, transaction.Level);
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
        Connection connection;
        Transaction transaction;
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
                Assert.Equal(i, transaction.Level);
            }

            transaction.Abort();
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
            Assert.False(transaction.IsDisposed);
        }

        Assert.True(connection.IsDisposed);
        Assert.True(transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Implicit_Abort()
    {
        Connection connection;
        Transaction transaction;
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
                Assert.Equal(i, transaction.Level);
            }
        }

        Assert.True(connection.IsDisposed);
        Assert.True(transaction.IsDisposed);
    }
}