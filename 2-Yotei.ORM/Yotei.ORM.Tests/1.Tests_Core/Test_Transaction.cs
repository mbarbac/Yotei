using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Core;

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
            using (transaction = new FakeTransaction(connection))
            {
                for (int i = 0; i < count; i++)
                {
                    transaction.Start();

                    Assert.True(connection.IsOpen);
                    Assert.False(connection.IsDisposed);

                    Assert.False(transaction.IsDisposed);
                    Assert.True(transaction.IsActive);
                    Assert.Equal(i + 1, transaction.Level);
                }

                for (int i = count - 1; i >= 0; i--)
                {
                    transaction.Commit();

                    if (i > 0)
                    {
                        Assert.True(connection.IsOpen);
                        Assert.False(connection.IsDisposed);

                        Assert.False(transaction.IsDisposed);
                        Assert.True(transaction.IsActive);
                        Assert.Equal(i, transaction.Level);
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

            Assert.Equal(0, transaction.Level);
            Assert.False(transaction.IsActive);
            Assert.True(transaction.IsDisposed);
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
            using (transaction = new FakeTransaction(connection))
            {
                for (int i = 0; i < count; i++)
                {
                    transaction.Start();

                    Assert.True(connection.IsOpen);
                    Assert.False(connection.IsDisposed);

                    Assert.False(transaction.IsDisposed);
                    Assert.True(transaction.IsActive);
                    Assert.Equal(i + 1, transaction.Level);
                }

                transaction.Abort();

                Assert.False(connection.IsOpen);
                Assert.False(connection.IsDisposed);

                Assert.False(transaction.IsDisposed);
                Assert.False(transaction.IsActive);
                Assert.Equal(0, transaction.Level);
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
    public static void Test_Start_And_Dispose_Transaction()
    {
        var engine = new FakeEngine();
        IConnection connection;
        ITransaction transaction;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);

            int count = 3;
            using (transaction = new FakeTransaction(connection))
            {
                for (int i = 0; i < count; i++)
                {
                    transaction.Start();

                    Assert.True(connection.IsOpen);
                    Assert.False(connection.IsDisposed);

                    Assert.False(transaction.IsDisposed);
                    Assert.True(transaction.IsActive);
                    Assert.Equal(i + 1, transaction.Level);
                }
            }

            Assert.True(transaction.IsDisposed);
            Assert.Equal(0, transaction.Level);
            Assert.False(transaction.IsActive);

            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
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

            using (transaction = connection.CreateTransaction())
            {
                int count = 3;
                for (int i = 0; i < count; i++)
                {
                    transaction.Start();

                    Assert.True(connection.IsOpen);
                    Assert.False(connection.IsDisposed);

                    Assert.False(transaction.IsDisposed);
                    Assert.True(transaction.IsActive);
                    Assert.Equal(i + 1, transaction.Level);
                }
            }

            Assert.False(transaction.IsActive);
            Assert.Equal(0, transaction.Level);
            Assert.True(transaction.IsDisposed);

            Assert.False(connection.IsOpen);
            Assert.False(connection.IsDisposed);
        }

        Assert.False(connection.IsOpen);
        Assert.True(connection.IsDisposed);
    }
}