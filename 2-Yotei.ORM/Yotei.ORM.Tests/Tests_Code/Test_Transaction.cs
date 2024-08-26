namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Transaction
{
    //[Enforced]
    [Fact]
    public static void Test_Creation()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            using (transaction = new FakeTransaction(connection))
            {
                Assert.False(connection.IsOpen);
                Assert.False(connection.IsDisposed);

                Assert.Equal(0, transaction.Level);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);
            }

            Assert.Equal(0, transaction.Level);
            Assert.False(transaction.IsActive);
            Assert.True(transaction.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Disposed_When_Connection_Disposed()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            transaction = new FakeTransaction(connection);
            Assert.Equal(0, transaction.Level);
            Assert.False(transaction.IsActive);
            Assert.False(transaction.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);

        Assert.Equal(0, transaction.Level);
        Assert.False(transaction.IsActive);
        Assert.False(transaction.IsDisposed);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Start_Commit()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            int count = 3;
            transaction = new FakeTransaction(connection);

            for (int i = 1; i <= count; ++i)
            {
                transaction.Start();

                Assert.True(connection.IsOpen);
                Assert.False(connection.IsDisposed);

                Assert.False(transaction.IsDisposed);
                Assert.True(transaction.IsActive);
                Assert.Equal(i, transaction.Level);
            }
            for (int i = count; i > 0; --i)
            {
                transaction.Commit();

                if (i > 1)
                {
                    Assert.True(connection.IsOpen);
                    Assert.False(connection.IsDisposed);

                    Assert.False(transaction.IsDisposed);
                    Assert.True(transaction.IsActive);
                    Assert.Equal(i - 1, transaction.Level);
                }
                else
                {
                    Assert.False(connection.IsOpen);
                    Assert.False(connection.IsDisposed);

                    Assert.False(transaction.IsDisposed);
                    Assert.False(transaction.IsActive);
                    Assert.Equal(0, transaction.Level);
                }
            }
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Abort()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            int count = 3;
            transaction = new FakeTransaction(connection);

            for (int i = 1; i <= count; ++i)
            {
                transaction.Start();

                Assert.True(connection.IsOpen);
                Assert.False(connection.IsDisposed);

                Assert.False(transaction.IsDisposed);
                Assert.True(transaction.IsActive);
                Assert.Equal(i, transaction.Level);
            }

            transaction.Abort();

            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            Assert.False(transaction.IsDisposed);
            Assert.False(transaction.IsActive);
            Assert.Equal(0, transaction.Level);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_And_Dispose_Transaction()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            using (transaction = new FakeTransaction(connection))
            {
                int count = 3;
                for (int i = 1; i <= count; ++i)
                {
                    transaction.Start();

                    Assert.True(connection.IsOpen);
                    Assert.False(connection.IsDisposed);

                    Assert.False(transaction.IsDisposed);
                    Assert.True(transaction.IsActive);
                    Assert.Equal(i, transaction.Level);
                }
            }

            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            Assert.True(transaction.IsDisposed);
            Assert.False(transaction.IsActive);
            Assert.Equal(0, transaction.Level);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_And_Dispose_Connection()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            transaction = new FakeTransaction(connection);
            int count = 3;

            for (int i = 1; i <= count; ++i)
            {
                transaction.Start();

                Assert.True(connection.IsOpen);
                Assert.False(connection.IsDisposed);

                Assert.False(transaction.IsDisposed);
                Assert.True(transaction.IsActive);
                Assert.Equal(i, transaction.Level);
            }
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);

        // Despite the connection is disposed, the transaction instance is not aborted and not
        // disposed, because it is not the one the connection keeps track of. Hence why the user
        // code shall always use the connection.Transaction property.

        Assert.False(transaction.IsDisposed);
        Assert.True(transaction.IsActive);
        Assert.Equal(3, transaction.Level);
    }
}