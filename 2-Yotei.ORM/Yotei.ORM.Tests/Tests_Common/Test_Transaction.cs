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
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);
            }

            Assert.Equal(0, ((FakeTransaction)transaction).Level);
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

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            Assert.Equal(0, ((FakeTransaction)connection.Transaction).Level);
            Assert.False(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);
        }

        Assert.Equal(0, ((FakeTransaction)connection.Transaction).Level);
        Assert.False(connection.Transaction.IsActive);
        Assert.True(connection.Transaction.IsDisposed);

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Start_Commit()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
            int count = 3;

            for (int i = 0; i < count; i++)
            {
                connection.Transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(((FakeTransaction)connection.Transaction).Level == (i + 1));
                Assert.True(connection.Transaction.IsActive);
                Assert.False(connection.Transaction.IsDisposed);
            }

            for (int i = 0; i < count; i++)
            {
                connection.Transaction.Commit();
                Assert.False(connection.Transaction.IsDisposed);

                if (i == count - 1)
                {
                    Assert.False(connection.Transaction.IsActive);
                    Assert.False(connection.Transaction.IsDisposed);
                    Assert.False(connection.IsOpen);
                }
                else
                {
                    Assert.True(connection.Transaction.IsActive);
                    Assert.False(connection.Transaction.IsDisposed);
                    Assert.True(connection.IsOpen);
                }
            }
        }

        Assert.Equal(0, ((FakeTransaction)connection.Transaction).Level);
        Assert.False(connection.Transaction.IsActive);
        Assert.True(connection.Transaction.IsDisposed);
        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Abort()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
            int count = 3;

            for (int i = 0; i < count; i++)
            {
                connection.Transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(((FakeTransaction)connection.Transaction).Level == (i + 1));
                Assert.True(connection.Transaction.IsActive);
                Assert.False(connection.Transaction.IsDisposed);
            }

            connection.Transaction.Abort();
            Assert.False(connection.IsOpen);
            Assert.False(((FakeTransaction)connection.Transaction).Level > 0);
            Assert.False(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);
        }

        Assert.Equal(0, ((FakeTransaction)connection.Transaction).Level);
        Assert.False(connection.Transaction.IsActive);
        Assert.True(connection.Transaction.IsDisposed);

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_And_Dispose()
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
                transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(((FakeTransaction)transaction).Level > 0);
                Assert.True(transaction.IsActive);
                Assert.False(transaction.IsDisposed);
            }

            Assert.False(connection.IsOpen);
            Assert.Equal(0, ((FakeTransaction)transaction).Level);
            Assert.False(transaction.IsActive);
            Assert.True(transaction.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Dispose_Connection()
    {
        var engine = new FakeEngine();
        IConnection connection;
        int count = 3;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            for (int i = 0; i < count; i++)
            {
                connection.Transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(((FakeTransaction)connection.Transaction).Level == (i + 1));
                Assert.True(connection.Transaction.IsActive);
                Assert.False(connection.Transaction.IsDisposed);
            }
        }

        Assert.Equal(0, ((FakeTransaction)connection.Transaction).Level);
        Assert.False(connection.Transaction.IsActive);
        Assert.True(connection.Transaction.IsDisposed);

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }
}