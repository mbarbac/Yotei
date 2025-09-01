namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_Transaction_Explicit
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
            using (transaction = new FakeTransaction(connection))
            {
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);
            }

            Assert.False(connection.IsDisposed);
            Assert.True(transaction.IsDisposed);
        }
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Commit()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            using (transaction = new FakeTransaction(connection))
            {
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);

                var num = 3;
                for (int i = 1; i <= num; i++)
                {
                    transaction.Start();
                    Assert.True(connection.IsOpen);
                    Assert.True(transaction.IsActive);
                    Assert.Equal(i, transaction.Level);
                }
                for (int i = num - 1; i >= 0; i--)
                {
                    transaction.Commit();
                    Assert.Equal(i > 0, connection.IsOpen);
                    Assert.Equal(i > 0, transaction.IsActive);
                    Assert.Equal(i, transaction.Level);
                }

                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);
            }

            Assert.False(connection.IsDisposed);
            Assert.True(transaction.IsDisposed);
        }
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
            using (transaction = new FakeTransaction(connection))
            {
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);

                var num = 3;
                for (int i = 1; i <= num; i++)
                {
                    transaction.Start();
                    Assert.True(connection.IsOpen);
                    Assert.True(transaction.IsActive);
                    Assert.Equal(i, transaction.Level);
                }

                transaction.Abort();
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);
            }

            Assert.False(connection.IsDisposed);
            Assert.True(transaction.IsDisposed);
        }
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_ImplicitAbort()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            using (transaction = new FakeTransaction(connection))
            {
                Assert.False(connection.IsOpen);
                Assert.False(transaction.IsActive);
                Assert.False(transaction.IsDisposed);

                var num = 3;
                for (int i = 1; i <= num; i++)
                {
                    transaction.Start();
                    Assert.True(connection.IsOpen);
                    Assert.True(transaction.IsActive);
                    Assert.Equal(i, transaction.Level);
                }
            }

            Assert.False(connection.IsDisposed);
            Assert.True(transaction.IsDisposed);
        }
        Assert.True(connection.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_ConnectionDisposed()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            transaction = new FakeTransaction(connection);
            Assert.False(connection.IsOpen);
            Assert.False(transaction.IsActive);
            Assert.False(transaction.IsDisposed);

            var num = 3;
            for (int i = 1; i <= num; i++)
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