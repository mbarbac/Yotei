namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public class Test_Connection
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();
        using var connection = new FakeConnection(engine);

        Assert.Equal(Code.Connection.RETRIES, connection.Retries);
        Assert.Equal(Code.Connection.RETRYINTERVALMS, connection.RetryInterval.Milliseconds);
        Assert.NotNull(connection.Transaction);

        Assert.False(connection.IsDisposed);
        Assert.False(connection.Transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        using var source = new FakeConnection(engine) { Retries = 100 };
        using var target = source.Clone();

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
        var engine = new FakeEngine();

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);

            connection.Open();
            Assert.True(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);

            connection.Transaction.Start();
            Assert.True(connection.Transaction.IsActive);

            connection.Close();
            Assert.False(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);
        }

        Assert.True(connection.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Open_Dispose()
    {
        IConnection connection;
        var engine = new FakeEngine();

        using (connection = new FakeConnection(engine))
        {
            Assert.False(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);

            connection.Open();
            Assert.True(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);

            connection.Transaction.Start();
            Assert.True(connection.Transaction.IsActive);
        }
        Assert.False(connection.IsOpen);
        Assert.False(connection.Transaction.IsActive);

        Assert.True(connection.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Managed_Transactions()
    {
        List<ITransaction> items = [];
        IConnection connection;
        var engine = new FakeEngine();

        using (connection = new FakeConnection(engine))
        {
            for (int i = 0; i < 10; i++)
            {
                var item = new FakeTransaction(connection);
                item.Start();
                items.Add(item);
            }
            Assert.True(connection.IsOpen);
            Assert.False(connection.Transaction.IsActive);
        }

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            Assert.False(item.IsActive);
            Assert.True(item.IsDisposed);
        }
        Assert.True(connection.IsDisposed);
        Assert.True(connection.Transaction.IsDisposed);
    }
}