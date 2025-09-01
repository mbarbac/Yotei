namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_Transaction_Implicit
{
    //[Enforced]
    [Fact]
    public static void Test_Exists()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);
        }
        Assert.True(connection.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_Commit()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);

            var num = 3;
            for (int i = 1; i <= num; i++)
            {
                connection.Transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(connection.Transaction.IsActive);
                Assert.Equal(i, connection.Transaction.Level);
            }
            for (int i = num - 1; i >= 0; i--)
            {
                if (i == 0) { }

                connection.Transaction.Commit();
                Assert.Equal(i > 0, connection.IsOpen);
                Assert.Equal(i > 0, connection.Transaction.IsActive);
                Assert.Equal(i, connection.Transaction.Level);
            }

            Assert.False(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);

            connection.Dispose();
        }
        Assert.True(connection.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
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
            Assert.False(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);

            var num = 3;
            for (int i = 1; i <= num; i++)
            {
                connection.Transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(connection.Transaction.IsActive);
                Assert.Equal(i, connection.Transaction.Level);
            }

            connection.Transaction.Abort();
            Assert.False(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);            
        }
        Assert.True(connection.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_EplicitAbort()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);

            var num = 3;
            for (int i = 1; i <= num; i++)
            {
                connection.Transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(connection.Transaction.IsActive);
                Assert.Equal(i, connection.Transaction.Level);
            }

            connection.Transaction.Abort();
            Assert.False(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);
        }
        Assert.True(connection.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start_ConnectionDisposed()
    {
        var engine = new FakeEngine();
        IConnection connection;

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);
            Assert.False(connection.Transaction.IsDisposed);

            var num = 3;
            for (int i = 1; i <= num; i++)
            {
                connection.Transaction.Start();
                Assert.True(connection.IsOpen);
                Assert.True(connection.Transaction.IsActive);
                Assert.Equal(i, connection.Transaction.Level);
            }
        }

        Assert.True(connection.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }
}